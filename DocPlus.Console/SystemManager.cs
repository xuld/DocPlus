using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DocPlus.Core;

namespace DocPlus.Console {

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

            foreach(string file in Directory.GetFiles(folder, fileNameFilter, System.IO.SearchOption.TopDirectoryOnly)) {
                IDocProject dr = CreateProject(file);
                if(dr != null)
                    try {
                        DocProjects.Add(new KeyValuePair<string, string>(dr.DisplayName, file));
                    } catch {

                    }
            }
        }

        public static IDocProject CreateProject(string dllFile) {
            try {
                Assembly a = Assembly.LoadFrom(dllFile);
                string fileName = Path.GetFileNameWithoutExtension(dllFile);
                System.Type cls = a.GetType(fileName + ".DocProject", true);
                if(typeof(IDocProject).IsAssignableFrom(cls)) {
                    return (IDocProject)Activator.CreateInstance(cls);
                }
            } catch(System.Exception e) {
                System.Console.Error.WriteLine("加载 '" + dllFile + "' 时出现异常: \r\n\t" + e.Message);
            }

            return null;
        }

        public static IDocProject OpenProject(string path) {
            foreach(var file in DocProjects) {
                IDocProject dr = CreateProject(file.Value);
                if(dr != null) {
                    try {
                        dr.Load(path);
                        return dr;
                    } catch {

                    }
                }
            }

            System.Console.Error.WriteLine("无法打开项目文件 " + path);
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

        /// <summary>
        /// 初始化整个系统，显示窗口，并执行消息循环。
        /// </summary>
        internal static void Run(string[] args) {
            SearchDocRenders(Path.GetDirectoryName(typeof(SystemManager).Assembly.Location));

            if(args.Length < 1) {
                ShowUsage();
                return;
            }


            string proj = args[0];
            BuildProject(OpenProject(proj));




        }

        static void ShowUsage() {
            System.Console.WriteLine("usage: DocPlus.Console.exe myproj.docproj");
        }

        #endregion


    }

}
