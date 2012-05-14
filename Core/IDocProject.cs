using System.Collections.Generic;

namespace DocPlus.Core {

    /// <summary>
    /// 表示一个文档生成工程。
    /// </summary>
    public interface IDocProject {

        #region 工程性质

        /// <summary>
        /// 获取当前项目的显示名。
        /// </summary>
        string DisplayName {
            get;
        }

        /// <summary>
        /// 获取当前项目支持的默认扩展名。
        /// </summary>
        string DefaultExt {
            get;
        }

        /// <summary>
        /// 获取当前项目支持的文件扩展名，用|隔开。
        /// </summary>
        string FileFilter {
            get;
        }

        #endregion

        #region 生成准备

        /// <summary>
        /// 获取绑定到项目的文件列表。
        /// </summary>
        List<string> Items {
            get;
        }

        /// <summary>
        /// 获取或设置用于输出生成进度的工具。
        /// </summary>
        IProgressReporter ProgressReporter {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置生成的目标文件夹。
        /// </summary>
        string TargetPath {
            get;
            set;
        }

        #endregion

        #region 生成操作

        /// <summary>
        /// 载入指定路径的项目文件。
        /// </summary>
        /// <param name="path">目标文件的位置。</param>
        void Load(string path);

        /// <summary>
        /// 保存项目到指定路径。
        /// </summary>
        /// <param name="path">目标文件的位置。</param>
        void Save(string path);

        /// <summary>
        /// 关闭项目。
        /// </summary>
        void Close();

        /// <summary>
        /// 开始编译整个文档。
        /// </summary>
        void Build();

        /// <summary>
        /// 清理编译产生的临时文件。
        /// </summary>
        void ClearBuild();

        #endregion

    }

}
