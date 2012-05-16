using System.Collections.Generic;
using System.Collections.Specialized;
using CorePlus.Parser.Javascript;
using System.Text;

namespace DocPlus.Javascript {

    /// <summary>
    /// 表示一个文档注释变量。
    /// </summary>
    public class DocComment : NameObjectCollectionBase {

        #region 注释自身属性

        /// <summary>
        /// 获取或设置变量在源码定义的位置。
        /// </summary>
        public Location StartLocation {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置变量在源码定义的位置。
        /// </summary>
        public Location EndLocation {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置定义当前变量的源文件。
        /// </summary>
        public string Source {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前变量的来源。
        /// </summary>
        public DocCommentAttribute Attribute {
            get;
            set;
        }

        #endregion

        #region 系统函数

        /// <summary>
        /// 获取或设置指定标签对应的原始数据。
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public object this[string nodeName] {
            get {
                return BaseGet(nodeName);
            }
            set {
                BaseSet(nodeName, value);
            }
        }

        /// <summary>
        /// 获取指定节点对应的值。
        /// </summary>
        /// <param name="nodeName">节点名。</param>
        /// <returns>对应的值。</returns>
        public string GetComment(string nodeName) {
            return (string)BaseGet(nodeName);
        }

        /// <summary>
        /// 设置指定节点对应的值。
        /// </summary>
        /// <param name="nodeName">节点名。</param>
        /// <returns>对应的值。</returns>
        public void SetComment(string nodeName, string value) {
            BaseSet(nodeName, value);
        }

        /// <summary>
        /// 自动填充指定的注释。
        /// </summary>
        /// <param name="nodeName">节点名。</param>
        /// <param name="value">值。</param>
        public void AutoFill(string nodeName, object value) {
            if(BaseGet(nodeName) == null) {
                BaseSet(nodeName, value);
            }
        }

        /// <summary>
        /// 初始化 <see cref="DocPlus.Javascript.DocComment"/> 类的新实例。
        /// </summary>
        public DocComment() {

        }

        /// <summary>
        /// 初始化 <see cref="DocPlus.Javascript.DocComment"/> 类的新实例。
        /// </summary>
        public DocComment(Location startLocation, Location endLocation) {
            StartLocation = startLocation;
            EndLocation = endLocation;
        }

        /// <summary>
        /// 将指定变量的注释合并到当前变量。
        /// </summary>
        /// <param name="src"></param>
        public void Merge(DocComment src) {
            for(int i = 0; i < src.Count; i++) {
                BaseSet(src.BaseGetKey(i), src.BaseGet(i));
            }
        }

        #endregion

        #region Node2Property

        /// <summary>
        /// 获取或设置注释节点名。
        /// </summary>
        public string Name {
            get {
                return (string)this[NodeNames.Name];
            }
            set {
                this[NodeNames.Name] = value;
            }
        }

        /// <summary>
        /// 获取或设置注释节点名。
        /// </summary>
        public string Type {
            get {
                return (string)this[NodeNames.Type];
            }
            set {
                this[NodeNames.Type] = value;
            }
        }

        /// <summary>
        /// 获取或设置注释节点名。
        /// </summary>
        public object Value {
            get {
                return this[NodeNames.Value];
            }
            set {
                this[NodeNames.Value] = value;
            }
        }

        #endregion

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// 	<see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            foreach (string key in this) {
                sb.Append('@');
                sb.Append(key);
                sb.Append(' ');
                sb.Append(this[key]);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string MemberOf {
            get {
                return (string)this[NodeNames.MemberOf];
            }
            set {
                this[NodeNames.MemberOf] = value;
            }
        }

        public string MemberType {
            get {
                return (string)this[NodeNames.MemberType];
            }
            set {
                this[NodeNames.MemberType] = value;
            }
        }

        public string FullName {
            get {
                return MemberOf == null ? Name : (MemberOf + "." + Name);
            }
        }

        public bool IsStatic {
            get {
                if(this[NodeNames.MemberAccess] == null)
                    return false;

                if(((string)this[NodeNames.MemberAccess]).Contains("static"))
                    return true;


                return false;
            }
        }

        public string NamespaceSetter { get; set; }

        public DocComment Parent { get; set; }

        public bool Ignore { get; set; }
    }

}
