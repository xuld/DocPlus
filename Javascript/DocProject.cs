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
        [Description("是否在生成成功后打开。")]
        [DefaultValue(true)]
        [DisplayName("生成后打开")]
        public bool OpenIfSuccess { get; set; }

        /// <summary>
        /// 获取或设置输入文件的编码。
        /// </summary>
        [Category("软件")]
        [Description("软件会自动分析文件编码，但如果自动分析结果不正确，您可以手动指定。")]
        [TypeConverter(typeof(EncodingConverter))]
        [DefaultValue(null)]
        [DisplayName("编码")]
        public Encoding Encoding { get; set; }

        [Description("生成网页使用的模板位置。")]
        [DefaultValue("templates")]
        [DisplayName("文件夹")]
        [Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(UITypeEditor))]
        [Category("网页")]
        public string TemplateName {
            get;
            set;
        }

        #endregion

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

        /// <summary>
        /// 当将一个 JSON 对象赋值给父对象时，文档解析时自动将成员作为目标对象的成员。
        /// </summary>
        public bool ResolveObjectSetter { get; set; }

        #endregion

        /// <summary>
        /// 初始化 <see cref="DocPlus.Javascript.DocProject"/> 类的新实例。
        /// </summary>
        public DocProject() {
            NewLine = Environment.NewLine;
            EnableClosure = AutoCreateFunctionParam = EnableAutoCreateComment = UseNamingRules = true;
            OpenIfSuccess = true;
            ResolveObjectSetter = true;
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
                        parser.ParseFile(s.Substring(name.Length), s);
                    }
                } else {
                    parser.ParseFile(name);
                }
            }

            // 返回解析之后得到的原始文档数据。
            return parser.Data;

        }

        /// <summary>
        /// 开始编译整个文档。
        /// </summary>
        public override void Build() {

            // 首先获取原始的文档数据。
            DocData data = Parse();

            // 然后使用 DocGenerator 进行最终文件生成。
            new DocGenerator(this).Generate(data);
        }
    }

}
