using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using CorePlus.Parser.Javascript;
using CorePlus.Api;

namespace DocPlus.Javascript {

    /// <summary>
    /// 解析 Java 风格的文档注释的工具。
    /// </summary>
    public sealed class JavaCommentParser {

        #region 内部

        DocProject _project;

        /// <summary>
        /// 内部使用的缓存。
        /// </summary>
        StringBuilder _buffer = new StringBuilder();

        /// <summary>
        /// 当前的行号。
        /// </summary>
        int _line;

        void Error(string message) {
            _project.ProgressReporter.Error(_project.CurrentFile, _line, message, 0);
        }

        void Warning(string message) {
            _project.ProgressReporter.Warning(_project.CurrentFile, _line, message, 0);
        }

        void Error(string message, params object[] args) {
            _project.ProgressReporter.Error(_project.CurrentFile, _line, String.Format(message, args), 0);
        }

        void Warning(string message, params object[] args) {
            _project.ProgressReporter.Warning(_project.CurrentFile, _line, String.Format(message, args), 0);
        }

        /// <summary>
        /// 初始化 <see cref="DocPlus.DocParser.Javascript.JavaCommentParser"/> 的新实例。
        /// </summary>
        public JavaCommentParser(DocProject project) {
            _project = project;
            AddDefaultTypes();
            InitTags();
            if (_project.AutoCreateParamComment) {
                _paramsTypes = new Dictionary<string, string[]>();
                AddSystemDefautParam();
            }
        }

        #endregion

        #region 标签

        /// <summary>
        /// 用于快速按名字获取标签的缓存。
        /// </summary>
        Dictionary<string, CommentNode> _commentNodes = new Dictionary<string, CommentNode>();

        /// <summary>
        /// 存放各个标签的处理函数。
        /// </summary>
        Dictionary<CommentNode, Action<Variant, CommentNode>> _actions = new Dictionary<CommentNode, Action<Variant, CommentNode>>();

        /// <summary>
        /// 默认的不支持标签的声明。
        /// </summary>
        /// <param name="currentVariant">当前解析的目标变量。</param>
        /// <param name="node">正在解析的标签。</param>
        void Warning(Variant currentVariant, CommentNode node) {
            Warning("未定义标签 " + node.Name + "， 请确认标签名是否正确，注意大小写。");
        }

        /// <summary>
        /// 默认的不支持标签的声明。
        /// </summary>
        /// <param name="currentVariant">当前解析的目标变量。</param>
        /// <param name="node">正在解析的标签。</param>
        void NotSupport(Variant currentVariant, CommentNode node) {
            Error("本版本不支持标签 @{0}", node.Name);
        }

        void AddSystemTag(CommentNode tag, Action<Variant, CommentNode> action) {
            _commentNodes[tag.Name] = tag;
            _actions[tag] = action;
        }

        /// <summary>
        /// 根据名字返回已定义的 CommentTag 。
        /// </summary>
        /// <param name="name">名字。</param>
        /// <returns>CommentTag 类实例。</returns>
        CommentNode GetCommentTags(string name) {
            if (_commentNodes.ContainsKey(name))
                return _commentNodes[name];

           CommentNode t = _commentNodes[name] = new CommentNode(name);
            _actions[t] = Warning;
            return t;
        }

        /// <summary>
        /// 初始化系统定义的标签。
        /// </summary>
        void InitTags() {

            // 全局标签

            AddSystemTag(CommentNode.Author, ParseGlobalTags);
            AddSystemTag(CommentNode.ProjectDescription, ParseGlobalTags);
            AddSystemTag(CommentNode.License, ParseGlobalTags);
            AddSystemTag(CommentNode.FileOverview, ParseGlobalTags);
            AddSystemTag(CommentNode.Copyright, ParseGlobalTags);
            AddSystemTag(CommentNode.File, ParseGlobalTags);
            AddSystemTag(CommentNode.Version, ParseGlobalTags);
            AddSystemTag(CommentNode.Progma, ParseProgma);
            AddSystemTag(CommentNode.Define, ParseDefine);
            //  AddSystemTag(CommentNode.Include);

            // 特殊处理的标签

            AddSystemTag(CommentNode.Class, (v, c) => ParseMemberType(v, MemberType.Class));
            AddSystemTag(CommentNode.Enum, (v, c) => ParseMemberType(v, MemberType.Enum));
            AddSystemTag(CommentNode.Interface, (v, c) => ParseMemberType(v, MemberType.Interface));
            AddSystemTag(CommentNode.Member, (v, c) => ParseMemberType(v, MemberType.Default));
            AddSystemTag(CommentNode.Method, (v, c) => ParseMemberType(v, MemberType.Method));
            AddSystemTag(CommentNode.Event, (v, c) => ParseTypedMemberType(v, MemberType.Event));
            AddSystemTag(CommentNode.Property, (v, c) => ParseTypedMemberType(v, MemberType.Property));
            AddSystemTag(CommentNode.Field, (v, c) => ParseTypedMemberType(v, MemberType.Field));
            AddSystemTag(CommentNode.Getter, (v, c) => ParseTypedMemberType(v, MemberType.PropertyGetter));
            AddSystemTag(CommentNode.Setter, (v, c) => ParseTypedMemberType(v, MemberType.PropertySetter));
            AddSystemTag(CommentNode.Config, (v, c) => ParseTypedMemberType(v, MemberType.Config));

            AddSystemTag(CommentNode.Namespace, ParseNamespace);
            AddSystemTag(CommentNode.Constructor, ParseConstructor);
            AddSystemTag(CommentNode.Ignore, ParseIgnore);
            AddSystemTag(CommentNode.System, ParseSystem);
            AddSystemTag(CommentNode.Hide, ParseHide);
            AddSystemTag(CommentNode.MemberOf, ParseMemberOf);
            AddSystemTag(CommentNode.NameNode, ParseName);
            AddSystemTag(CommentNode.TypeNode, ParseType);

            AddSystemTag(CommentNode.Public, (v, c) => ParseMemberAccess(v, MemberAccess.Public));
            AddSystemTag(CommentNode.Private, (v, c) => ParseMemberAccess(v, MemberAccess.Private));
            AddSystemTag(CommentNode.Protected, (v, c) => ParseMemberAccess(v, MemberAccess.Protected));
            AddSystemTag(CommentNode.Internal, (v, c) => ParseMemberAccess(v, MemberAccess.Internal));

            AddSystemTag(CommentNode.Final, (v, c) => ParseMemberAttributes(v, MemberAttributes.Sealed));
            AddSystemTag(CommentNode.Static, (v, c) => ParseMemberAttributes(v, MemberAttributes.Static));
            AddSystemTag(CommentNode.Abstract, (v, c) => ParseMemberAttributes(v, MemberAttributes.Abstract));
            AddSystemTag(CommentNode.Virtual, (v, c) => ParseMemberAttributes(v, MemberAttributes.Virtual));
            AddSystemTag(CommentNode.Const, (v, c) => ParseMemberAttributes(v, MemberAttributes.Const));
            AddSystemTag(CommentNode.ReadOnly, (v, c) => ParseMemberAttributes(v, MemberAttributes.ReadOnly));
            AddSystemTag(CommentNode.Override, (v, c) => ParseMemberAttributes(v, MemberAttributes.Override));

            // 通用的标签

            AddSystemTag(CommentNode.Summary, ParseText);
            AddSystemTag(CommentNode.Remark, ParseMultiText);
            AddSystemTag(CommentNode.Example, ParseText);
            AddSystemTag(CommentNode.Syntax, ParseText);
            AddSystemTag(CommentNode.Category, ParseText);

            AddSystemTag(CommentNode.Alias, ParseMultiName);
            AddSystemTag(CommentNode.DefaultValue, ParseText);

            AddSystemTag(CommentNode.Extends, ParseExtendsAndImplements);
            AddSystemTag(CommentNode.Implements, ParseExtendsAndImplements);
            AddSystemTag(CommentNode.Exception, ParseException);
            AddSystemTag(CommentNode.Param, ParseParams);
            AddSystemTag(CommentNode.Return, ParseReturn);

            AddSystemTag(CommentNode.See, ParseMultiLink);
            AddSystemTag(CommentNode.SeeAlso, ParseMultiLink);
            AddSystemTag(CommentNode.Requires, ParseRequires);

            AddSystemTag(CommentNode.Since, ParseText);
            AddSystemTag(CommentNode.Deprecated, ParseText);;

            //  AddSystemTag(CommentNode.Throws);



          //  AddSystemTag(CommentNode.Code, NotSupport);



            DefineComment("fileoverview", "fileOverview");
            DefineComment("seealso", "seeAlso");
            DefineComment("returns", "return");
            DefineComment("extend", "extends");
            DefineComment("inherits", "extends");
            DefineComment("params", "param");
            DefineComment("args", "param");
            DefineComment("arguments", "param");
            DefineComment("argument", "param");
            DefineComment("group", "category");
            DefineComment("throws", "exception");
            DefineComment("cfg", "config");
            DefineComment("implement", "implements");
            DefineComment("throw", "exception");
            DefineComment("augments", "extends");
            DefineComment("inner", "internal");
            DefineComment("singleton", "static");
            DefineComment("lends", "memberOf");
            DefineComment("memberof", "memberOf");
            DefineComment("value", "defaultValue");
            DefineComment("defaultvalue", "defaultValue");
            DefineComment("default", "defaultValue");
            DefineComment("value", "defaultValue");
            DefineComment("description", "summary");
            DefineComment("constant", "const");
            DefineComment("sealed", "final");

        }

        /// <summary>
        /// 定义标签的引用。这样使用 <paramref name="fromName"/> 标签可代替 <paramref name="toName"/> 标签的使用。
        /// </summary>
        /// <param name="fromName">要操作的字符串。</param>
        /// <param name="toName">要操作的字符串。</param>
        public void DefineReference(string fromName, string toName) {
            _commentNodes[fromName] = GetCommentTags(toName);
        }

        #endregion

        #region 全局

        GlobalConfigs _globalConfigs = new GlobalConfigs();

        /// <summary>
        /// 获取当前文档解析得到的全局配置对象。
        /// </summary>
        public GlobalConfigs GlobalConfigs {
            get {
                return _globalConfigs;
            }
        }

        #endregion

        #region 变量类型

        Dictionary<string, string[]> _paramsTypes;

        /// <summary>
        /// 类型转换的字典。
        /// </summary>
        NameValueCollection _types = new NameValueCollection();

        void AddDefaultTypes() {
            _types["bool"] = "Boolean";
            _types["boolean"] = "Boolean";
            _types["int"] = "Integer";
            _types["integer"] = "Integer";
            _types["number"] = "Number";
            _types["num"] = "Number";
            _types["float"] = "Number";
            _types["double"] = "Number";
            _types["single"] = "Number";
            _types["decimal"] = "Integer";
            _types["string"] = "String";
            _types["function"] = "Function";
            _types["json"] = "Json";
            _types["array"] = "Array";
            _types["object"] = "Object";
            _types["date"] = "Date";
        }

        void AddSystemDefautParam() {
            DefineParamType("node", "Node");
            DefineParamType("elem", "Element");
            DefineParamType("el", "Element");
            DefineParamType("fn", "Function");
            DefineParamType("date", "Date");
            DefineParamType("func", "Function");
            DefineParamType("attr", "String");
            DefineParamType("options", "Object");
            DefineParamType("opt", "Object");
            DefineParamType("obj", "Object");
            DefineParamType("checker", "Function");
            DefineParamType("arr", "Array");
            DefineParamType("str", "String");
            DefineParamType("callback", "Function");
            DefineParamType("id", "String/Element");
        }

        string[] GetAutoPa(string p) {
            return _paramsTypes.ContainsKey(p) ? _paramsTypes[p] : null;
        }

        /// <summary>
        /// 添加参数名和类型的对应关系，这样如果参数没有指定类型时，会自定匹配该类型。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        void DefineParamType(string name, string type) {
            _paramsTypes[name] = new string[] { type, name, String.Empty };
        }

        void DefineComment(string fromName, string toName) {
            _commentNodes[fromName] = _commentNodes.ContainsKey(toName) ? _commentNodes[toName] : new CommentNode(fromName);
        }

        #endregion

        #region 处理函数

        #region 底层处理

        static bool NotName(int i) {
            return !ECMACharType.IsIdentifierPart(i) && i != '.' && i != '#';
        }

        /// <summary>
        /// 读取名字。
        /// </summary>
        /// <returns></returns>
        static string ReadName(StringBuilder sb, ref int start) {

            // [DisplayName]

            SkipWhiteSpace(sb, ref start);

            string result = ReadIfNot(sb, ref start, NotName);

            return result;
        }

        static string ReadLiteral(StringBuilder sb, ref int start) {
            char q = '\0';


            int org = start;

            while (start < sb.Length) {

                if (q == '\0') {
                    switch (sb[start]) {
                        case '\'':
                        case '\"':
                        case '/':
                        case '`':
                            q = sb[start];
                            break;
                        case '[':
                            q = ']';
                            break;
                        case '{':
                            q = '}';
                            break;
                        default:
                            q = ' ';
                            break;
                    }

                    start++;
                    continue;
                }

                if (q == ' ') {
                    if (char.IsWhiteSpace(sb[start]) || sb[start] == ']')
                        break;
                } else if (q == sb[start]) {

                    if (q == '`')
                        org++;
                    else
                        start++;
                    break;
                }


                start++;
            }

            if (org >= sb.Length) {
                return String.Empty;
            }

            if (start > sb.Length) {
                start = sb.Length;
            }

            return sb.ToString(org, start - org);
        }

        static string ReadDefaultValue(StringBuilder sb, ref int start) {

            SkipWhiteSpace(sb, ref start);
            if (start < sb.Length && sb[start] == '=') {
                start++;
                SkipWhiteSpace(sb, ref start);
                return ReadLiteral(sb, ref start);
            }

            return null;
        }

        static bool IsWhiteSpace(int c) {
            return char.IsWhiteSpace((char)c);
        }

        static string ReadWord(StringBuilder sb, ref int start) {
            return ReadAll(sb, ref start, ECMACharType.IsIdentifierPart);
        }

        static void SkipWhiteSpace(StringBuilder sb, ref int start) {
            if (start < sb.Length && char.IsWhiteSpace(sb[start])) {

                while (++start < sb.Length && char.IsWhiteSpace(sb[start])) ;

            }
        }

        static void SkipWhiteSpace(StringBuilder sb, ref int start, ref int currenLine) {
            while (start < sb.Length && char.IsWhiteSpace(sb[start])) {
                CalcLine(sb, ref start, ref currenLine);
                start++;
            }
        }

        static string ReadIfNot(StringBuilder sb, ref int start, Predicate<int> cond) {
            int s = start;
            for (; start < sb.Length && !cond(sb[start]); start++) ;

            return sb.ToString(s, start - s);

        }

        /// <summary>
        /// 如果满足指定条件就继续读。
        /// </summary>
        /// <param name="_buffer"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="conditon"></param>
        /// <returns></returns>
        static string ReadAll(StringBuilder sb, ref int start, Predicate<int> cond) {

            int s = start;
            for (; start < sb.Length && cond(sb[start]); start++) ;

            return sb.ToString(s, start - s);
        }

        static bool IsSeperator(int c) {
            return c == '|' || c == '\\' || c == '/' || c == ',' || c == '}' || c == '\r' || c == '\n' || char.IsWhiteSpace((char)c);
        }

        static bool IsCharQ(StringBuilder sb, int start, char c) {
            if (start >= sb.Length - 3 || sb[start] != c || sb[start + 1] != c || sb[start + 2] != c) {
                return false;
            }

            return true;
        }

        static void CalcLine(StringBuilder sb, ref int start, ref int currentLine) {
            // 如果后面为换行。
            switch (sb[start]) {
                case '\r':
                    if (++start == sb.Length || sb[start] != '\n')
                        start--;  // 跳过 \r
                    goto case '\n';
                case '\n':
                case '\u2029':
                    currentLine++;
                    break;
            }
        }

        /// <summary>
        /// 把2个数组的内容合并。
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        /// <returns></returns>
        static string[] Concat(string[] p, string[] p_2) {
            if (p == null)
                return p_2;

            string[] r = new string[p.Length + p_2.Length];
            p.CopyTo(r, 0);
            p_2.CopyTo(r, p.Length);

            return r;
        }

        #endregion

        #region 基本封装

        string ReadName(ref int start) {
            return ReadName(_buffer, ref start);
        }

        string ReadDefaultValue(ref int start) {
            return ReadDefaultValue(_buffer, ref start);
        }

        void EnsureEmpty(ref int start) {

            SkipWhiteSpace(ref start);

            if (start < _buffer.Length) {
                Error("检测到多余的字符:" + _buffer.ToString(start, _buffer.Length - start));
            }

        }

        string ReadNameAndEnsureEmpty(ref int start) {

            string r = ReadName(ref start);

            EnsureEmpty(ref start);

            return r;

        }

        string ReadText(ref int start) {
            SkipWhiteSpace(ref start);

            int end = _buffer.Length;

            while (start < --end && IsWhiteSpace(_buffer[end])) ;

            return _buffer.ToString(start, end - start + 1);
        }

        string ReadType(ref int start, bool enableMap = true) {
            return ReadType(_buffer, ref start, enableMap);
        }

        string ReadType(StringBuilder sb, ref int start, bool enableMap = true) {

            SkipWhiteSpace(sb, ref start);

            string type = String.Empty;

            if (start < sb.Length && sb[start] == '{') {

                string tmp;

                do {

                    start++;

                    SkipWhiteSpace(sb, ref start);

                    tmp = ReadIfNot(sb, ref start, IsSeperator);

                    if (enableMap)
                        tmp = _types[tmp] ?? tmp;

                    if (start < sb.Length && sb[start] != '}') {
                        while (++start < sb.Length && sb[start] != '}' && IsSeperator(sb[start])) ;

                        --start;
                    }

                    if (type.Length == 0)
                        type = tmp;
                    else if (tmp.Length > 0)
                        type += '/' + tmp;
                } while (tmp.Length > 0 && start < sb.Length && sb[start] != '}');


                if (start == sb.Length || sb[start] != '}') {
                    Error(" } 未关闭");
                } else {
                    start++;
                }
            }

            return type;
        }

        string ReadLink(ref int start) {
            //  SkipWhiteSpace(ref start);

            return ReadText(ref start);
        }

        string ReadWord(ref int start) {
            return ReadAll(_buffer, ref start, ECMACharType.IsIdentifierPart);
        }

        void SkipWhiteSpace(ref int start) {
            if (start < _buffer.Length && char.IsWhiteSpace(_buffer[start])) {
                while (++start < _buffer.Length && char.IsWhiteSpace(_buffer[start])) ;

            }
        }

        #endregion

        #region 高级封装

        string[] ReadName() {
            int start = 0;

            return new string[1] { ReadNameAndEnsureEmpty(ref start) };
        }

        string[] ReadText() {
            int start = 0;

            return new string[1] { ReadText(ref start) };
        }

        string[] ReadLink() {
            int start = 0;
            string[] r = new string[1] { ReadLink(ref start) };

            //   EnsureEmpty(ref start);


            return r;
        }

        string[] ReadReturn() {

            int start = 0;

            string type = ReadType(ref start);

            string summary = ReadText(ref start);

            return new string[2] { type, summary };
        }

        string[] ReadParams(bool autoCreateParam) {
            int start = 0;

            string type = ReadType(ref start);

            SkipWhiteSpace(ref start);

            byte foundLB = 0;

            if (start < _buffer.Length && _buffer[start] == '[') {
                foundLB = 1;
                start++;
            }

            string name = ReadIfNot(_buffer, ref start, NotName);

            if (foundLB > 0) {
                SkipWhiteSpace(ref start);
                if (start < _buffer.Length && _buffer[start] == ']') {
                    start++;
                    foundLB++;
                }
            }

            string dftValue = ReadDefaultValue(ref start);

            if (foundLB > 0) {
                if (dftValue == null)
                    dftValue = String.Empty;

                if (foundLB == 1) {
                    SkipWhiteSpace(ref start);
                    if (start < _buffer.Length && _buffer[start] == ']')
                        start++;
                    else
                        Error("缺少 ]");
                }
            }

            string summary = ReadText(ref start);

            return FillParamType(new string[4] { type, name, summary, dftValue }, autoCreateParam);


        }

        string[] FillParamType(string[] r, bool autoCreateParam) {
            if (_project.AutoCreateParamComment) {
                if (r[0].Length == 0 && !String.IsNullOrEmpty(r[1])) {

                    if (_paramsTypes.ContainsKey(r[1])) {

                        if (r[0].Length == 0)
                            r[0] = _paramsTypes[r[1]][0];

                        if (r[2].Length == 0)
                            r[2] = _paramsTypes[r[1]][2];
                    }

                    if (r[0].Length == 0) {
                        if (r[1].StartsWith("is") || r[1].StartsWith("has") || r[1].StartsWith("contains") || r[1].StartsWith("can")) {
                            r[0] = "Boolean";
                        } else if (r[1].StartsWith("str")) {
                            r[0] = "String";
                        }

                    }

                } else if (autoCreateParam) {
                    _paramsTypes[r[1]] = r;
                }
            }

            return r;
        }

        #endregion

        //#region 其它处理

        ////void TryReadName(Variant currentVariant, CommentNode node) {
        ////    int start = 0;
        ////    string r = ReadNameAndEnsureEmpty(ref start);

        ////    if (r.Length == 0)
        ////        Error("{0} 标签后缺少合法的标识符。", node);
        ////    else {
        ////        currentVariant[node] = new string[1] { r };
        ////    }
        ////}

        ////void CheckMember(MemberType exits, CommentNode node) {

        ////    if (exits != MemberType.Default) {

        ////        if (exits.ToString() == node.DisplayName)
        ////            Error("使用了重复的标签 {0}。", node.DisplayName);
        ////        else
        ////            Error(" {0} 和 {1} 不能同时使用，因为两者所表示的意义相同。", node.DisplayName, exits.ToString());
        ////    }
        ////}

        ////void TryReadSummary(Variant currentVariant, CommentNode node) {
        ////    string[] r = ReadSummary();
        ////    if (r[0].Length > 0)
        ////        currentVariant[node] = r;
        ////}

        ////bool CheckMember(CommentNode exits, CommentNode node, CommentNode node1, CommentNode node2) {
        ////    if (exits != null) {

        ////        if ((exits == node1 && node == node2) || (exits == node2 && node == node1)) {
        ////            return true;
        ////        }

        ////        if (exits == node)
        ////            Error("使用了重复的标签 {0}", node.DisplayName);
        ////        else
        ////            Error(" {0} 和 {1} 不能同时使用", node.DisplayName, exits.DisplayName);
        ////    }

        ////    return false;
        ////}

        ////void CheckRedefine(Variant currentVariant, CommentNode node) {
        //    if (!node.CanRedefine && currentVariant.ContainsKey(node))
        //        Error("使用了重复的标签 {0}", node.DisplayName);
        //}

        //#endregion

        #region 底层更新

        void UpdateText(Variant currentVariant, CommentNode node, string value) {
            if (currentVariant.ContainsKey(node)) {
                Error("同名标签覆盖: @summary {0} => {1}。", currentVariant[node], value);
            }

            currentVariant[node] = new string[]{ value };
        }

        void UpdateName(Variant currentVariant, string name) {

            // 如果有 . 分隔为 memberOf 和 name
            int i = name.LastIndexOf('.');
            if (i > 0 && i < name.Length - 1) {
                string member = name.Substring(0, i);
                if (currentVariant.MemberOf != null) {
                    Error("同名标签覆盖: @memberOf {0} => {1}。", currentVariant.MemberOf, member);
                }
                currentVariant.MemberOf = member;
                name = name.Substring(i + 1);
            }

            if (currentVariant.Name != null) {
                Error("同名标签覆盖: @name {0} => {1}。", currentVariant.Name, name);
            }

            currentVariant.Name = name;
            currentVariant.UpdateNamespaceSetter();
        }

        void UpdateType(Variant currentVariant, string type) {
            if (currentVariant.Type != null) {
                Error("同名标签覆盖: @type {0} => {1}。", currentVariant.Type, type);
            }
            currentVariant.Type = type;
        }

        void UpdateMemberType(Variant currentVariant, MemberType memberType) {
            if (currentVariant.MemberType != MemberType.Default) {
                if (currentVariant.MemberType == memberType) {
                    Warning("重复声明了 @{0}", memberType.ToString().ToLowerInvariant());
                } else {
                    Error("同名标签覆盖: @{0} => @{1}。", currentVariant.MemberType.ToString().ToLowerInvariant(), memberType.ToString().ToLowerInvariant());
                }
            }
            currentVariant.MemberType = memberType;
            currentVariant.UpdateNamespaceSetter();
        }

        void UpdateMemberAccess(Variant currentVariant, MemberAccess value) {
            if (currentVariant.MemberAccess != MemberAccess.Default) {
                if ((currentVariant.MemberAccess == MemberAccess.Protected && value == MemberAccess.Internal) || (currentVariant.MemberAccess == MemberAccess.Internal && value == MemberAccess.Protected)) {
                    value = MemberAccess.ProtectedInternal;
                } else if (currentVariant.MemberAccess == value) {
                    Error("重复指定标签: @{0}。", value.ToString().ToLowerInvariant());
                } else {
                    Error("同名标签覆盖: @{0} => @{1}。", currentVariant.MemberAccess.ToString().ToLowerInvariant(), value.ToString().ToLowerInvariant());
                }
            }

            currentVariant.MemberAccess = value;
        }

        void UpdateMemberAttributes(Variant currentVariant, MemberAttributes value) {
            if (currentVariant.MemberAttribute != MemberAttributes.None) {
                if ((currentVariant.MemberAttribute == MemberAttributes.ReadOnly && value == MemberAttributes.Static) || (currentVariant.MemberAttribute == MemberAttributes.Static && value == MemberAttributes.ReadOnly)) {
                    value = MemberAttributes.StaticReadOnly;
                } else if ((currentVariant.MemberAttribute == MemberAttributes.ReadOnly && value == MemberAttributes.Const) || (currentVariant.MemberAttribute == MemberAttributes.Const && value == MemberAttributes.ReadOnly)) {
                    value = MemberAttributes.StaticConst;
                } else if (currentVariant.MemberAttribute == value) {
                    Error("重复指定标签: @{0}。", value.ToString().ToLowerInvariant());
                } else {
                    Error("同名标签覆盖: @{0} => @{1}。", currentVariant.MemberAttribute.ToString().ToLowerInvariant(), value.ToString().ToLowerInvariant());
                }
            }

            currentVariant.MemberAttribute = value;
        }

        #endregion

        #region 底层更新的封装

        void TryUpdateText(Variant currentVariant, CommentNode node, ref int start) {
            string text = ReadText(ref start);
            if (text.Length > 0) {
                UpdateText(currentVariant, node, text);
            }
        }

        void TryUpdateName(Variant currentVariant, ref int start) {
            string name = ReadName(ref start);
            if (name.Length > 0) {
                UpdateName(currentVariant, name);
            }
        }

        void TryUpdateType(Variant currentVariant, ref int start) {
            string type = ReadType(ref start);
            if (type.Length > 0) {
                UpdateType(currentVariant, type);
            }
        }

        void TryUpdateDefaultValue(Variant currentVariant, ref int start) {
            string type = ReadDefaultValue(ref start);
            if (type != null) {
                UpdateText(currentVariant, CommentNode.DefaultValue, type);
            }
        }

        void TryReadNextNode(Variant currentVariant, string currentName) {
            int start = 0;
            string s = ReadName(ref start);
            if (s.Length > 0) {
                _buffer.Remove(0, start);
                if (_commentNodes.ContainsKey(s))
                    ParseCommentTag(currentVariant, _commentNodes[s]);
                else
                    Error("@{0} 后有无效的字符串\"{1}\"", currentName.ToLowerInvariant(), s);
            }
        }

        #endregion

        #region 真正的标签解析逻辑

        #region 全局标签

        /// <summary>
        /// 解析全局性质的标签。
        /// </summary>
        /// <param name="currentVariant">当前解析的目标变量。</param>
        /// <param name="node">正在解析的标签。</param>
        void ParseGlobalTags(Variant currentVariant, CommentNode node) {
            int start = 0;
            string summary = ReadText(ref start);
            if (_globalConfigs.ContainsKey(node)) {
                Error("同名标签覆盖: @{0} {1} => {2}", node.Name, _globalConfigs[node], summary);
            }
            _globalConfigs[node] = summary;
        }

        void ParseProgma(Variant currentVariant, CommentNode node) {

        }

        void ParseDefine(Variant currentVariant, CommentNode node) {
            int start = 0;
            SkipWhiteSpace(ref start);

            if (start < _buffer.Length) {
                switch (_buffer[start]) {
                    case '{':
                        string fromType = ReadType(ref start, false);
                        string toType = ReadType(ref start, false);

                        if (toType.Length > 0)
                            _types[fromType] = toType;
                        else
                            Error("@define 需要第2个参数");
                        break;

                    default:
                        SkipWhiteSpace(ref start);
                        string fromWord = ReadWord(ref start);
                        SkipWhiteSpace(ref start);
                        string toWord = ReadWord(ref start);

                        DefineComment(fromWord, toWord);
                        EnsureEmpty(ref start);
                        break;



                }

            } else {
                Error("@define 后面缺少内容");
            }
        }

        #endregion

        #region 成员类型

        void ParseMemberType(Variant currentVariant, MemberType memberType) {
            int start = 0;
            TryUpdateName(currentVariant, ref start);
            TryUpdateText(currentVariant, CommentNode.Summary, ref start);
            UpdateMemberType(currentVariant, memberType);
        }

        void ParseTypedMemberType(Variant currentVariant, MemberType memberType) {
            int start = 0;
            TryUpdateType(currentVariant, ref start);
            TryUpdateName(currentVariant, ref start);
            TryUpdateDefaultValue(currentVariant, ref start);
            TryUpdateText(currentVariant, CommentNode.Summary, ref start);
            UpdateMemberType(currentVariant, memberType);
        }

        void ParseMemberAccess(Variant currentVariant, MemberAccess value) {
            UpdateMemberAccess(currentVariant, value);
            TryReadNextNode(currentVariant, value.ToString());
        }

        void ParseMemberAttributes(Variant currentVariant, MemberAttributes value) {
            UpdateMemberAttributes(currentVariant, value);
            TryReadNextNode(currentVariant, value.ToString());
        }

        #endregion

        #region 通用标签

        void ParseText(Variant currentVariant, CommentNode node) {
            int start = 0;
            TryUpdateText(currentVariant, node, ref start);
        }

        void ParseNamespace(Variant currentVariant, CommentNode node) {
            int start = 0;
            string name = ReadName(ref start);
            if (name.Length > 0) {
                //if (currentVariant.NamespaceSetter != null) {
                //    Error("同名标签覆盖: @namespace {1} => {2}", currentVariant.NamespaceSetter, name);
                //}
                if(currentVariant.Name == null)
                    currentVariant.Name = name;

                currentVariant.NamespaceSetter = name;
            }
        }

        void ParseName(Variant currentVariant, CommentNode node) {
            int start = 0;
            string name = ReadText(ref start);
            if (name.Length > 0) {
                UpdateName(currentVariant, name);
            }
        }

        void ParseType(Variant currentVariant, CommentNode node) {
            int start = 0;
            SkipWhiteSpace(ref start);
            if (start == _buffer.Length || _buffer[start] != '{') {
                if (start > 0)
                    _buffer[--start] = '{';
                else
                    _buffer.Insert(0, '{');

                _buffer.Append('}');
            }

            string type = ReadType(ref start);
            if (type.Length > 0) {
                EnsureEmpty(ref start);

                if (currentVariant.Type != null) {
                    Error("同名标签覆盖: @type {1} => {2}", currentVariant.Type, type);
                }

                currentVariant.Type = type;
            } else {
                Error("需要类型");
            }
        }

        void ParseRequires(Variant currentVariant, CommentNode node) {
            currentVariant[node] = Concat(currentVariant[node], ReadLink());
        }

        void ParseException(Variant currentVariant, CommentNode node) {
            if (currentVariant.Exceptions == null) {
                currentVariant.Exceptions = new List<string[]>();
            }
            currentVariant.Exceptions.Add(ReadReturn());
        }

        void ParseParams(Variant currentVariant, CommentNode node) {
            if (currentVariant.Params == null) {
                currentVariant.Params = new List<string[]>();
            }
            currentVariant.Params.Add(ReadParams(currentVariant.Summary == null));
        }

        void ParseMemberOf(Variant currentVariant, CommentNode node) {
            int start = 0;
            string name = ReadName(ref start);
            if (name.Length > 0) {
                if (currentVariant.MemberOf != null) {
                    Error("同名标签覆盖: @namespace {1} => {2}", currentVariant.NamespaceSetter, name);
                }

                currentVariant.MemberOf = name;
            }
        }

        void ParseReturn(Variant currentVariant, CommentNode node) {
            int start = 0;

            string type = ReadType(ref start);

            string summary = ReadText(ref start);

            if (type.Length > 0) {
                if (currentVariant.ReturnType != null) {
                    Error("同名标签覆盖: @return {{{1}}} => {{{2}}}", currentVariant.ReturnType, type);
                }

                currentVariant.ReturnType = type;
            }

            if(summary.Length > 0)
                currentVariant.ReturnSummary = summary;
        }

        void ParseLink(Variant currentVariant, CommentNode node) {
            currentVariant[node] = Concat(currentVariant[node], ReadLink());
        }

        void ParseExtendsAndImplements(Variant currentVariant, CommentNode node) {
            if (currentVariant.MemberType == MemberType.Default)
                currentVariant.MemberType = MemberType.Class;
            ParseMultiName(currentVariant, node);
        }

        void ParseMultiName(Variant currentVariant, CommentNode node) {
            string[] r = ReadName();
            if (!node.AllowEmpty && r[0].Length == 0)
                Error("{0} 后面不能为空。", node);
            else
                currentVariant[node] = Concat(currentVariant[node], r);
        }

        void ParseMultiText(Variant currentVariant, CommentNode node) {
            int start = 0;
            string r = ReadText(ref start);
            if (currentVariant.ContainsKey(node)) {
                currentVariant[node][0] += Environment.NewLine + r;
            } else {
                currentVariant[node] = new string[] { r };
            }
        }

        void ParseMultiLink(Variant currentVariant, CommentNode node) {
            string[] r = ReadLink();
            if (!node.AllowEmpty && r[0].Length == 0)
                Error("{0} 后面不能为空。", node);
            else
                currentVariant[node] = Concat(currentVariant[node], r);
        }

        void ParseIgnore(Variant currentVariant, CommentNode node) {
            int start = 0;
            string r = ReadNameAndEnsureEmpty(ref start);
            if (r.Length == 0)
                currentVariant.Ignore = true;
            else {
                if (_globalConfigs.Ignores == null) {
                    _globalConfigs.Ignores = new List<string>();
                }

                _globalConfigs.Ignores.Add(r);
            }
        }

        void ParseSystem(Variant currentVariant, CommentNode node) {
            currentVariant.System = true;
        }

        void ParseHide(Variant currentVariant, CommentNode node) {
            currentVariant.Hide = true;
        }

        void ParseConstructor(Variant currentVariant, CommentNode node) {
            int start = 0;
            TryUpdateName(currentVariant, ref start);
            TryUpdateName(currentVariant, ref start);
            TryUpdateText(currentVariant, CommentNode.Summary, ref start);
            currentVariant.IsConstructor = true;
        }

        #endregion

        #endregion

        #region 解析

        /// <summary>
        /// 对单一的标签解析。
        /// </summary>
        /// <param name="currentVariant">标签所在的注释。</param>
        /// <param name="node">当前标签类型。</param>
        void ParseCommentTag(Variant currentVariant, CommentNode node) {

            if (!node.AllowEmpty && _buffer.Length == 0) {
                Error("{0} 标签后必须有内容。", node);
                return;
            }

            if (node.MustBeEmpty) {
                int start = 0;
                SkipWhiteSpace(ref start);
                if (start < _buffer.Length)
                    Warning("{0} 标签后有多余的内容，已忽略。", node);
            }

            _actions[node](currentVariant, node);



        }

        /// <summary>
        /// 解析一个注释节点
        /// </summary>
        /// <param name="token">注释内容。</param>
        /// <returns>文档注释。</returns>
        public Variant Parse(TokenInfo token) {

            // 创建新的文档注释。
            Variant currentVariant = new Variant(token.StartLocation, token.EndLocation);

            // 目前属于的标签。默认是 Summary
            CommentNode current = CommentNode.Summary;

            StringBuilder value = token.LiteralBuffer;

            int len = value.Length;

            // 缓存更新。
            _buffer.Length = 0;


            int i = 1;   //  忽视第一个  *
            bool isLineStart = true;
            _line = token.StartLocation.Line;

            // 是否在 {@}  内。
            string inDoc = null;

            // 2个注释间 以 @ 分开。

            while (i < len) {

                switch (value[i]) {
                    case '@':

                        //   读取 @
                        if (++i == len)
                            break;

                        // 如果后是符号， 作为转义处理。
                        if (char.IsPunctuation(value[i])) {
                            switch (value[i]) {
                                case '"':
                                case '\'':
                                    i++;
                                    _buffer.Append("&qote;");
                                    continue;
                                case '&':
                                    i++;
                                    _buffer.Append("&amp;");
                                    continue;
                                default:
                                    _buffer.Append(value[i++]);
                                    continue;
                            }
                        }

                        // 如果后面为换行。
                        switch (value[i]) {
                            case '\r':
                                if (++i == len || value[i] != '\n')
                                    i--;  // 跳过 \r
                                goto dod;
                            case '\n':
                            case '\u2029':
                                goto dod;  // 按新行处理， 但无需标记新行。
                            case '<':
                                i++;
                                _buffer.Append("&lt;");
                                continue;
                            case '>':
                                i++;
                                _buffer.Append("&gt;");
                                continue;
                        }


                        // 如果是行首， 进行 @命名 处理
                        if (isLineStart) {
                            isLineStart = false;

                            // 回去检查 @ 之前是否是空格，如果是空格，解析为 @标签。
                        } else if (i <= 3 || !IsWhiteSpace(value[i - 2])) {



                            i--;

                            goto default;

                        }

                        // 结束上一个 current。
                        ParseCommentTag(currentVariant, current);

                        // 读取新的 current 。
                        _buffer.Length = 0;

                        current = GetCommentTags(ReadWord(value, ref i));

                        break;

                    case '\r':
                        if (++i == len || value[i] != '\n') {
                            i--;
                        }

                        goto case '\n';
                    case '\n':
                    case '\u2029':

                        isLineStart = true;



                    dod:

                        // 读取 \n
                        i++;

                        // 新行。
                        _line++;

                        // 跳过空格。
                        SkipWhiteSpace(value, ref i, ref _line);

                        // 跳过开始的 *
                        while (i < len && value[i] == '*') {

                            // 忽略 *
                            i++;

                            // 继续忽略空格。
                            SkipWhiteSpace(value, ref i, ref _line);


                        }


                        if (!current.Inlines)
                            _buffer.Append(_project.NewLine);

                        break;



                    default:

                        // 如果当前已经换行， 但现在写的标签未支持 多行 ， 关闭当前标签。剩余部分标为 remark  。 
                        if (isLineStart && current.Inlines) {

                            // 结束上一个 current。
                            ParseCommentTag(currentVariant, current);

                            current = CommentNode.Remark;
                            _buffer.Length = 0;

                            isLineStart = false;

                        }


                        switch (value[i]) {
                            case '{':
                                int c = i + 1;
                                if (c < len && value[c] == '@') {

                                    //忽略 @
                                    i = c + 1;

                                    inDoc = ReadWord(value, ref i);
                                    _buffer.Append("<").Append(inDoc);

                                    if (_commentNodes.ContainsKey(inDoc))
                                        inDoc = _commentNodes[inDoc].Name;
                                    switch (inDoc) {
                                        case "link":
                                            _buffer.Append(">");
                                            break;
                                        case "param":
                                            _buffer.Append(" type=\"").Append(ReadType(value, ref i)).Append("\" name=\"").Append(ReadName(value, ref i)).Append("\">");
                                            break;
                                        case "return":
                                            _buffer.Append(" type=\"").Append(ReadType(value, ref i)).Append("\">");
                                            break;
                                        case "code":
                                            string type = ReadType(value, ref i);
                                            if (type.Length > 0) {
                                                _buffer.Append(" type=\"").Append(type).Append("\"");
                                            }

                                            _buffer.Append(">");




                                            SkipWhiteSpace(value, ref i, ref _line);

                                            if (IsCharQ(value, i, '<')) {
                                                i += 3;

                                                SkipWhiteSpace(value, ref i, ref _line);



                                                while (i < len && (value[i] != '>' || !IsCharQ(value, i, '>') && value.Length - i >= 3)) {
                                                    CalcLine(value, ref i, ref _line);

                                                    _buffer.Append(value[i++]);
                                                }

                                                int tmp = 0, end = _buffer.Length - 1;
                                                while (IsWhiteSpace(_buffer[end])) {
                                                    tmp++; end--;
                                                }
                                                if (tmp > 0)
                                                    _buffer.Remove(end + 1, tmp);

                                                i += 3;
                                            }
                                            break;

                                        default:
                                            _buffer.Append(">");
                                            break;
                                    }

                                    SkipWhiteSpace(value, ref i, ref _line);
                                    continue;
                                } else {
                                    break;
                                }

                            case '}':

                                if (inDoc != null) {
                                    i++;
                                    _buffer.Append("</").Append(inDoc).Append(">");
                                    inDoc = null;
                                    continue;
                                } else {
                                    break;
                                }

                        }

                        _buffer.Append(value[i++]);



                        break;


                }
            }

            // 结束上一个 current。
            ParseCommentTag(currentVariant, current);

            return currentVariant;

        }

        #endregion

        #endregion
    }
}
