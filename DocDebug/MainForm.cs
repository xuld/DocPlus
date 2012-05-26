using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DocPlus.Javascript;
using CorePlus.Parser.Javascript;
using System.Collections;

namespace DocPlus.GUI.Debug {
    public partial class MainForm : Form {

        DocProject _project = new DocProject();

        DocPlus.GUI.TraceWindowControl tw = new TraceWindowControl();

        ScintillaNET.Scintilla scintilla = new ScintillaNET.Scintilla();

        public MainForm() {
            InitializeComponent();

            tw.Dock = DockStyle.Fill;
            tw.CloseClick += new EventHandler(tw_CloseClick);
            splitContainer1.Panel2.Controls.Add(tw);

            scintilla.Dock = DockStyle.Fill;
            scintilla.Margins[0].Width = 40;
            scintilla.ConfigurationManager.Language = "js";
            scintilla.BorderStyle = BorderStyle.None;
            tabPage1.Controls.Add(scintilla);
            scintilla.Text = global::DocPlus.GUI.Debug.Properties.Settings.Default.SourceCode;

            splitContainer1.Panel2Collapsed = true;
            propertyGrid1.SelectedObject = _project;

            _project.ProgressReporter = new ProgressReporter(tw);
            
        }

        void tw_CloseClick(object sender, EventArgs e) {
            splitContainer1.Panel2Collapsed = true;
        }

        private void 控制台CToolStripMenuItem_Click(object sender, EventArgs e) {
            ShowConsole();
        }

        void ShowConsole() {
            splitContainer1.Panel2Collapsed = false;
            tw.Show();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            global::DocPlus.GUI.Debug.Properties.Settings.Default.SourceCode = scintilla.Text;
            global::DocPlus.GUI.Debug.Properties.Settings.Default.Save();
        }

        private void 生成ToolStripMenuItem_Click(object sender, EventArgs e) {
            Build();
        }

        void Build() {

            ShowConsole();
            _project.ProgressReporter.Start();
            DocParser parser = new DocParser(_project);
            parser.ParseString(scintilla.Text.Replace("\t", "    "));
            _project.ProgressReporter.Complete();
            ShowData( parser.End()  );
        }

        void ShowData(DocData data) {
            treeView1.Nodes.Clear();

            treeView1.SuspendLayout();
            treeView1.Tag = 1;
            foreach(var kv in data.DocComments) {
                TreeNode node = treeView1.Nodes.Add(kv.Key);
                node.Tag = kv.Value;

                foreach(string k in kv.Value) {
                    if (k == "membertype" || k == "memberaccess" || k == "memberattribute") {
                        node.Nodes.Add("@" + kv.Value[k]).Tag = kv.Value;
                    } else if(k == "memberOf" || k == "name"){
                    }else{
                        node.Nodes.Add(String.Concat("@", k, " ", kv.Value[k].ToString())).Tag = kv.Value;
                    }
                }
            }

            treeView1.ResumeLayout(true);
        }

        void OnSelectNode(TreeNode treeNode) {
            if (treeNode.Tag != null) {
                int i = (int)treeView1.Tag;
                Location start, end;
                switch (i) {
                    case 1:
                        DocComment dc = (DocComment)treeNode.Tag;
                        start = dc.StartLocation;
                        end = dc.EndLocation;
                        break;

                    case 2:
                        Node node = (Node)treeNode.Tag;
                        start = node.StartLocation;
                        end = node.EndLocation;
                        break;

                    default:
                        return;
                }

                int pos = scintilla.FindColumn(start.Line - 1, start.Column - 1);

                scintilla.Selection.Start = pos;

                if (start.Line == end.Line) {
                    pos += end.Column - start.Column;
                } else {
                    pos = scintilla.FindColumn(end.Line - 1, end.Column - 1);
                }
                scintilla.Selection.End = pos;
                scintilla.Scrolling.ScrollToCaret();
            }
        }

        void Bind(TreeNode node, CorePlus.Parser.Javascript.Node codeNode) {
            System.Type type = codeNode.GetType();
            node.Text = String.Format("{0}<{1}>", codeNode.GetType().Name, codeNode);
            node.Tag = codeNode;
            TreeNode prop = node.Nodes.Add(PR);
            SimpleProperty(prop);
            foreach (System.Reflection.PropertyInfo p in type.GetProperties()) {




                TreeNode tnode = new TreeNode();
                object value = p.GetValue(codeNode, null);


                if ((p.Name == "Parent" || p.Name == "Target") && value is BreakableStatement) {
                    tnode.Text = p.Name + "=" + value;
                    prop.Nodes.Add(tnode);
                    continue;
                }


                if (value is CorePlus.Parser.Javascript.Node) {


                    Bind(tnode, (CorePlus.Parser.Javascript.Node)value);

                    node.Nodes.Add(tnode);
                } else if (value is IEnumerable && !(value is string)) {
                    tnode.Text = p.Name;
                    bool isNode = false;
                    foreach (object node2 in (IEnumerable)value) {
                        TreeNode node3 = new TreeNode();
                        if (node2 is CorePlus.Parser.Javascript.Node) {
                            Bind(node3, (CorePlus.Parser.Javascript.Node)node2);
                            isNode = true;
                        } else {
                            node3.Text = node2.ToString();
                            SimpleProperty(node3);
                        }
                        tnode.Nodes.Add(node3);
                    }
                    Contains(tnode);

                    if (isNode) {
                        node.Nodes.Add(tnode);
                    } else {
                        prop.Nodes.Add(tnode);
                    }
                } else if (!HideName(p.Name)) {
                    tnode.Text = String.Format("{0} = {1}", p.Name, value);
                    //   SimpleProperty(tnode);
                    prop.Nodes.Add(tnode);
                    continue;
                } else {
                    continue;
                }
            }
        }

        bool HideName(string name) {
            return name == "IsInlineable" || name == "HasSideEffects" || name == "IsEmpty";
        }

        void SimpleProperty(TreeNode node) {
            node.ForeColor = Color.Gray;
        }

        void Contains(TreeNode node) {
            node.NodeFont = new Font(node.NodeFont ?? SystemFonts.DefaultFont, FontStyle.Italic);
        }

        const string PR = "属性";

        void Expand(TreeNode node) {
            if (node.Text != PR) {
                if (node.Nodes.Count == 1 && node.Nodes[0].Text == PR)
                    return;

                node.Expand();


                foreach (TreeNode c in node.Nodes) {
                    Expand(c);
                }
            }

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e) {

            Parser p = new Parser();
            Script s = p.ParseString(scintilla.Text.Replace("\t", "    "));
            
            treeView1.Nodes.Clear();

            treeView1.SuspendLayout();
            treeView1.Tag = 2;
            Bind(treeView1.Nodes.Add("Javascript"), s);
            foreach (TreeNode c in treeView1.Nodes) {
                Expand(c);
            }

            treeView1.ResumeLayout(true);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e) {

            ShowConsole();
            _project.ProgressReporter.Start();
            DocParser parser = new DocParser(_project);
            DocComment[] comments = parser.GetComments(scintilla.Text.Replace("\t", "    "));
            _project.ProgressReporter.Complete();


            treeView1.Nodes.Clear();

            treeView1.SuspendLayout();
            treeView1.Tag = 1;
            foreach (DocComment kv in comments) {
                TreeNode node = treeView1.Nodes.Add(kv.ToString());
                node.Tag = kv;

                foreach (string k in kv) {
                    node.Nodes.Add("@" + k + " " + kv[k]).Tag = kv;
                }
            }

            treeView1.ResumeLayout(true);


        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) {
            OnSelectNode(e.Node);
        }


    }
}
