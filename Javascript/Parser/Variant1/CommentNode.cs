using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace DocPlus.Javascript {

    /// <summary>
    /// 表示一个标签。
    /// </summary>
    public sealed class CommentNode1 {

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
        /// 初始化 <see cref="DocPlus.DocParser.Javascript.CommentNodeNames"/> 的新实例。
        /// </summary>
        /// <param name="name">标签的名字。</param>
        /// <param name="canRedefine">是否可重定义。</param>
        /// <param name="inlines">是否可多行。</param>
        /// <param name="allowEmpty">是否可空参数。</param>
        /// <param name="mustBeEmpty">是否必须无参数。</param>
        /// <param name="isSystemDefined">是否是系统内置的。</param>
        public CommentNode1(string name, bool canRedefine = true, bool inlines = false, bool allowEmpty = true, bool mustBeEmpty = false) {
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

        public static CommentNode1 ProjectDescription = new CommentNode1("projectDescription", false, false, false, false);
        public static CommentNode1 License = new CommentNode1("license", false, false, false, false);
        public static CommentNode1 FileOverview = new CommentNode1("fileOverview", false, false, false, false);
        public static CommentNode1 Copyright = new CommentNode1("copyright", false, false, false, false);
        public static CommentNode1 File = new CommentNode1("file", false, false, false, false);
        public static CommentNode1 Version = new CommentNode1("version", false, false, false, false);
        public static CommentNode1 Requires = new CommentNode1("requires", true, true, false, false);
        public static CommentNode1 Include = new CommentNode1("include", false, false, false, false);
        public static CommentNode1 Progma = new CommentNode1("progma", false, false, false, false);

        public static CommentNode1 Class = new CommentNode1("class", false, true, true, false);
        public static CommentNode1 Enum = new CommentNode1("enum", false, true, true, false);
        public static CommentNode1 Interface = new CommentNode1("interface", false, true, true, false);
        public static CommentNode1 Method = new CommentNode1("method", false, true, true, false);
        public static CommentNode1 Member = new CommentNode1("member", false, true, true, false);

        public static CommentNode1 Event = new CommentNode1("event", false, true, true, false);
        public static CommentNode1 Property = new CommentNode1("property", false, true, true, false);
        public static CommentNode1 Field = new CommentNode1("field", true, true, true, false);
        public static CommentNode1 Getter = new CommentNode1("getter", false, true, true, false);
        public static CommentNode1 Setter = new CommentNode1("setter", true, true, true, false);
        public static CommentNode1 Config = new CommentNode1("config", true, true, true, false);

        public static CommentNode1 Public = new CommentNode1("public", false, true, true, false);
        public static CommentNode1 Private = new CommentNode1("private", false, true, true, false);
        public static CommentNode1 Protected = new CommentNode1("protected", false, true, true, false);
        public static CommentNode1 Internal = new CommentNode1("internal", false, true, true, false);

        public static CommentNode1 Final = new CommentNode1("final", false, true, true, false);
        public static CommentNode1 Static = new CommentNode1("static", false, true, true, false);
        public static CommentNode1 Abstract = new CommentNode1("abstract", false, true, true, false);
        public static CommentNode1 Virtual = new CommentNode1("virtual", false, true, true, false);
        public static CommentNode1 Const = new CommentNode1("const", false, true, true, false);
        public static CommentNode1 Override = new CommentNode1("override", false, true, true, false);
        public static CommentNode1 ReadOnly = new CommentNode1("readOnly", false, true, true, false);

        public static CommentNode1 Constructor = new CommentNode1("constructor", false, true, true, false);
        public static CommentNode1 Namespace = new CommentNode1("namespace", false, true, true, false);

        public static CommentNode1 Extends = new CommentNode1("extends", true, true, false, false);
        public static CommentNode1 Implements = new CommentNode1("implements", true, true, false, false);

        public static CommentNode1 Author = new CommentNode1("author", false, false, false, false);

        public static CommentNode1 Summary = new CommentNode1("summary", true, false, true, false);
        public static CommentNode1 Remark = new CommentNode1("remark", true, false, true, false);
        public static CommentNode1 Example = new CommentNode1("example", true, false, false, false);
        public static CommentNode1 Syntax = new CommentNode1("syntax", false, false, false, false);
        public static CommentNode1 Category = new CommentNode1("category", false, true, false, false);
        public static CommentNode1 DefaultValue = new CommentNode1("defaultValue", false, true, false, false);

        public static CommentNode1 NameNode = new CommentNode1("name", false, true, false, false);
        public static CommentNode1 MemberOf = new CommentNode1("memberOf", false, true, false, false);
        public static CommentNode1 Alias = new CommentNode1("alias", false, true, false, false);
        //   public static CommentNodeNames Value = new CommentNodeNames("value", CommentType.MemberAndName, false, true, false, false);

        public static CommentNode1 See = new CommentNode1("see", true, true, false, false);
        public static CommentNode1 SeeAlso = new CommentNode1("seeAlso", true, true, false, false);

        public static CommentNode1 Return = new CommentNode1("return", false, true, false, false);

      //  public static CommentNodeNames Singleton = new CommentNodeNames("singleton", CommentType.MemberFlag, false, true, true, true);
        public static CommentNode1 Ignore = new CommentNode1("ignore", true, true, true, false);
        public static CommentNode1 System = new CommentNode1("system", false, true, true, true);
        public static CommentNode1 Hide = new CommentNode1("hide", true, true, true, false);

        public static CommentNode1 Since = new CommentNode1("since", false, true, true, false);
        public static CommentNode1 Deprecated = new CommentNode1("deprecated", false, true, true, false);

        public static CommentNode1 Param = new CommentNode1("param", true, false, false, false);
        
    //   public static CommentNodeNames Throws = new CommentNodeNames("throws", CommentType.Throws, false, true, false, false);

        public static CommentNode1 TypeNode = new CommentNode1("type", false, true, false, false);

        public static CommentNode1 Exception = new CommentNode1("exception", false, true, false, false);

        public static CommentNode1 Define = new CommentNode1("define", true, true, false, false);
        public static CommentNode1 Code = new CommentNode1("code", true, false, false, false);

        #endregion
    
    }

}
