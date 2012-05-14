using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace DocPlus.GUI {
    /// <summary>
    /// TraceWindow is a class that will connect to trace events and display trace outputs in a text box
    /// </summary>
    public class TraceWindowControl : System.Windows.Forms.UserControl {
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label captionLabel;
        private System.Windows.Forms.ToolTip toolTip1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem 复制CToolStripMenuItem;
        private ToolStripMenuItem 清除TToolStripMenuItem;
        private ToolStripMenuItem 导出到文件LToolStripMenuItem;
        private System.ComponentModel.IContainer components;

        /// <summary>
        /// Creates a new instance of the TraceWindowControl class
        /// </summary>
        public TraceWindowControl() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        /// <summary>
        /// Gets/Set the window caption
        /// </summary>
        [Category("外观")]
        [Browsable(true)]
        public override string Text {
            get { return captionLabel.Text; }
            set { captionLabel.Text = value; }
        }

        /// <summary>
        /// Gets/Sets the test displayed in the trace window
        /// </summary>
        public string TraceText {
            get { return richTextBox1.Text; }
            set { richTextBox1.Text = value; }
        }

        private bool _AutoConnect = false;

        /// <summary>
        /// Determines whether the control will connect to trace events when it becomes visible, and disconnect when it is hidden
        /// </summary>
        [Category("Behavior")]
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Determines whether the control will connect to trace events when it becomes visible, and disconnect when it is hidden")]
        public bool AutoConnect {
            get { return _AutoConnect; }
            set { _AutoConnect = value; }
        }

        /// <summary>
        /// Clears the contents of the window
        /// </summary>
        public void Clear() {

            if(richTextBox1.InvokeRequired)
                richTextBox1.BeginInvoke(new Action(richTextBox1.Clear));
            else
                this.richTextBox1.Clear();
        }

        #region Disposer
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if(disposing) {
                Disconnect();

                if(components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.captionLabel = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.复制CToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.清除TToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导出到文件LToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.CausesValidation = false;
            this.richTextBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.richTextBox1.DetectUrls = false;
            this.richTextBox1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.HideSelection = false;
            this.richTextBox1.Location = new System.Drawing.Point(0, 16);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(418, 209);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.richTextBox1.WordWrap = false;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.BackColor = System.Drawing.SystemColors.Control;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.Location = new System.Drawing.Point(402, 1);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(14, 14);
            this.closeButton.TabIndex = 1;
            this.closeButton.TabStop = false;
            this.closeButton.Text = "x";
            this.toolTip1.SetToolTip(this.closeButton, "Close");
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            this.closeButton.MouseLeave += new System.EventHandler(this.closeButton_MouseLeave);
            // 
            // captionLabel
            // 
            this.captionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.captionLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.captionLabel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.captionLabel.Location = new System.Drawing.Point(5, 0);
            this.captionLabel.Name = "captionLabel";
            this.captionLabel.Size = new System.Drawing.Size(413, 16);
            this.captionLabel.TabIndex = 2;
            this.captionLabel.Text = "输出";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.复制CToolStripMenuItem,
            this.清除TToolStripMenuItem,
            this.导出到文件LToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(151, 70);
            // 
            // 复制CToolStripMenuItem
            // 
            this.复制CToolStripMenuItem.Name = "复制CToolStripMenuItem";
            this.复制CToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.复制CToolStripMenuItem.Text = "复制(&C)";
            this.复制CToolStripMenuItem.Click += new System.EventHandler(this.复制CToolStripMenuItem_Click);
            // 
            // 清除TToolStripMenuItem
            // 
            this.清除TToolStripMenuItem.Name = "清除TToolStripMenuItem";
            this.清除TToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.清除TToolStripMenuItem.Text = "清除(&T)";
            this.清除TToolStripMenuItem.Click += new System.EventHandler(this.清除TToolStripMenuItem_Click);
            // 
            // 导出到文件LToolStripMenuItem
            // 
            this.导出到文件LToolStripMenuItem.Name = "导出到文件LToolStripMenuItem";
            this.导出到文件LToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.导出到文件LToolStripMenuItem.Text = "导出到文件(&L)";
            this.导出到文件LToolStripMenuItem.Click += new System.EventHandler(this.导出到文件LToolStripMenuItem_Click);
            // 
            // TraceWindowControl
            // 
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.captionLabel);
            this.Name = "TraceWindowControl";
            this.Size = new System.Drawing.Size(418, 225);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Raises the VisibleChanged event
        /// </summary>
        /// <param name="e">event arguments</param>
        protected override void OnVisibleChanged(EventArgs e) {
            if(AutoConnect) {
                if(this.Visible)
                    Connect();
                else
                    Disconnect();
            }
            base.OnVisibleChanged(e);
        }

        private TextWriterTraceListener listener = null;

        /// <summary>
        /// Connects the control to trace events
        /// </summary>
        public void Connect() {
            listener = new TextWriterTraceListener(new TextBoxWriter(this.richTextBox1));

            System.Diagnostics.Trace.Listeners.Add(listener);
        }


        private delegate void AppendTextDelegate(string message);

        public void Trace(string message) {
            if(richTextBox1.InvokeRequired) {
                richTextBox1.BeginInvoke(new AppendTextDelegate(richTextBox1.AppendText), new object[] { message + Environment.NewLine });
            } else {
                richTextBox1.AppendText(message);
                richTextBox1.AppendText(Environment.NewLine);
            }
        }

        /// <summary>
        /// Disconnects the control from trace events
        /// </summary>
        public void Disconnect() {
            if(listener != null) {
                System.Diagnostics.Trace.Listeners.Remove(listener);
                listener.Flush();
                listener.Close();
                listener = null;
            }
        }

        /// <summary>
        /// Raised when the close button is clicked
        /// </summary>
        public event EventHandler CloseClick;

        /// <summary>
        /// Raises the <see cref="CloseClick"/> event
        /// </summary>
        protected virtual void OnCloseClick() {
            if(CloseClick != null)
                CloseClick(this, EventArgs.Empty);
        }

        private void closeButton_Click(object sender, System.EventArgs e) {
            this.Visible = false;
            OnCloseClick();
        }

        private void closeButton_MouseLeave(object sender, System.EventArgs e) {
            if(this.Focused)
                this.Parent.Focus();
        }

        private void 复制CToolStripMenuItem_Click(object sender, EventArgs e) {

            if(richTextBox1.SelectionLength == 0)
                richTextBox1.SelectAll();
            richTextBox1.Copy();
        }

        private void 清除TToolStripMenuItem_Click(object sender, EventArgs e) {
            richTextBox1.Clear();
        }

        private void 导出到文件LToolStripMenuItem_Click(object sender, EventArgs e) {

            SaveFileDialog sfg = new SaveFileDialog();
            sfg.Filter = "日志文件(*.log)|*.log";
            sfg.FileName = "输出";

            if(sfg.ShowDialog() == DialogResult.OK) {
                if(richTextBox1.SelectionLength == 0)
                    richTextBox1.SelectAll();
                File.WriteAllText(sfg.FileName, richTextBox1.SelectedText);
            }
        }
    }
}
