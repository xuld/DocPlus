using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DocPlus.Core;
using System.ComponentModel;
using CorePlus.Api;
using System.Windows.Forms.Design;
using System.Drawing.Design;

namespace DocPlus.Javascript {

    /// <summary>
    /// 表示一个文档生成工程。
    /// </summary>
    public class DocProject : DocProjectBase, IDocProject {

        #region 工程性质

        /// <summary>
        /// 获取当前项目的名字。
        /// </summary>
        [Browsable(false)]
        public override string DisplayName {
            get {
                return "Javascript 文档";
            }
        }

        /// <summary>
        /// 获取当前项目支持的默认扩展名。
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        public override string DefaultExt {
            get {
                return ".js";
            }
        }

        /// <summary>
        /// 获取当前项目支持的文件的过滤器。
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        public override string FileFilter {
            get {
                return "Javascript 文件(*.js)|*.js|全部文件(*.*)|*.*";
            }
        }

        #endregion

        #region 解析配置

        /// <summary>
        /// 获取或设置系统变量定义。
        /// </summary>
        [Category("系统变量")]
        [Description("定义一些Javascript内置成员的文件。")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        [DefaultValue("define.js")]
        [DisplayName("预定义文件")]
        public string SystemDefine {
            get;
            set;
        }

        public bool ResolveUncommentedValue { get; set; }

        public bool EnableInternal { get; set; }

        /// <summary>
        /// 如果是静态类，自动标记全部成员都静态的。
        /// </summary>
        public bool AutoMarkClassAttribute { get; set; }

        /// <summary>
        /// 是否排除不合法ECMA命名规范的属性。
        /// </summary>
        public bool CheckIdentifierStart { get; set; }

        #region JavaCommentParser

        /// <summary>
        /// 自动根据参数名决定类型。
        /// </summary>
        public bool AutoCreateParamComment {
            get;
            set;
        }

        /// <summary>
        /// 当多行注释分行后，补充的字符。 默认是回车。
        /// </summary>
        public string NewLine {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置是否忽略语法错误。
        /// </summary>
        public bool SkipSyntaxError {
            get;
            set;
        }

        internal string CurrentFile {
            get {
                return CurrentSource.Path;
            }
        }

        internal Source CurrentSource {
            get;
            set;
        }

        #endregion

        #region DocAstVistor

        /// <summary>
        /// 获取或设置是否启用自动生成注释。
        /// </summary>
        public bool EnableAutoCreateComment {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置是否自动把类普通成员标记成 static 。
        /// </summary>
        public bool EnableAutoCreateStatic {
            get;
            set;
        }

        /// <summary>
        /// 使用就近匹配原则搜索名字空间。DocPlus 默认使用变量域搜索名字空间算法，启用此项可兼容 jsdoc 文档。
        /// </summary>
        public bool EnableGlobalNamespaceSetter {
            get;
            set;
        }

        /// <summary>
        /// 在已经有名字空间的情况下，再次发现一个名字空间，是否将新的名字空间作为上个名字空间的子名字空间。
        /// </summary>
        public bool EnableMultiNamespace {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置是否自动识别命名规则。(如 _ 开头的变量自动识别成私有成员)
        /// </summary>
        public bool UseNamingRules {
            get;
            set;
        }

        /// <summary>
        /// 是否自动根据函数定义生成参数。
        /// </summary>
        public bool AutoCreateFunctionParam {
            get;
            set;
        }

        /// <summary>
        /// 允许检测闭包成员，并自动隐藏闭包成员。
        /// </summary>
        public bool EnableClosure {
            get;
            set;
        }

        #endregion

        public bool UseStrictMode { get; set; }

        #endregion

        public DocProject() {
            NewLine = Environment.NewLine;
            EnableClosure = AutoCreateFunctionParam = EnableAutoCreateComment = UseNamingRules = true;
        }

        /// <summary>
        /// 开始编译整个文档。
        /// </summary>
        public override void Build() {

            ApiDoc api = new DocParser(this).Build();

            api.Save(TargetPath);

                //IDocParser parser = GetDocParser();

                //_parser.Reset();

                //_parser.Settings = _cfg;

                //_parser.ProgressReporter.Clear();

                //_parser.ProgressReporter.Write("启用生成 >> " + _cfg.SavePath);

                //try {

                //    _parser.NewDocument();

                //    if (!String.IsNullOrEmpty(_cfg.SystemDefine)) {
                //        string p = FileHelper.GetFullPath(_cfg.SystemDefine);
                //        if (FileHelper.ExistsFile(p))
                //            ParseFile(p);
                //    }

                //    foreach (string file in lbFiles.Items) {
                //        ParseFile(file);
                //    }


                //    _parser.ProgressReporter.Write("正在合成...");

                //    _parser.SaveDocument();

                //    _parser.ProgressReporter.Write("正在保存...");

                //    _parser.Document.Save(_cfg.SavePath, _cfg.OutputEncoding ?? Encoding.Default);

                //    _parser.ProgressReporter.Success("生成完成 >> " + _cfg.SavePath);

                //} catch (Exception e) {

                //    SaveState();
                //    OnError(e);

                //    return;
                //}

                //View();

                //SaveState();
        }

    }

}
