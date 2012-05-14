using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using CorePlus.Parser.Javascript;
using CorePlus.IO;
using CorePlus.Core;
using CorePlus.Api;

namespace DocPlus.Javascript.Demo {
    public partial class MainForm :Form {


        const string PR = "属性";

        void TestToken() {

            StringBuilder sb = new StringBuilder(source.Text);

            TokenType type = Token.GetTokenType(sb);

            kf.Text = type.ToString();


            tabControl1.SelectedIndex = 4;
        }

        void TestLatex() {
            latex.Items.Clear();

            Scanner c = new Scanner(new StringBuffer(source.Text));

            while (c.Read().Type != TokenType.EOS) {
                latex.Items.Add(c.CurrentToken.ToString());
            }

            tabControl1.SelectedIndex = 3;
        }

        void TestParser() {

            Console.Clear();

            Parser p = new Parser();
            Script s = p.ParseString(source.Text);

            parser.Nodes.Clear();
            parser.SuspendLayout();
            Bind(parser.Nodes.Add("Javascript"), s);
            foreach (TreeNode c in parser.Nodes) {
                Expand(c);
            }

            parser.ResumeLayout(true);
            tabControl1.SelectedIndex = 2;
        }

        void TestDoc() {

            DocProject proj = new DocProject();

            DocParser dp = new DocParser(proj);

            dp.ParseString(source.Text);

            ShowGlobal(dp.Global, checkBox1.Checked);

            tabControl1.SelectedIndex = 1;
        }

        public MainForm() {
            InitializeComponent();

            source.Text =

                @"";
        }

        private void TestVariants() {
            
            DocProject proj = new DocProject();

            DocParser dp = new DocParser(proj);

            dp.ParseString(source.Text);

            ShowGlobal(dp.Global, checkBox1.Checked);
        }

        private void TestMergeDoc() {

            DocProject proj = new DocProject();

            DocParser dp = new DocParser(proj);

            dp.ParseString(source.Text);

            ShowGlobal(dp.Global, checkBox1.Checked);

            ApiDoc doc = dp.Build();

            doc.Save("test.txt");

            Console.Write(FileHelper.ReadAllText("test.txt", Encoding.Default));

            tabControl1.SelectedIndex = 1;

            //  Py.Logging.Logger.Clear();
            //  Py.Doc.Javascript.JavaScriptDocParser p = new Doc.Javascript.JavaScriptDocParser();

            ////  p.ParseFile("define.js");

            //  p.NewDocument();

            //  p.ParseString(source.Text);

            //  p.SaveDocument();

            //  Py.Doc.Javascript.DocParseHelper.Show( p.Document );

            //  lists.Items.Clear();

            //  tabControl1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e) {
            Properties.Settings.Default.Save();
            TestParser();
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
                    Contain(tnode);

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

        bool HideName(string name) {
            return name == "IsInlineable" || name == "HasSideEffects" || name == "IsEmpty";
        }

        void SimpleProperty(TreeNode node) {
            node.ForeColor = Color.Gray;
        }

        void Contain(TreeNode node) {
            node.NodeFont = new Font(node.NodeFont ?? SystemFonts.DefaultFont, FontStyle.Italic);
        }

        void Select(Location start, Location end) {
            int startPos = source.GetFirstCharIndexFromLine(start.Line - 1) + start.Column - 1;
            int endPos = source.GetFirstCharIndexFromLine(end.Line - 1) + end.Column - 1;
           
            
            
             source.Select(startPos, endPos - startPos);
        }

        private void button2_Click(object sender, EventArgs e) {
            Properties.Settings.Default.Save();
            TestLatex();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            Properties.Settings.Default.Save();
        }

        private void errorHint_Click(object sender, EventArgs e) {

        }

        static void ShowGlobal(Variant v, bool showIgnore) {
            List<Variant> displayed = new List<Variant>() { v };
            foreach (var v2 in v) {
                ShowGlobal(v2.Value, null, v2.Key, displayed, showIgnore);
            }
        }

        static void ShowGlobal(Variant v, string prefix, string name, List<Variant> displayed, bool showIgnore) {

            if (!displayed.Contains(v)) {
                displayed.Add(v);

                if(showIgnore || !v.Ignore) {

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(prefix);
                    Console.Write(name);
                    Console.ForegroundColor = v.Ignore ? ConsoleColor.Gray : ConsoleColor.White;
                    Console.Write("      ");
                    Console.Write(Str.Substring(v.GetRaw().Replace("\r\n", "   "), 0, Console.WindowWidth - 1));
                    Console.WriteLine();

                }

                foreach (var v2 in v) {
                    ShowGlobal(v2.Value, prefix + name + '.', v2.Key, displayed, showIgnore);
                }
            } else {

                if(showIgnore || !v.Ignore) {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write(prefix);
                    Console.Write(name);
                    Console.ForegroundColor = v.Ignore ? ConsoleColor.Gray : ConsoleColor.White;
                    Console.Write("      ");
                    Console.Write(Str.Substring(v.GetRaw().Replace("\r\n", "   "), 0, Console.WindowWidth - 1));
                    Console.WriteLine();

                }
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            Properties.Settings.Default.Save();
            Console.Clear();
            TestToken();
        }

        private void source_KeyUp(object sender, KeyEventArgs e) {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A) {
                ((TextBox)sender).SelectAll();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            Properties.Settings.Default.Save();
            Console.Clear();
            TestDoc();
        }

        private void parser_AfterSelect(object sender, TreeViewEventArgs e) {
           CorePlus.Parser.Javascript.Node d = e.Node.Tag as CorePlus.Parser.Javascript.Node;

            if (d != null) {
                Select(d.StartLocation, d.EndLocation);
                source.ScrollToCaret();
            }
        }

        private void button5_Click(object sender, EventArgs e) {
            Properties.Settings.Default.Save();
            Console.Clear();
            TestMergeDoc();
        }

        private void button6_Click(object sender, EventArgs e) {
            Properties.Settings.Default.Save();
            Console.Clear();
            TestVariants();
        }
    }


}
