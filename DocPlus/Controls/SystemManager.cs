using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.IO;
using DocPlus.Core;

namespace DocPlus.GUI {

    /// <summary>
    /// 管理整个系统事务的类。
    /// </summary>
    public static class SystemManager {

        #region 程序

        /// <summary>
        /// 所有可以使用的绑定器。
        /// </summary>
        public static List<KeyValuePair<string, string>> DocProjects {
            get;
            private set;
        }

        /// <summary>
        /// 在指定的文件夹搜索包含 <see cref="IDocRender"/> 实例的托管 DLL 文件。如果搜索成功，则创建实现了 <see cref="IDocRender"/> 接口的类的实例并添加到 DocReaders 属性。
        /// </summary>
        /// <param name="folder">指定要搜索的文件夹，不搜索子目录。</param>
        /// <param name="fileNameFilter">搜索的文件名，运行使用*通配符。</param>
        public static void SearchDocRenders(string folder, string fileNameFilter = "DocPlus.*.dll") {
            DocProjects = new List<KeyValuePair<string, string>>();

            foreach (string file in Directory.GetFiles(folder, fileNameFilter, System.IO.SearchOption.TopDirectoryOnly)) {
                IDocProject dr = CreateProject(file);
                if (dr != null)
                    try {
                        DocProjects.Add(new KeyValuePair<string, string>(dr.DisplayName, file));
                    } catch {

                    }
            }
        }

        /// <summary>
        /// 初始化整个系统，显示窗口，并执行消息循环。
        /// </summary>
        internal static void Run(string[] args) {
            SearchDocRenders(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath));
            if (args.Length > 0)
                AddRecentFile(args[0]);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        public static IDocProject CreateProject(string dllFile) {
            try {
                Assembly a = Assembly.LoadFrom(dllFile);
                string fileName = Path.GetFileNameWithoutExtension(dllFile);
                System.Type cls = a.GetType(fileName + ".DocProject", true);
                if (typeof(IDocProject).IsAssignableFrom(cls)) {
                    return (IDocProject)Activator.CreateInstance(cls);
                }
            } catch (System.Exception e) {
                ShowError("加载 '" + dllFile + "' 时出现异常: \r\n\t" + e.Message);
            }

            return null;
        }

        public static IDocProject OpenProject(string path) {
            foreach (var file in DocProjects) {
                IDocProject dr = CreateProject(file.Value);
                if (dr != null) {
                    try {
                        dr.Load(path);
                        return dr;
                    } catch{

                    }
                }
            }

            ShowError("无法打开项目文件 " + path);
            return null;
        }

        /// <summary>
        /// 开始编译项目。
        /// </summary>
        /// <param name="project">要生成的项目。</param>
        public static void BuildProject(IDocProject project) {
            project.ProgressReporter.Start();
            project.Build();
            project.ProgressReporter.Complete();
        }

        #endregion

        #region 对话框

        internal const string Title = "DocPlus";

        /// <summary>
        /// 使用对话框向用户显示一个错误信息, 并循环用户是否重试。
        /// </summary>
        /// <param name="message">要显示的错误信息。</param>
        public static void ShowError(string message) {
            MessageBox.Show(message, Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 使用对话框向用户显示一个错误信息, 并循环用户是否重试。
        /// </summary>
        /// <param name="message">要显示的错误信息。</param>
        public static void ShowAlert(string message) {
            MessageBox.Show(message, Title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// 使用对话框向用户显示一个错误信息, 并循环用户是否重试。
        /// </summary>
        /// <param name="message">要显示的错误信息。</param>
        public static bool ShowErrorAndRetry(string message) {
            return MessageBox.Show(message, Title, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry;
        }

        #endregion

        #region 历史文件

        /// <summary>
        /// 最大历史记录的数目。
        /// </summary>
        const int MaxFileHistoryCount = 10;

        public static void AddRecentFile(string fileName) {

            if (!RecentFiles.Contains(fileName))
                Properties.Settings.Default.RecentFiles.Add(fileName);

            if (Properties.Settings.Default.RecentFiles.Count > MaxFileHistoryCount) {
                Properties.Settings.Default.RecentFiles.RemoveAt(0);
            }

        }

        /// <summary>
        /// 获取用户最近打开的文件列表。
        /// </summary>
        public static StringCollection RecentFiles {
            get {
                if (Properties.Settings.Default.RecentFiles == null)
                    Properties.Settings.Default.RecentFiles = new StringCollection();
                return Properties.Settings.Default.RecentFiles;
            }
        }

        /// <summary>
        /// 获取用户最后一次打开的文件。
        /// </summary>
        public static string RecentProject {
            get {
                if (Properties.Settings.Default.RecentFiles == null)
                    return null;
                return (string)Properties.Settings.Default.RecentFiles[Properties.Settings.Default.RecentFiles.Count - 1];
            }
        }

        #endregion


    }

}
