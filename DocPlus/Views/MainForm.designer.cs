namespace DocPlus.GUI {
    partial class MainForm {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.miFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miCreateProject = new System.Windows.Forms.ToolStripMenuItem();
            this.miOpenProject = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.miCloseProject = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.miSaveProject = new System.Windows.Forms.ToolStripMenuItem();
            this.miSaveAsProject = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.miRecentProject = new System.Windows.Forms.ToolStripMenuItem();
            this.无记录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.miView = new System.Windows.Forms.ToolStripMenuItem();
            this.miOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.miOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.miStatus = new System.Windows.Forms.ToolStripMenuItem();
            this.miProject = new System.Windows.Forms.ToolStripMenuItem();
            this.miAddFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miAddDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.miRemoveAll = new System.Windows.Forms.ToolStripMenuItem();
            this.miRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.miSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.miToggle = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.miProperty = new System.Windows.Forms.ToolStripMenuItem();
            this.miBuildProject = new System.Windows.Forms.ToolStripMenuItem();
            this.miBuild = new System.Windows.Forms.ToolStripMenuItem();
            this.miCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.miClearBuild = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.miViewDoc = new System.Windows.Forms.ToolStripMenuItem();
            this.miHelpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.miHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.miAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnOptions = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAddDirectory = new System.Windows.Forms.Button();
            this.btnAddFile = new System.Windows.Forms.Button();
            this.lbFiles = new System.Windows.Forms.ListBox();
            this.cbFiles = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiAddFile = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiAddDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiToggle = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiExplorer = new System.Windows.Forms.ToolStripMenuItem();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.cbFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip.Location = new System.Drawing.Point(0, 415);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(626, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(32, 17);
            this.toolStripStatusLabel1.Text = "就绪";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFile,
            this.miView,
            this.miProject,
            this.miBuildProject,
            this.miHelpMenu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip.Size = new System.Drawing.Size(626, 25);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip1";
            // 
            // miFile
            // 
            this.miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCreateProject,
            this.miOpenProject,
            this.toolStripSeparator2,
            this.miCloseProject,
            this.toolStripMenuItem2,
            this.miSaveProject,
            this.miSaveAsProject,
            this.toolStripSeparator5,
            this.miRecentProject,
            this.toolStripMenuItem3,
            this.miExit});
            this.miFile.Name = "miFile";
            this.miFile.Size = new System.Drawing.Size(58, 21);
            this.miFile.Text = "文件(&F)";
            // 
            // miCreateProject
            // 
            this.miCreateProject.Name = "miCreateProject";
            this.miCreateProject.Size = new System.Drawing.Size(189, 22);
            this.miCreateProject.Text = "新建项目(&N)";
            this.miCreateProject.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.miCreateProject_DropDownItemClicked);
            this.miCreateProject.Click += new System.EventHandler(this.miCreateProject_Click);
            // 
            // miOpenProject
            // 
            this.miOpenProject.Name = "miOpenProject";
            this.miOpenProject.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.miOpenProject.Size = new System.Drawing.Size(189, 22);
            this.miOpenProject.Text = "打开项目(&O)";
            this.miOpenProject.Click += new System.EventHandler(this.miOpenProject_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(186, 6);
            // 
            // miCloseProject
            // 
            this.miCloseProject.Name = "miCloseProject";
            this.miCloseProject.Size = new System.Drawing.Size(189, 22);
            this.miCloseProject.Text = "关闭项目(&C)";
            this.miCloseProject.Click += new System.EventHandler(this.miCloseProject_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(186, 6);
            // 
            // miSaveProject
            // 
            this.miSaveProject.Name = "miSaveProject";
            this.miSaveProject.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.miSaveProject.Size = new System.Drawing.Size(189, 22);
            this.miSaveProject.Text = "保存项目(&S)";
            this.miSaveProject.Click += new System.EventHandler(this.miSaveProject_Click);
            // 
            // miSaveAsProject
            // 
            this.miSaveAsProject.Name = "miSaveAsProject";
            this.miSaveAsProject.Size = new System.Drawing.Size(189, 22);
            this.miSaveAsProject.Text = "项目另存为(&A)...";
            this.miSaveAsProject.Click += new System.EventHandler(this.miSaveAsProject_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(186, 6);
            // 
            // miRecentProject
            // 
            this.miRecentProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.无记录ToolStripMenuItem});
            this.miRecentProject.Name = "miRecentProject";
            this.miRecentProject.Size = new System.Drawing.Size(189, 22);
            this.miRecentProject.Text = "最近的项目(&R)";
            this.miRecentProject.DropDownOpening += new System.EventHandler(this.miRecentProject_DropDownOpening);
            this.miRecentProject.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.miRecentProject_DropDownItemClicked);
            // 
            // 无记录ToolStripMenuItem
            // 
            this.无记录ToolStripMenuItem.Name = "无记录ToolStripMenuItem";
            this.无记录ToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.无记录ToolStripMenuItem.Text = "(无记录)";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(186, 6);
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            this.miExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.miExit.Size = new System.Drawing.Size(189, 22);
            this.miExit.Text = "退出(&X)";
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // miView
            // 
            this.miView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miOutput,
            this.miOptions,
            this.miStatus});
            this.miView.Name = "miView";
            this.miView.Size = new System.Drawing.Size(60, 21);
            this.miView.Text = "视图(&V)";
            // 
            // miOutput
            // 
            this.miOutput.CheckOnClick = true;
            this.miOutput.Name = "miOutput";
            this.miOutput.Size = new System.Drawing.Size(152, 22);
            this.miOutput.Text = "输出(&T)";
            this.miOutput.CheckedChanged += new System.EventHandler(this.miOutput_CheckedChanged);
            // 
            // miOptions
            // 
            this.miOptions.CheckOnClick = true;
            this.miOptions.Name = "miOptions";
            this.miOptions.Size = new System.Drawing.Size(152, 22);
            this.miOptions.Text = "属性(&P)";
            this.miOptions.CheckedChanged += new System.EventHandler(this.miOptions_CheckedChanged);
            // 
            // miStatus
            // 
            this.miStatus.Checked = true;
            this.miStatus.CheckOnClick = true;
            this.miStatus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.miStatus.Name = "miStatus";
            this.miStatus.Size = new System.Drawing.Size(152, 22);
            this.miStatus.Text = "状态栏(&S)";
            this.miStatus.CheckedChanged += new System.EventHandler(this.miStatus_CheckedChanged);
            // 
            // miProject
            // 
            this.miProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAddFile,
            this.miAddDirectory,
            this.toolStripSeparator1,
            this.miRemoveAll,
            this.miRemove,
            this.miSelectAll,
            this.miToggle,
            this.toolStripSeparator4,
            this.miProperty});
            this.miProject.Name = "miProject";
            this.miProject.Size = new System.Drawing.Size(59, 21);
            this.miProject.Text = "项目(&P)";
            // 
            // miAddFile
            // 
            this.miAddFile.Name = "miAddFile";
            this.miAddFile.Size = new System.Drawing.Size(181, 22);
            this.miAddFile.Text = "添加文件(&F)...";
            this.miAddFile.Click += new System.EventHandler(this.miAddFile_Click);
            // 
            // miAddDirectory
            // 
            this.miAddDirectory.Name = "miAddDirectory";
            this.miAddDirectory.Size = new System.Drawing.Size(181, 22);
            this.miAddDirectory.Text = "添加目录(&D)...";
            this.miAddDirectory.Click += new System.EventHandler(this.miAddDirectory_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
            // 
            // miRemoveAll
            // 
            this.miRemoveAll.Name = "miRemoveAll";
            this.miRemoveAll.Size = new System.Drawing.Size(181, 22);
            this.miRemoveAll.Text = "移除全部";
            this.miRemoveAll.Click += new System.EventHandler(this.miRemoveAll_Click);
            // 
            // miRemove
            // 
            this.miRemove.Name = "miRemove";
            this.miRemove.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.miRemove.Size = new System.Drawing.Size(181, 22);
            this.miRemove.Text = "移除选中项";
            this.miRemove.Click += new System.EventHandler(this.miRemove_Click);
            // 
            // miSelectAll
            // 
            this.miSelectAll.Name = "miSelectAll";
            this.miSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.miSelectAll.Size = new System.Drawing.Size(181, 22);
            this.miSelectAll.Text = "全选(&A)";
            this.miSelectAll.Click += new System.EventHandler(this.miSelectAll_Click);
            // 
            // miToggle
            // 
            this.miToggle.Name = "miToggle";
            this.miToggle.Size = new System.Drawing.Size(181, 22);
            this.miToggle.Text = "反选";
            this.miToggle.Click += new System.EventHandler(this.miToggle_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(178, 6);
            // 
            // miProperty
            // 
            this.miProperty.Name = "miProperty";
            this.miProperty.Size = new System.Drawing.Size(181, 22);
            this.miProperty.Text = "属性(&P)";
            this.miProperty.Click += new System.EventHandler(this.miProperty_Click);
            // 
            // miBuildProject
            // 
            this.miBuildProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miBuild,
            this.miCancel,
            this.miClearBuild,
            this.toolStripSeparator3,
            this.toolStripMenuItem1,
            this.miViewDoc});
            this.miBuildProject.Name = "miBuildProject";
            this.miBuildProject.Size = new System.Drawing.Size(60, 21);
            this.miBuildProject.Text = "生成(&B)";
            // 
            // miBuild
            // 
            this.miBuild.Name = "miBuild";
            this.miBuild.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.miBuild.Size = new System.Drawing.Size(163, 22);
            this.miBuild.Text = "生成(&D)";
            this.miBuild.Click += new System.EventHandler(this.miBuild_Click);
            // 
            // miCancel
            // 
            this.miCancel.Enabled = false;
            this.miCancel.Name = "miCancel";
            this.miCancel.Size = new System.Drawing.Size(163, 22);
            this.miCancel.Text = "取消生成(&C)";
            this.miCancel.Click += new System.EventHandler(this.miCancel_Click);
            // 
            // miClearBuild
            // 
            this.miClearBuild.Name = "miClearBuild";
            this.miClearBuild.Size = new System.Drawing.Size(163, 22);
            this.miClearBuild.Text = "清理(&C)";
            this.miClearBuild.Click += new System.EventHandler(this.miClearBuild_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(160, 6);
            // 
            // miViewDoc
            // 
            this.miViewDoc.Name = "miViewDoc";
            this.miViewDoc.Size = new System.Drawing.Size(163, 22);
            this.miViewDoc.Text = "查看(&V)";
            this.miViewDoc.Click += new System.EventHandler(this.miViewDoc_Click);
            // 
            // miHelpMenu
            // 
            this.miHelpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miHelp,
            this.toolStripMenuItem4,
            this.miAbout});
            this.miHelpMenu.Name = "miHelpMenu";
            this.miHelpMenu.Size = new System.Drawing.Size(61, 21);
            this.miHelpMenu.Text = "帮助(&H)";
            // 
            // miHelp
            // 
            this.miHelp.Name = "miHelp";
            this.miHelp.Size = new System.Drawing.Size(186, 22);
            this.miHelp.Text = "帮助主页(&I)...";
            this.miHelp.Click += new System.EventHandler(this.miHelp_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(183, 6);
            // 
            // miAbout
            // 
            this.miAbout.Name = "miAbout";
            this.miAbout.Size = new System.Drawing.Size(186, 22);
            this.miAbout.Text = "关于 DocCreater(&A)";
            this.miAbout.Click += new System.EventHandler(this.miAbout_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2Collapsed = true;
            this.splitContainer1.Size = new System.Drawing.Size(626, 390);
            this.splitContainer1.SplitterDistance = 248;
            this.splitContainer1.TabIndex = 3;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.btnStart);
            this.splitContainer2.Panel1.Controls.Add(this.btnOptions);
            this.splitContainer2.Panel1.Controls.Add(this.btnMoveDown);
            this.splitContainer2.Panel1.Controls.Add(this.btnMoveUp);
            this.splitContainer2.Panel1.Controls.Add(this.btnRemove);
            this.splitContainer2.Panel1.Controls.Add(this.btnAddDirectory);
            this.splitContainer2.Panel1.Controls.Add(this.btnAddFile);
            this.splitContainer2.Panel1.Controls.Add(this.lbFiles);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.propertyGrid);
            this.splitContainer2.Panel2Collapsed = true;
            this.splitContainer2.Size = new System.Drawing.Size(626, 390);
            this.splitContainer2.SplitterDistance = 402;
            this.splitContainer2.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(516, 352);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(103, 23);
            this.btnStart.TabIndex = 15;
            this.btnStart.Text = "生成(&V)...";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.miBuild_Click);
            // 
            // btnOptions
            // 
            this.btnOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOptions.Location = new System.Drawing.Point(401, 352);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(104, 23);
            this.btnOptions.TabIndex = 16;
            this.btnOptions.Text = "属性(&O)";
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.miProperty_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveDown.Location = new System.Drawing.Point(515, 286);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(104, 23);
            this.btnMoveDown.TabIndex = 17;
            this.btnMoveDown.Text = "移下(&M)";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveUp.Location = new System.Drawing.Point(516, 257);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(104, 23);
            this.btnMoveUp.TabIndex = 18;
            this.btnMoveUp.Text = "移上(&U)";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Location = new System.Drawing.Point(515, 317);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(104, 23);
            this.btnRemove.TabIndex = 13;
            this.btnRemove.Text = "移除选中项(&R)";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.miRemove_Click);
            // 
            // btnAddDirectory
            // 
            this.btnAddDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddDirectory.Location = new System.Drawing.Point(516, 45);
            this.btnAddDirectory.Name = "btnAddDirectory";
            this.btnAddDirectory.Size = new System.Drawing.Size(104, 23);
            this.btnAddDirectory.TabIndex = 12;
            this.btnAddDirectory.Text = "添加目录(&D)...";
            this.btnAddDirectory.UseVisualStyleBackColor = true;
            this.btnAddDirectory.Click += new System.EventHandler(this.miAddDirectory_Click);
            // 
            // btnAddFile
            // 
            this.btnAddFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddFile.Location = new System.Drawing.Point(516, 16);
            this.btnAddFile.Name = "btnAddFile";
            this.btnAddFile.Size = new System.Drawing.Size(104, 23);
            this.btnAddFile.TabIndex = 14;
            this.btnAddFile.Text = "添加文件(&A)...";
            this.btnAddFile.UseVisualStyleBackColor = true;
            this.btnAddFile.Click += new System.EventHandler(this.miAddFile_Click);
            // 
            // lbFiles
            // 
            this.lbFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbFiles.ContextMenuStrip = this.cbFiles;
            this.lbFiles.Font = new System.Drawing.Font("微软雅黑", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbFiles.FormattingEnabled = true;
            this.lbFiles.ItemHeight = 20;
            this.lbFiles.Location = new System.Drawing.Point(12, 16);
            this.lbFiles.Name = "lbFiles";
            this.lbFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbFiles.Size = new System.Drawing.Size(497, 324);
            this.lbFiles.TabIndex = 11;
            this.lbFiles.DoubleClick += new System.EventHandler(this.lbFiles_DoubleClick);
            this.lbFiles.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbFiles_MouseDown);
            // 
            // cbFiles
            // 
            this.cbFiles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiAddFile,
            this.cmiAddDirectory,
            this.toolStripMenuItem7,
            this.cmiRemove,
            this.toolStripSeparator6,
            this.cmiSelectAll,
            this.cmiToggle,
            this.toolStripMenuItem8,
            this.cmiExplorer});
            this.cbFiles.Name = "contextMenuStrip1";
            this.cbFiles.Size = new System.Drawing.Size(246, 154);
            // 
            // cmiAddFile
            // 
            this.cmiAddFile.Name = "cmiAddFile";
            this.cmiAddFile.Size = new System.Drawing.Size(245, 22);
            this.cmiAddFile.Text = "添加文件(&A)...";
            this.cmiAddFile.Click += new System.EventHandler(this.miAddFile_Click);
            // 
            // cmiAddDirectory
            // 
            this.cmiAddDirectory.Name = "cmiAddDirectory";
            this.cmiAddDirectory.Size = new System.Drawing.Size(245, 22);
            this.cmiAddDirectory.Text = "添加目录(&D)...";
            this.cmiAddDirectory.Click += new System.EventHandler(this.miAddDirectory_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(242, 6);
            // 
            // cmiRemove
            // 
            this.cmiRemove.Name = "cmiRemove";
            this.cmiRemove.Size = new System.Drawing.Size(245, 22);
            this.cmiRemove.Text = "移除(&R)";
            this.cmiRemove.Click += new System.EventHandler(this.miRemove_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(242, 6);
            // 
            // cmiSelectAll
            // 
            this.cmiSelectAll.Name = "cmiSelectAll";
            this.cmiSelectAll.Size = new System.Drawing.Size(245, 22);
            this.cmiSelectAll.Text = "全选(&S)";
            this.cmiSelectAll.Click += new System.EventHandler(this.miSelectAll_Click);
            // 
            // cmiToggle
            // 
            this.cmiToggle.Name = "cmiToggle";
            this.cmiToggle.Size = new System.Drawing.Size(245, 22);
            this.cmiToggle.Text = "反选(&F)";
            this.cmiToggle.Click += new System.EventHandler(this.miToggle_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(242, 6);
            // 
            // cmiExplorer
            // 
            this.cmiExplorer.Name = "cmiExplorer";
            this.cmiExplorer.Size = new System.Drawing.Size(245, 22);
            this.cmiExplorer.Text = "在 windows 资源管理器打开(&E)";
            this.cmiExplorer.Click += new System.EventHandler(this.cmiExplorer_Click);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(96, 100);
            this.propertyGrid.TabIndex = 1;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(163, 22);
            this.toolStripMenuItem1.Text = "文档调试工具(&T)";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 437);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "DocPlus";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.cbFiles.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem miFile;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem miExit;
        private System.Windows.Forms.ToolStripMenuItem miProject;
        private System.Windows.Forms.ToolStripMenuItem miProperty;
        private System.Windows.Forms.ToolStripMenuItem miHelpMenu;
        private System.Windows.Forms.ToolStripMenuItem miHelp;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem miAbout;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAddDirectory;
        private System.Windows.Forms.Button btnAddFile;
        private System.Windows.Forms.ListBox lbFiles;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.ToolStripMenuItem miView;
        private System.Windows.Forms.ToolStripMenuItem miOutput;
        private System.Windows.Forms.ToolStripMenuItem miOptions;
        private System.Windows.Forms.ToolStripMenuItem miStatus;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.ContextMenuStrip cbFiles;
        private System.Windows.Forms.ToolStripMenuItem cmiRemove;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem cmiSelectAll;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem cmiExplorer;
        private System.Windows.Forms.ToolStripMenuItem cmiToggle;
        private System.Windows.Forms.ToolStripMenuItem miCreateProject;
        private System.Windows.Forms.ToolStripMenuItem miCloseProject;
        private System.Windows.Forms.ToolStripMenuItem miOpenProject;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem miAddFile;
        private System.Windows.Forms.ToolStripMenuItem miAddDirectory;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem miRemoveAll;
        private System.Windows.Forms.ToolStripMenuItem miToggle;
        private System.Windows.Forms.ToolStripMenuItem miRemove;
        private System.Windows.Forms.ToolStripMenuItem miSaveProject;
        private System.Windows.Forms.ToolStripMenuItem miSaveAsProject;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem miBuildProject;
        private System.Windows.Forms.ToolStripMenuItem miBuild;
        private System.Windows.Forms.ToolStripMenuItem miCancel;
        private System.Windows.Forms.ToolStripMenuItem miViewDoc;
        private System.Windows.Forms.ToolStripMenuItem miSelectAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem miClearBuild;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem miRecentProject;
        private System.Windows.Forms.ToolStripMenuItem 无记录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cmiAddFile;
        private System.Windows.Forms.ToolStripMenuItem cmiAddDirectory;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;

    }
}

