using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace DocPlus.Javascript {

    /// <summary>
    /// 表示一个标签。
    /// </summary>
    public sealed class CommentNode {

        #region 动态

        byte _bitFlags;

        bool GetFlag(byte bit) {
            return (_bitFlags & bit) == bit;
        }

        /// <summary>
        /// 获取标签的名字。
        /// </summary>
        public string Name {
            get;
            private set;
        }

        /// <summary>
        /// 获取或设置一个标签是否可重定义。
        /// </summary>
        public bool CanRedefine {
            get {
                return GetFlag(1);
            }
        }

        /// <summary>
        /// 获取或设置一个标签是否可多行。
        /// </summary>
        public bool Inlines {
            get {
                return GetFlag(2);
            }
        }

        /// <summary>
        /// 获取或设置一个标签是否可空参数。
        /// </summary>
        public bool AllowEmpty {
            get {
                return GetFlag(4);
            }
        }

        /// <summary>
        /// 获取或设置一个标签是否必须无参数。
        /// </summary>
        public bool MustBeEmpty {
            get {
                return GetFlag(8);
            }
        }

        /// <summary>
        /// 初始化 <see cref="DocPlus.DocParser.Javascript.CommentNode"/> 的新实例。
        /// </summary>
        /// <param name="name">标签的名字。</param>
        /// <param name="canRedefine">是否可重定义。</param>
        /// <param name="inlines">是否可多行。</param>
        /// <param name="allowEmpty">是否可空参数。</param>
        /// <param name="mustBeEmpty">是否必须无参数。</param>
        /// <param name="isSystemDefined">是否是系统内置的。</param>
        public CommentNode(string name, bool canRedefine = true, bool inlines = false, bool allowEmpty = true, bool mustBeEmpty = false) {
            Name = name;

            if (canRedefine)
                _bitFlags |= 1;
            if(inlines)
                _bitFlags |= 2;
            if (allowEmpty)
                _bitFlags |= 4;
            if (mustBeEmpty)
                _bitFlags |= 8;

        }

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// 	<see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString() {
            return Name;
        }

        #endregion

        #region 系统

        public static CommentNode ProjectDescription = new CommentNode("projectDescription", false, false, false, false);
        public static CommentNode License = new CommentNode("license", false, false, false, false);
        public static CommentNode FileOverview = new CommentNode("fileOverview", false, false, false, false);
        public static CommentNode Copyright = new CommentNode("copyright", false, false, false, false);
        public static CommentNode File = new CommentNode("file", false, false, false, false);
        public static CommentNode Version = new CommentNode("version", false, false, false, false);
        public static CommentNode Requires = new CommentNode("requires", true, true, false, false);
        public static CommentNode Include = new CommentNode("include", false, false, false, false);
        public static CommentNode Progma = new CommentNode("progma", false, false, false, false);

        public static CommentNode Class = new CommentNode("class", false, true, true, false);
        public static CommentNode Enum = new CommentNode("enum", false, true, true, false);
        public static CommentNode Interface = new CommentNode("interface", false, true, true, false);
        public static CommentNode Method = new CommentNode("method", false, true, true, false);
        public static CommentNode Member = new CommentNode("member", false, true, true, false);

        public static CommentNode Event = new CommentNode("event", false, true, true, false);
        public static CommentNode Property = new CommentNode("property", false, true, true, false);
        public static CommentNode Field = new CommentNode("field", true, true, true, false);
        public static CommentNode Getter = new CommentNode("getter", false, true, true, false);
        public static CommentNode Setter = new CommentNode("setter", true, true, true, false);
        public static CommentNode Config = new CommentNode("config", true, true, true, false);

        public static CommentNode Public = new CommentNode("public", false, true, true, false);
        public static CommentNode Private = new CommentNode("private", false, true, true, false);
        public static CommentNode Protected = new CommentNode("protected", false, true, true, false);
        public static CommentNode Internal = new CommentNode("internal", false, true, true, false);

        public static CommentNode Final = new CommentNode("final", false, true, true, false);
        public static CommentNode Static = new CommentNode("static", false, true, true, false);
        public static CommentNode Abstract = new CommentNode("abstract", false, true, true, false);
        public static CommentNode Virtual = new CommentNode("virtual", false, true, true, false);
        public static CommentNode Const = new CommentNode("const", false, true, true, false);
        public static CommentNode Override = new CommentNode("override", false, true, true, false);
        public static CommentNode ReadOnly = new CommentNode("readOnly", false, true, true, false);

        public static CommentNode Constructor = new CommentNode("constructor", false, true, true, false);
        public static CommentNode Namespace = new CommentNode("namespace", false, true, true, false);

        public static CommentNode Extends = new CommentNode("extends", true, true, false, false);
        public static CommentNode Implements = new CommentNode("implements", true, true, false, false);

        public static CommentNode Author = new CommentNode("author", false, false, false, false);

        public static CommentNode Summary = new CommentNode("summary", true, false, true, false);
        public static CommentNode Remark = new CommentNode("remark", true, false, true, false);
        public static CommentNode Example = new CommentNode("example", true, false, false, false);
        public static CommentNode Syntax = new CommentNode("syntax", false, false, false, false);
        public static CommentNode Category = new CommentNode("category", false, true, false, false);
        public static CommentNode DefaultValue = new CommentNode("defaultValue", false, true, false, false);

        public static CommentNode NameNode = new CommentNode("name", false, true, false, false);
        public static CommentNode MemberOf = new CommentNode("memberOf", false, true, false, false);
        public static CommentNode Alias = new CommentNode("alias", false, true, false, false);
        //   public static CommentNode Value = new CommentNode("value", CommentType.MemberAndName, false, true, false, false);

        public static CommentNode See = new CommentNode("see", true, true, false, false);
        public static CommentNode SeeAlso = new CommentNode("seeAlso", true, true, false, false);

        public static CommentNode Return = new CommentNode("return", false, true, false, false);

      //  public static CommentNode Singleton = new CommentNode("singleton", CommentType.MemberFlag, false, true, true, true);
        public static CommentNode Ignore = new CommentNode("ignore", true, true, true, false);
        public static CommentNode System = new CommentNode("system", false, true, true, true);
        public static CommentNode Hide = new CommentNode("hide", true, true, true, false);

        public static CommentNode Since = new CommentNode("since", false, true, true, false);
        public static CommentNode Deprecated = new CommentNode("deprecated", false, true, true, false);

        public static CommentNode Param = new CommentNode("param", true, false, false, false);
        
    //   public static CommentNode Throws = new CommentNode("throws", CommentType.Throws, false, true, false, false);

        public static CommentNode TypeNode = new CommentNode("type", false, true, false, false);

        public static CommentNode Exception = new CommentNode("exception", false, true, false, false);

        public static CommentNode Define = new CommentNode("define", true, true, false, false);
        public static CommentNode Code = new CommentNode("code", true, false, false, false);

        #endregion
    
    }

}
