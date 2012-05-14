using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.IO;
using System.Collections.Specialized;
using System.Xml;

namespace DocPlus.Core {

    /// <summary>
    /// 表示一个文档生成工程。
    /// </summary>
    [DefaultProperty("TargetPath")]
    public abstract class DocProjectBase : IDocProject {

        #region 工程性质

        /// <summary>
        /// 获取当前项目的名字。
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        public abstract string DisplayName {
            get;
        }

        /// <summary>
        /// 获取当前项目支持的默认扩展名。
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        public abstract string DefaultExt {
            get;
        }

        /// <summary>
        /// 获取当前项目支持的文件的过滤器。
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        public abstract string FileFilter {
            get;
        }

        #endregion

        #region 生成准备

        /// <summary>
        /// 获取绑定到项目的文件列表。
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        public List<string> Items {
            get;
            private set;
        }

        /// <summary>
        /// 获取或设置用于记录日志的工具。
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        public IProgressReporter ProgressReporter {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置保存的位置。
        /// </summary>
        [Category("软件")]
        [Description("生成的文件默认保存的位置。")]
        [DefaultValue(null)]
        [DisplayName("保存位置")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string TargetPath {
            get;
            set;
        }

        #endregion

        #region 生成操作

        /// <summary>
        /// 初始化 <see cref="DocPlus.Core.DocProjectBase"/> 类的新实例。
        /// </summary>
        protected DocProjectBase() {
            Items = new List<string>();
            ProgressReporter = ConsoleProgressReporter.Instance;
            OpenIfSuccess = true;
        }

        /// <summary>
        /// 载入指定路径的项目文件。
        /// </summary>
        /// <param name="path">目标文件的位置。</param>
        public virtual void Load(string path) {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);

            XmlNode itemGroup = xml.SelectSingleNode("Project/ItemGroup");

            foreach(XmlNode s in itemGroup.ChildNodes) {
                Items.Add(s.InnerText);
            }

            XmlNode propertyGroup = xml.SelectSingleNode("Project/PropertyGroup");

            foreach(XmlElement s in itemGroup.ChildNodes) {
                _vals[s.Name] = s.InnerText;
            }





        }

        /// <summary>
        /// 保存项目到指定路径。
        /// </summary>
        /// <param name="path">目标文件的位置。</param>
        public virtual void Save(string path) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""1.0"" DisplayName=""" + DisplayName + @""">
  <ItemGroup/>
  <PropertyGroup/>
</Project>");

            XmlNode itemGroup = xml.SelectSingleNode("Project/ItemGroup");

            foreach(string s in Items) {
                XmlNode t = xml.CreateElement("Item");
                t.InnerText = s;
                itemGroup.AppendChild(t);
            }

            XmlNode propertyGroup = xml.SelectSingleNode("Project/PropertyGroup");

            foreach(string s in _vals) {
                XmlNode t = xml.CreateElement(s);
                t.InnerText = _vals[s];
                itemGroup.AppendChild(t);
            }


            xml.Save(path);
        }

        /// <summary>
        /// 关闭项目。
        /// </summary>
        public virtual void Close() {

        }

        /// <summary>
        /// 开始编译整个文档。
        /// </summary>
        public abstract void Build();

        /// <summary>
        /// 清理缓存文件。
        /// </summary>
        public virtual void ClearBuild() {
            Directory.Delete(TargetPath, true);
        }

        #endregion

        #region 配置底层

        NameValueCollection _vals = new NameValueCollection();

        static int ParseInt(string s) {
            int r;
            return int.TryParse(s, out r) ? r : 0;
        }

        protected string GetValue(string key) {
            return _vals[key];
        }

        protected void SetValue(string key, string value) {
            _vals[key] = value;
        }

        protected bool GetValue(string key, bool dftValue) {
            string s = _vals[key];
            return s == null ? dftValue : s == "true";
        }

        protected void SetValue(string key, bool value) {
            _vals[key] = value ? "true" : "false";
        }

        protected Encoding GetValue(string key, Encoding dftValue) {
            string s = _vals[key];
            return s == null ? dftValue : Encoding.GetEncoding(ParseInt(s));
        }

        protected void SetValue(string key, Encoding value) {
            _vals[key] = value.CodePage.ToString();
        }

        #endregion

        #region 配置

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

        /// <summary>
        /// 获取或设置输入文件的编码。
        /// </summary>
        [Category("软件")]
        [Description("软件会自动分析文件编码，但如果自动分析结果不正确，您可以手动指定。")]
        [TypeConverter(typeof(EncodingConverter))]
        [DefaultValue(null)]
        [DisplayName("文档编码")]
        public Encoding OutputEncoding { get; set; }

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

    }

}
