using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using DocPlus.Core;

namespace DocPlus.GUI {

    /// <summary>
    /// 程序主窗口。
    /// </summary>
    public partial class MainForm :Form {

        /// <summary>
        /// 当前打开的项目。
        /// </summary>
        IDocProject _currentProject;

        bool _isDirty = false;

        string _currentPath;

        TraceWindowControl _tw = new TraceWindowControl();

        /// <summary>
        /// 获取或设置当前文件是否被修改。
        /// </summary>
        public bool IsDirty {
            get {
                return _isDirty;
            }
            set {
                _isDirty = value;
                if(value != Text.EndsWith("*")) {
                    if(value) {
                        Text += "*";
                    } else {
                        Text = Text.Substring(0, Text.Length - 1);
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置当前打开的项目文件地址。
        /// </summary>
        public string CurrentPath {
            get {
                return _currentPath;
            }
            set {
                _currentPath = value;
                if (_currentPath == null) {
                    Text = CurrentProject == null ? SystemManager.Title : ("无标题 - " + SystemManager.Title);
                } else {
                    SystemManager.AddRecentFile(value);
                    Text = _currentPath + " - " + SystemManager.Title;
                }
            }
        }

        /// <summary>
        /// 获取当前正在使用的项目。
        /// </summary>
        public IDocProject CurrentProject {
            get {
                return _currentProject;
            }
            set {
                _currentProject = value;

                if (value != null) {
                    propertyGrid.SelectedObject = _currentProject;
                    UpdateList();
                }
                UpdateUI(_currentProject != null);
            }
        }

        public MainForm() {
            InitializeComponent();
            _tw.Dock = DockStyle.Fill;
            _tw.CloseClick += new EventHandler(tw_CloseClick);
            splitContainer1.Panel2.Controls.Add(_tw);
            Text = SystemManager.Title;
            UpdateUI(false);
        }

        void OpenProject(string path) {
            if (!CloseProject())
                return;
            CurrentProject = SystemManager.OpenProject(path);
            if (CurrentProject != null) {
                CurrentPath = path;
                CurrentProject.ProgressReporter = new ProgressReporter(_tw);
                Hint("就绪");
            }
        }

        /// <summary>
        /// 关闭当前打开的项目。
        /// </summary>
        /// <returns>操作如果被用户取消，则返回 false 。</returns>
        bool CloseProject() {

            if(CurrentProject != null) {

                if(IsDirty) {
                    switch(new CloseMsgBox(SystemManager.Title, "是否保存对当前项目的更改?").ShowDialog(this)) {
                        case System.Windows.Forms.DialogResult.Yes:
                            miSaveProject_Click(this, EventArgs.Empty);
                            break;

                        case System.Windows.Forms.DialogResult.Cancel:
                            return false;

                    }

                    IsDirty = false;
                }

                CurrentProject.Close();
                lbFiles.Items.Clear();
                CurrentProject = null;
                CurrentPath = null;



            }

            return true;
        }

        void OpenRecentProject() {
            string s = SystemManager.RecentProject;
            if (s != null) {
                OpenProject(s);
            }
        }

        void Hint(string s) {
            toolStripStatusLabel1.Text = s;
        }

        void UpdateList() {
            lbFiles.Items.Clear();

            foreach(string file in _currentProject.Items) {
                lbFiles.Items.Add(file);
            }

            cmiSelectAll.Enabled = cmiToggle.Enabled = btnMoveUp.Enabled = btnMoveDown.Enabled = btnRemove.Enabled = miRemoveAll.Enabled = cmiRemove.Enabled = miRemove.Enabled = lbFiles.Items.Count > 0;
        }

        void UpdateUI(bool enable) {
            miProject.Enabled = miSaveProject.Enabled = miSaveAsProject.Enabled = miCloseProject.Enabled = miBuildProject.Enabled = splitContainer1.Enabled = enable;
        }

        void AddItems(string[] values) {
            CurrentProject.Items.AddRange(values);
            IsDirty = true;
            UpdateList();
            Hint(String.Format("已添加 {0} 项", values.Length));
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            CurrentProject.ProgressReporter.Start();
            CurrentProject.Build();
            CurrentProject.ProgressReporter.Complete();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Cancelled) {
                Hint("生成被取消。");
            } else if (e.Error == null) {
                Hint("全部生成完成。");
            } else {
                CurrentProject.ProgressReporter.Error(e.Error);
                Hint("生成失败。");
            }

            miCancel.Enabled = false;
        }

        private void MainForm_Load(object sender, EventArgs e) {

            foreach (var item in SystemManager.DocProjects) {
                miCreateProject.DropDownItems.Add(item.Key).Tag = item.Value;
            }

            if (SystemManager.DocProjects.Count == 0) {
                miCreateProject.DropDownItems.Add("(无可用的文档生成插件)").Enabled = false;
            }

            if (Properties.Settings.Default.OpenRecentProject)
                OpenRecentProject();
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e) {
            if (CurrentProject != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e) {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (s != null) {
                AddItems(s);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = !CloseProject();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {

            Properties.Settings.Default.Save();
        }

        private void miRecentProject_DropDownOpening(object sender, EventArgs e) {
            miRecentProject.DropDownItems.Clear();
            if (SystemManager.RecentFiles.Count == 0) {
                miRecentProject.DropDownItems.Add("(无历史项目)").Enabled = false;


                return;

            }
            foreach (var p in SystemManager.RecentFiles) {
                miRecentProject.DropDownItems.Insert(0, new ToolStripButton(p));
            }

        }

        private void miRecentProject_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            miFile.HideDropDown();

            string path = e.ClickedItem.Text;
            if (path != "(无历史项目)") {
                OpenProject(path);
            }
        }

        private void miCreateProject_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            miFile.HideDropDown();
            CloseProject();
            CurrentProject = SystemManager.CreateProject((string)e.ClickedItem.Tag);
            CurrentPath = null;
        }

        private void miOpenProject_Click(object sender, EventArgs e) {
            using (OpenFileDialog d = new OpenFileDialog()) {
                d.Filter = "DocPlus 项目文件(*.docproj)|*.docproj|所有文件(*.*)|*.*";
                d.DefaultExt = ".docproj";
                if (DialogResult.OK == d.ShowDialog()) {
                    OpenProject(d.FileName);
                }
            }
        }

        private void miCloseProject_Click(object sender, EventArgs e) {
            CloseProject();
        }

        private void miSaveProject_Click(object sender, EventArgs e) {
            if(CurrentPath == null) {
                miSaveAsProject_Click(sender, e);
            } else if(IsDirty) {
                CurrentProject.Save(CurrentPath);
                IsDirty = false;
            }
           

            Hint("已保存的项");
        }

        private void miSaveAsProject_Click(object sender, EventArgs e) {
            using(SaveFileDialog d = new SaveFileDialog()) {
                d.Filter = "DocPlus 项目文件(*.docproj)|*.docproj|所有文件(*.*)|*.*";
                d.DefaultExt = ".docproj";
                d.FileName = CurrentPath ?? "无标题";
                if(DialogResult.OK == d.ShowDialog()) {
                    CurrentProject.Save(d.FileName);
                    IsDirty = false;
                    CurrentPath = d.FileName;
                }
            }

            Hint("已保存的项");
        }

        private void miAddFile_Click(object sender, EventArgs e) {
            using (OpenFileDialog ofd = new OpenFileDialog()) {

                Hint(null);
                ofd.Filter = CurrentProject.FileFilter;
                ofd.DefaultExt = CurrentProject.DefaultExt;
                ofd.Multiselect = true;
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    AddItems(ofd.FileNames);
                } else {
                    Hint("就绪");
                }

            }
        }

        private void miAddDirectory_Click(object sender, EventArgs e) {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog()) {

                Hint(null);
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    CurrentProject.Items.Add(fbd.SelectedPath);
                    IsDirty = true;
                    UpdateList();
                    Hint(String.Format("已添加 {0} 个文件夹", 1));
                } else {
                    Hint("就绪");
                }
            }


        }

        private void miBuild_Click(object sender, EventArgs e) {




            if (backgroundWorker.IsBusy) {
                if (MessageBox.Show(this, "上次生成操作未完成。是否撤销当前操作并重新生成?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                    backgroundWorker.CancelAsync();
                else
                    return;
            }

            miSaveProject_Click(sender, e);

            if (CurrentPath == null) {
                Hint("项目未保存。操作已被取消。");
                return;
            }

            if (CurrentProject.Items.Count == 0) {
                Hint("未添加任何文件。");
                return;
            }

            if (CurrentProject.TargetPath == null)
                CurrentProject.TargetPath = Path.ChangeExtension(CurrentPath, ".api");

            miCancel.Enabled = true;
            miOutput.Checked = true;

            Hint("正在生成...");

            backgroundWorker.RunWorkerAsync();
        }

        private void miRemoveAll_Click(object sender, EventArgs e) {
            CurrentProject.Items.Clear();

            IsDirty = true;
            UpdateList();
        }

        private void miRemove_Click(object sender, EventArgs e) {
            var sels = lbFiles.SelectedIndices;

            if(sels.Count > 1 && MessageBox.Show(this, "是否移除选中的 " + sels.Count + " 个文件", SystemManager.Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != System.Windows.Forms.DialogResult.OK)
                return;

            if(sels.Count == 0) {
                return;
            }

            int a = sels[0];
            Hint("移除 " + sels.Count + " 个文件");
            for (int i = sels.Count - 1; i >= 0; i--) {
                CurrentProject.Items.RemoveAt(sels[i]);
            }

            IsDirty = true;
            UpdateList();

            if(lbFiles.Items.Count > 0) {
                if(lbFiles.Items.Count > a) {
                    lbFiles.SelectedIndex = a;
                } else {
                    lbFiles.SelectedIndex = lbFiles.Items.Count - 1;
                }
            }



        }

        private void miSelectAll_Click(object sender, EventArgs e) {
            for (int i = 0; i < lbFiles.Items.Count; i++) {
                lbFiles.SetSelected(i, true);
            }
        }

        private void miToggle_Click(object sender, EventArgs e) {
            for (int i = 0; i < lbFiles.Items.Count; i++) {
                lbFiles.SetSelected(i, !lbFiles.GetSelected(i));
            }
        }

        private void miCancel_Click(object sender, EventArgs e) {
            if (backgroundWorker.IsBusy)
                backgroundWorker.CancelAsync();
        }

        private void miClearBuild_Click(object sender, EventArgs e) {
            CurrentProject.ClearBuild();
            Hint("清理已完成。");
        }

        private void miProperty_Click(object sender, EventArgs e) {
            miOptions.Checked = !miOptions.Checked;
        }

        private void miViewDoc_Click(object sender, EventArgs e) {
            if (CurrentProject != null) {
                string path = CurrentProject.TargetPath;
                if (Directory.Exists(path)) {
                    Utils.Explore(path);
                    return;
                }
            }
            Hint("文档尚未生成，无法查看。");
        }

        private void miOutput_CheckedChanged(object sender, EventArgs e) {
            splitContainer1.Panel2Collapsed = !miOutput.Checked;
        }

        private void miStatus_CheckedChanged(object sender, EventArgs e) {
            statusStrip.Visible = miStatus.Checked;
        }

        private void miOptions_CheckedChanged(object sender, EventArgs e) {
            splitContainer2.Panel2Collapsed = !miOptions.Checked;
        }

        private void lbFiles_DoubleClick(object sender, EventArgs e) {
            cmiExplorer_Click(sender, e);
        }

        private void lbFiles_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                int index = this.lbFiles.IndexFromPoint(e.X, e.Y);
                if (index != -1 && !lbFiles.SelectedIndices.Contains(index)) {
                    lbFiles.SelectedIndices.Clear();
                    lbFiles.SelectedIndex = index;

                }
            }
        }

        private void btnMoveUp_Click(object sender, EventArgs e) {

            var sels = lbFiles.SelectedIndices;

            if (sels.Count == 0) {
                return;
            }

            int[] s = new int[sels.Count];

            sels.CopyTo(s, 0);

            if (s[0] <= 0)
                return;

            sels.Clear();

            for (int i = 0; i < s.Length; i++) {
                int t = s[i];
                var c = CurrentProject.Items[t];
                CurrentProject.Items[t] = CurrentProject.Items[t - 1];
                CurrentProject.Items[t - 1] = c;

                s[i] = t - 1;
            }


            UpdateList();

            for(int i = 0; i < s.Length; i++) {
                sels.Add(s[i]);
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e) {

            var sels = lbFiles.SelectedIndices;

            if (sels.Count == 0) {
                return;
            }


            int[] s = new int[sels.Count];

            sels.CopyTo(s, 0);

            if (s[s.Length - 1] >= CurrentProject.Items.Count - 1)
                return;

            sels.Clear();

            for (int i = s.Length - 1; i >= 0; i--) {
                int t = s[i];
                var c = CurrentProject.Items[t];
                CurrentProject.Items[t] = CurrentProject.Items[t + 1];
                CurrentProject.Items[t + 1] = c;

                s[i] = t + 1;
            }



            UpdateList();

            for(int i = 0; i < s.Length; i++) {
                sels.Add(s[i]);
            }
        }

        private void tw_CloseClick(object sender, EventArgs e) {
            miOutput.Checked = false;
        }

        private void miHelp_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("http://work.xuld.net/docplus");
        }

        private void miAbout_Click(object sender, EventArgs e) {

            new AboutBox().ShowDialog();
        }

        private void miExit_Click(object sender, EventArgs e) {
            Close();
        }

        private void cmiExplorer_Click(object sender, EventArgs e) {
            string i = (string)lbFiles.SelectedItem;
            if (i != null)
                Utils.Explore(i);
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
            IsDirty = true;
        }

        private void miCreateProject_Click(object sender, EventArgs e) {

        }

    }

}
