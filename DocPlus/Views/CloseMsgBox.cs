using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DocPlus.GUI {

    /// <summary>
    /// 另存为的对话框。
    /// </summary>
    public partial class CloseMsgBox :Form {

        /// <summary>
        /// 初始化 <see cref="Xuld.MyNotePad.CloseMsgBox"/> 的新实例。
        /// </summary>
        /// <param name="name">名字。</param>
        public CloseMsgBox(string text, string name) {
            InitializeComponent();
            Text = text;
            txtLabel.Text = name;
            StartPosition = FormStartPosition.CenterParent;
        }

        private void btnYes_Click(object sender, EventArgs e) {
            DialogResult = System.Windows.Forms.DialogResult.Yes;
        }

        private void btnNo_Click(object sender, EventArgs e) {
            DialogResult = System.Windows.Forms.DialogResult.No;
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
