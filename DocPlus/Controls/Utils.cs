using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace DocPlus.GUI {

    /// <summary>
    /// 提供常用工具辅助。
    /// </summary>
    public static class Utils {

        /// <summary>
        /// 在资源管理器浏览文件。
        /// </summary>
        /// <param name="path"></param>
        public static void Explore(string path) {
            Process.Start("explorer", "/select," + path);
        }

        /// <summary>
        /// 打开指定资源。
        /// </summary>
        /// <param name="path"></param>
        public static void ShellExecute(string path) {
            try {
                Process.Start(path);
            } catch {

            }
        }

        /// <summary>
        /// 打开指定资源。
        /// </summary>
        /// <param name="path"></param>
        public static void ShellExecute(string path, string arguments) {
            try {
                Process.Start(path, arguments);
            } catch {

            }
        }
    }
}
