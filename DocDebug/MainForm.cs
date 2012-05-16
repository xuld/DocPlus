using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DocPlus.Javascript;

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
            scintilla.Margins[0].Width = 20;
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
            splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            global::DocPlus.GUI.Debug.Properties.Settings.Default.SourceCode = scintilla.Text;
            global::DocPlus.GUI.Debug.Properties.Settings.Default.Save();
        }

        private void 生成ToolStripMenuItem_Click(object sender, EventArgs e) {
            Build();
        }

        void Build() {
            
            splitContainer1.Panel2Collapsed = false;
            _project.ProgressReporter.Start();
            DocParser parser = new DocParser(_project);
            parser.ParseString(scintilla.Text);
            _project.ProgressReporter.Complete();
            ShowData( parser.Data  );
        }

        void ShowData(DocData data) {
            foreach(var kv in data.DocComments) {
                TreeNode node = treeView1.Nodes.Add(kv.Key);
                node.Tag = kv.Value;
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
            if(e.Node.Tag != null) {
                DocComment dc = (DocComment)e.Node.Tag;

                int pos = scintilla.FindColumn(dc.StartLocation.Line - 1, dc.StartLocation.Column - 1);

                scintilla.Selection.Start = pos;

                pos = scintilla.FindColumn(dc.EndLocation.Line - 1, dc.EndLocation.Column - 1);
                scintilla.Selection.End = pos;
            }
        }


    }
}
