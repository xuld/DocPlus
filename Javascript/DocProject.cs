using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using DocPlus.Core;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace DocPlus.Javascript {

    /// <summary>
    /// 表示一个文档生成工程。
    /// </summary>
    [DefaultProperty("TargetPath")]
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

        #region 解析模板

        /// <summary>
        /// 获取或设置是否在生成成功后打开文件。
        /// </summary>
        [Category("软件")]
        [Description("是否在生成成功后在浏览中打开文档主页。")]
        [DefaultValue(true)]
        [DisplayName("生成完成后打开")]
        public bool OpenIfSuccess {
            get {
                return GetValue("OpenIfSuccess", true);
            }
            set {
                SetValue("OpenIfSuccess", value);
            }
        }

        /// <summary>
        /// 获取或设置输入文件的编码。
        /// </summary>
        [Category("软件")]
        [Description("软件会自动分析源文件编码，但如果自动分析结果不正确，您可以手动指定。")]
        [TypeConverter(typeof(EncodingConverter))]
        [DefaultValue(null)]
        [DisplayName("源文件编码")]
        public Encoding Encoding {
            get {
                return GetValue("Encoding", null);
            }
            set {
                SetValue("Encoding", value);
            }
        }

        /// <summary>
        /// 获取或设置生成网页使用的模板位置。
        /// </summary>
        [Category("软件")]
        [Description("生成网页使用的模板位置。")]
        [DefaultValue("template")]
        [DisplayName("模板文件夹")]
        [Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(UITypeEditor))]
        public string TemplateName {
            get {
                return GetValue("TemplateName") ?? "template";
            }
            set {
                SetValue("TemplateName", value);
            }
        }

        /// <summary>
        /// 生成前清空文件夹。
        /// </summary>
        [Category("软件")]
        [Description("定义是否在生成前清空目标文件夹。")]
        [DefaultValue(false)]
        [DisplayName("生成前清空文件夹")]
        public bool ClearBeforeRebuild {
            get {
                return GetValue("ClearBeforeRebuild", false);
            }
            set {
                SetValue("ClearBeforeRebuild", value);
            }
        }

        #endregion

        ///// <summary>
        ///// 获取或设置系统变量定义。
        ///// </summary>
        //[Category("系统变量")]
        //[Description("定义一些Javascript内置成员的文件。")]
        //[Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        //[DefaultValue("define.js")]
        //[DisplayName("预定义文件")]
        //public string SystemDefine {
        //    get;
        //    set;
        //}

      //  public bool ResolveUncommentedValue { get; set; }

      //  public bool EnableInternal { get; set; }

        ///// <summary>
        ///// 如果是静态类，自动标记全部成员都静态的。
        ///// </summary>
        //public bool AutoMarkClassAttribute { get; set; }

        ///// <summary>
        ///// 是否排除不合法ECMA命名规范的属性。
        ///// </summary>
        //public bool CheckIdentifierStart { get; set; }

        #region JavaCommentParser

        /// <summary>
        /// 自动根据参数名决定类型。
        /// </summary>
        [Category("解析")]
        [Description("允许系统根据参数名自动填充参数类型，如参数 strXXX 的类型为 String。")]
        [DefaultValue(true)]
        [DisplayName("自动猜测参数类型")]
        public bool AutoCreateParamComment {
            get {
                return GetValue("AutoCreateParamComment", true);
            }
            set {
                SetValue("AutoCreateParamComment", value);
            }
        }

        /// <summary>
        /// 当多行注释分行后，补充的字符。 默认是回车。
        /// </summary>
        [Category("解析")]
        [Description("允许系统根据参数名自动填充参数类型，如参数 strXXX 的类型为 String。")]
        [DefaultValue("\n")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DisplayName("多行注释分行后的换行符")]
        public string NewLine {
            get {
                return GetValue("NewLine") ?? "\n";
            }
            set {
                SetValue("NewLine", value);
            }
        }

        /// <summary>
        /// 获取或设置是否忽略语法错误。
        /// </summary>
        [Category("解析")]
        [Description("允许系统忽略语法错误并继续解析文档。")]
        [DefaultValue(false)]
        [DisplayName("忽略语法错误")]
        public bool SkipSyntaxError {
            get {
                return GetValue("SkipSyntaxError", false);
            }
            set {
                SetValue("SkipSyntaxError", value);
            }
        }

        #endregion

        #region DocAstVistor

        ///// <summary>
        ///// 获取或设置是否启用自动生成注释。
        ///// </summary>
        //public bool EnableAutoCreateComment {
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 获取或设置是否自动把类普通成员标记成 static 。
        ///// </summary>
        //public bool EnableAutoCreateStatic {
        //    get;
        //    set;
        //}

        /// <summary>
        /// 使用就近匹配原则搜索名字空间。DocPlus 默认使用变量域搜索名字空间算法，启用此项可兼容 jsdoc 文档。
        /// </summary>
        [Category("解析")]
        [Description("使用就近匹配原则搜索名字空间。DocPlus 默认使用变量域搜索名字空间算法，启用此项可兼容 jsdoc 文档。")]
        [DefaultValue(false)]
        [DisplayName("jsdoc 兼容模式")]
        public bool EnableGlobalNamespaceSetter {
            get {
                return GetValue("EnableGlobalNamespaceSetter", false);
            }
            set {
                SetValue("EnableGlobalNamespaceSetter", value);
            }
        }

        ///// <summary>
        ///// 在已经有名字空间的情况下，再次发现一个名字空间，是否将新的名字空间作为上个名字空间的子名字空间。
        ///// </summary>
        //public bool EnableMultiNamespace {
        //    get;
        //    set;
        //}

        /// <summary>
        /// 获取或设置是否自动识别命名规则。(如 _ 开头的变量自动识别成私有成员)
        /// </summary>
        [Category("解析")]
        [Description("自动识别命名规则。(如 _ 开头的变量自动识别成私有成员)。")]
        [DefaultValue(true)]
        [DisplayName("识别变量名")]
        public bool UseNamingRules {
            get {
                return GetValue("UseNamingRules", true);
            }
            set {
                SetValue("UseNamingRules", value);
            }
        }

        /// <summary>
        /// 是否根据函数定义自动生成参数。
        /// </summary>
        [Category("解析")]
        [Description("根据函数定义自动生成参数。")]
        [DefaultValue(true)]
        [DisplayName("识别函数定义")]
        public bool AutoCreateFunctionParam {
            get {
                return GetValue("AutoCreateFunctionParam", true);
            }
            set {
                SetValue("AutoCreateFunctionParam", value);
            }
        }

        /// <summary>
        /// 允许检测闭包成员，并自动隐藏闭包成员。
        /// </summary>
        [Category("解析")]
        [Description("允许检测闭包成员，并自动隐藏闭包成员。")]
        [DefaultValue(true)]
        [DisplayName("识别闭包成员")]
        public bool EnableClosure {
            get {
                return GetValue("EnableClosure", true);
            }
            set {
                SetValue("EnableClosure", value);
            }
        }

        /// <summary>
        /// 当将一个 JSON 对象赋值给父对象时，文档解析时自动将成员作为目标对象的成员。
        /// </summary>
        [Category("解析")]
        [Description("当将一个 JSON 对象赋值给父对象时，文档解析时自动将成员作为目标对象的成员。")]
        [DefaultValue(true)]
        [DisplayName("识别 JSON 对象")]
        public bool ResolveObjectSetter {
            get {
                return GetValue("ResolveObjectSetter", true);
            }
            set {
                SetValue("ResolveObjectSetter", value);
            }
        }

        /// <summary>
        /// 使用 Javascript 严格模式。
        /// </summary>
        [Category("解析")]
        [Description("使用 Javascript 严格模式（Strict Mode）。也可以在函数首行添加 \"use strict\" 通知解析器使用严格模式。")]
        [DefaultValue(false)]
        [DisplayName("使用 Javascript 严格模式")]
        public bool UseStrictMode {
            get {
                return GetValue("UseStrictMode", false);
            }
            set {
                SetValue("UseStrictMode", value);
            }
        }

        /// <summary>
        /// 默认继承对象。
        /// </summary>
        [Category("解析")]
        [Description("定义 Javascript 中类的默认基类。 如设置为 Object 。")]
        [DefaultValue(null)]
        [DisplayName("默认继承对象")]
        public string DefaultExtends {
            get {
                return GetValue("DefaultExtends");
            }
            set {
                SetValue("DefaultExtends", value);
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// 初始化 <see cref="DocPlus.Javascript.DocProject"/> 类的新实例。
        /// </summary>
        public DocProject() {
        }

        /// <summary>
        /// 解析当前文档的数据，并返回一个 <see cref="DocData"/> 实例。
        /// </summary>
        /// <returns></returns>
        public DocData Parse() {

            // 创建一个 DocParser 来解析文档。
            DocParser parser = new DocParser(this);

            // 使用 DocParser 来为添加的每个文件和文件夹单独解析。
            for(int i = 0; i < Items.Count; i++) {
                string name = Items[i];
                if(Directory.Exists(name)) {
                    foreach(string s in Directory.GetFiles(name, "*.js", SearchOption.AllDirectories)) {
                        parser.ParseFile(s, s.Substring(name.Length + 1));
                    }
                } else {
                    parser.ParseFile(name);
                }
            }

            // 返回解析之后得到的原始文档数据。
            return parser.End();

        }

        /// <summary>
        /// 开始编译整个文档。
        /// </summary>
        public override void Build() {

            ProgressReporter.Write("***启动生成***");
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

            watch.Start();

            // 首先获取原始的文档数据。
            DocData data = Parse();

            // 然后使用 DocGenerator 进行最终文件生成。
            new DocGenerator(this).Generate(data);

            ProgressReporter.Write("***生成完成***");

            watch.Stop();

            ProgressReporter.Write("共 " + Items.Count + " 项 用时: " + watch.Elapsed.TotalSeconds + "秒");
            ProgressReporter.Write(TargetPath);

            if(OpenIfSuccess) {
                try {
                    System.Diagnostics.Process.Start(TargetPath + "/index.html");
                } catch {

                }
            }
        }
    }

}
