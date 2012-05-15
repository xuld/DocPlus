using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using CorePlus.Parser.Javascript;

namespace DocPlus.Javascript {

    /// <summary>
    /// 解析 Java 风格的文档注释的工具。
    /// </summary>
    public sealed partial class JavaCommentParser {

        #region 内部

        #region 共享的变量

        /// <summary>
        /// 当前解析所属的项目。
        /// </summary>
        DocProject _project;

        /// <summary>
        /// 内部使用的缓存。
        /// </summary>
        StringBuilder _buffer = new StringBuilder();

        /// <summary>
        /// 当前指针在 _buffer 中的位置。
        /// </summary>
        int _position;

        /// <summary>
        /// 当前的行号。
        /// </summary>
        int _line;

        /// <summary>
        /// 当前正在解析的节点名。
        /// </summary>
        string _currentNodeName;

        /// <summary>
        /// 当前正在解析的变量。
        /// </summary>
        Variant _currentVariant;

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

        #region 地图对象

        /// <summary>
        /// 用于快速按名字获取标签的缓存。
        /// </summary>
        Dictionary<string, Action> _commentNodes = new Dictionary<string, Action>();

        NameValueCollection _defines = new NameValueCollection();

        /// <summary>
        /// 类型转换的字典。
        /// </summary>
        NameValueCollection _typedefs = new NameValueCollection();

        NameValueCollection _paramsTypes = new NameValueCollection();

        #endregion

        #region 警告和错误

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
        /// 默认的不支持标签的声明。
        /// </summary>
        /// <param name="currentVariant">当前解析的目标变量。</param>
        /// <param name="node">正在解析的标签。</param>
        void Warning(Variant currentVariant, CommentNode1 node) {
            Warning("未定义标签 " + node.Name + "， 请确认标签名是否正确，注意大小写。");
        }

        #endregion

        #region 必备的函数

        /// <summary>
        /// 初始化 <see cref="DocPlus.DocParser.Javascript.JavaCommentParser"/> 的新实例。
        /// </summary>
        public JavaCommentParser(DocProject project) {
            _project = project;
            AddDefaultTypes();
            InitNodes();
            if(_project.AutoCreateParamComment) {
                _paramsTypes = new Dictionary<string, string[]>();
                AddSystemDefautParam();
            }
        }

        /// <summary>
        /// 解析一个注释节点
        /// </summary>
        /// <param name="token">注释内容。</param>
        /// <returns>文档注释。</returns>
        public Variant Parse(TokenInfo token) {

            #region 更新解析器状态

            // 记录行数，方便出错时提示错误。
            _line = token.StartLocation.Line;

            // 创建新的文档注释。
            _currentVariant = new Variant(token.StartLocation, token.EndLocation);

            // 目前属于的标签。默认是 @summary
            _currentNodeName = NodeNames.Summary;

            // 更新目前状态。
            _buffer.Length = 0;
            _position = 0;

            #endregion

            // 输入源缓存。
            StringBuilder value = token.LiteralBuffer;
            int len = value.Length;
            int i = 1;   //  忽视第一个  *

            // 是否在 {@}  内。
            int quoteCount = 0;

            // 2个注释间 以 @ 分开。

            while(i < len) {

                switch(value[i]) {

                    #region @

                    case '@':

                        // 最后一个 @ 作为普通字符。
                        if(++i == len) {
                            _buffer.Append('@');

                            // 如果后跟着的是符号，作为转义处理。
                        } else if(char.IsPunctuation(value[i])) {

                            switch(value[i]) {
                                case '"':
                                case '\'':
                                    i++;
                                    _buffer.Append("&qote;");
                                    break;
                                case '&':
                                    i++;
                                    _buffer.Append("&amp;");
                                    break;
                                default:
                                    _buffer.Append(value[i++]);
                                    break;
                            }

                        } else {

                            // 如果后面为换行。
                            switch(value[i]) {
                                case '\r':

                                    // 如果下一个字符是 \n ，忽略当前字符。
                                    if(++i == len || value[i] != '\n') {
                                        i--;
                                    }

                                    goto case '\n';

                                case '\n':
                                case '\u2029':

                                    // 按新行处理， 但无需放入缓存。
                                    goto ignoreLineBreak;

                                case '<':
                                    i++;
                                    _buffer.Append("&lt;");
                                    break;
                                case '>':
                                    i++;
                                    _buffer.Append("&gt;");
                                    break;

                                default:

                                    // 假如正在 {。
                                    if(quoteCount > 0) {
                                        _buffer.Append(value[i]);
                                        break;
                                    }

                                    // 遇到了新的标签，先解析上一个标签。
                                    ParseCommentNode();

                                    // 重置状态。
                                    _currentNodeName = ReadWord(value, ref i);
                                    _buffer.Length = 0;
                                    _position = 0;

                                    break;

                            }


                        }

                        break;

                    #endregion

                    #region 换行

                    case '\r':

                        // 如果下一个字符是 \n ，忽略当前字符。
                        if(++i == len || value[i] != '\n') {
                            i--;
                        }

                        goto case '\n';
                    case '\n':
                    case '\u2029':

                        _buffer.Append('\n');

                    ignoreLineBreak:

                        // 读取 \n
                        i++;

                        // 新行。
                        _line++;

                        // 跳过空格。
                        SkipWhiteSpace(value, ref i, ref _line);

                        // 跳过开始的 *
                        if(i < len && value[i] == '*') {

                            // 忽略 *
                            i++;

                            // 继续忽略空格。
                            SkipWhiteSpace(value, ref i, ref _line);


                        }

                        break;



                    #endregion

                    #region 其它

                    case '{':
                        i++;

                        // 如果紧跟的是 @  表示引号添加。
                        if(i < len && value[i] == '@')
                            quoteCount++;
                        break;

                    case '}':
                        i++;
                        if(quoteCount > 0)
                            quoteCount--;
                        break;

                    default:
                        _buffer.Append(value[i++]);
                        break;


                    #endregion

                }
            }

            // 结束上一个 current。
            ParseCommentNode();

            return _currentVariant;
        }

        /// <summary>
        /// 对单一的标签解析。
        /// </summary>
        void ParseCommentNode() {
            _currentNodeName = _currentNodeName.ToLowerInvariant();

            if(_defines[_currentNodeName] != null) {
                _currentNodeName = _defines[_currentNodeName];
            }

            Action action;
            if(!_commentNodes.TryGetValue(_currentNodeName, out action)) {
                action = ParseStringNode;
            }


            action();
        }

        #endregion

        #region 底层处理

        static bool IsWhiteSpace(int c) {
            return char.IsWhiteSpace((char)c);
        }

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

            while(start < sb.Length) {

                if(q == '\0') {
                    switch(sb[start]) {
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

                if(q == ' ') {
                    if(char.IsWhiteSpace(sb[start]) || sb[start] == ']')
                        break;
                } else if(q == sb[start]) {

                    if(q == '`')
                        org++;
                    else
                        start++;
                    break;
                }


                start++;
            }

            if(org >= sb.Length) {
                return String.Empty;
            }

            if(start > sb.Length) {
                start = sb.Length;
            }

            return sb.ToString(org, start - org);
        }

        static string ReadDefaultValue(StringBuilder sb, ref int start) {

            SkipWhiteSpace(sb, ref start);
            if(start < sb.Length && sb[start] == '=') {
                start++;
                SkipWhiteSpace(sb, ref start);
                return ReadLiteral(sb, ref start);
            }

            return null;
        }

        static string ReadWord(StringBuilder sb, ref int start) {
            return ReadAll(sb, ref start, ECMACharType.IsIdentifierPart);
        }

        static void SkipWhiteSpace(StringBuilder sb, ref int start) {
            if(start < sb.Length && char.IsWhiteSpace(sb[start])) {

                while(++start < sb.Length && char.IsWhiteSpace(sb[start])) ;

            }
        }

        static void SkipWhiteSpace(StringBuilder sb, ref int start, ref int currenLine) {
            while(start < sb.Length && char.IsWhiteSpace(sb[start])) {
                CalcLine(sb, ref start, ref currenLine);
                start++;
            }
        }

        static string ReadIfNot(StringBuilder sb, ref int start, Predicate<int> cond) {
            int s = start;
            for(; start < sb.Length && !cond(sb[start]); start++) ;

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
            for(; start < sb.Length && cond(sb[start]); start++) ;

            return sb.ToString(s, start - s);
        }

        static bool IsSeperator(int c) {
            return c == '|' || c == '\\' || c == '/' || c == ',' || c == '}' || c == '\r' || c == '\n' || char.IsWhiteSpace((char)c);
        }

        static void CalcLine(StringBuilder sb, ref int start, ref int currentLine) {
            // 如果后面为换行。
            switch(sb[start]) {
                case '\r':
                    if(++start == sb.Length || sb[start] != '\n')
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
            if(p == null)
                return p_2;

            string[] r = new string[p.Length + p_2.Length];
            p.CopyTo(r, 0);
            p_2.CopyTo(r, p.Length);

            return r;
        }

        #endregion

        #endregion

        #region 用于解析器的底层函数

        #region 基本封装

        string ReadName(ref int start) {
            return ReadName(_buffer, ref start);
        }

        string ReadDefaultValue(ref int start) {
            return ReadDefaultValue(_buffer, ref start);
        }

        void EnsureEmpty(ref int start) {

            SkipWhiteSpace(ref start);

            if(start < _buffer.Length) {
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

            while(start < --end && IsWhiteSpace(_buffer[end])) ;

            return _buffer.ToString(start, end - start + 1);
        }

        string ReadType(ref int start, bool enableMap = true) {
            return ReadType(_buffer, ref start, enableMap);
        }

        string ReadType(StringBuilder sb, ref int start, bool enableMap = true) {

            SkipWhiteSpace(sb, ref start);

            string type = String.Empty;

            if(start < sb.Length && sb[start] == '{') {

                string tmp;

                do {

                    start++;

                    SkipWhiteSpace(sb, ref start);

                    tmp = ReadIfNot(sb, ref start, IsSeperator);

                    if(enableMap)
                        tmp = _typedefs[tmp] ?? tmp;

                    if(start < sb.Length && sb[start] != '}') {
                        while(++start < sb.Length && sb[start] != '}' && IsSeperator(sb[start])) ;

                        --start;
                    }

                    if(type.Length == 0)
                        type = tmp;
                    else if(tmp.Length > 0)
                        type += '/' + tmp;
                } while(tmp.Length > 0 && start < sb.Length && sb[start] != '}');


                if(start == sb.Length || sb[start] != '}') {
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
            if(start < _buffer.Length && char.IsWhiteSpace(_buffer[start])) {
                while(++start < _buffer.Length && char.IsWhiteSpace(_buffer[start])) ;

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

            if(start < _buffer.Length && _buffer[start] == '[') {
                foundLB = 1;
                start++;
            }

            string name = ReadIfNot(_buffer, ref start, NotName);

            if(foundLB > 0) {
                SkipWhiteSpace(ref start);
                if(start < _buffer.Length && _buffer[start] == ']') {
                    start++;
                    foundLB++;
                }
            }

            string dftValue = ReadDefaultValue(ref start);

            if(foundLB > 0) {
                if(dftValue == null)
                    dftValue = String.Empty;

                if(foundLB == 1) {
                    SkipWhiteSpace(ref start);
                    if(start < _buffer.Length && _buffer[start] == ']')
                        start++;
                    else
                        Error("缺少 ]");
                }
            }

            string summary = ReadText(ref start);

            return FillParamType(new string[4] { type, name, summary, dftValue }, autoCreateParam);


        }

        string[] FillParamType(string[] r, bool autoCreateParam) {
            if(_project.AutoCreateParamComment) {
                if(r[0].Length == 0 && !String.IsNullOrEmpty(r[1])) {

                    if(_paramsTypes.ContainsKey(r[1])) {

                        if(r[0].Length == 0)
                            r[0] = _paramsTypes[r[1]][0];

                        if(r[2].Length == 0)
                            r[2] = _paramsTypes[r[1]][2];
                    }

                    if(r[0].Length == 0) {
                        if(r[1].StartsWith("is") || r[1].StartsWith("has") || r[1].StartsWith("contains") || r[1].StartsWith("can")) {
                            r[0] = "Boolean";
                        } else if(r[1].StartsWith("str")) {
                            r[0] = "String";
                        }

                    }

                } else if(autoCreateParam) {
                    _paramsTypes[r[1]] = r;
                }
            }

            return r;
        }

        #endregion

        //#region 其它处理

        ////void TryReadName(Variant currentVariant, CommentNodeNames node) {
        ////    int start = 0;
        ////    string r = ReadNameAndEnsureEmpty(ref start);

        ////    if (r.Length == 0)
        ////        Error("{0} 标签后缺少合法的标识符。", node);
        ////    else {
        ////        currentVariant[node] = new string[1] { r };
        ////    }
        ////}

        ////void CheckMember(MemberType exits, CommentNodeNames node) {

        ////    if (exits != MemberType.Default) {

        ////        if (exits.ToString() == node.DisplayName)
        ////            Error("使用了重复的标签 {0}。", node.DisplayName);
        ////        else
        ////            Error(" {0} 和 {1} 不能同时使用，因为两者所表示的意义相同。", node.DisplayName, exits.ToString());
        ////    }
        ////}

        ////void TryReadSummary(Variant currentVariant, CommentNodeNames node) {
        ////    string[] r = ReadSummary();
        ////    if (r[0].Length > 0)
        ////        currentVariant[node] = r;
        ////}

        ////bool CheckMember(CommentNodeNames exits, CommentNodeNames node, CommentNodeNames node1, CommentNodeNames node2) {
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

        ////void CheckRedefine(Variant currentVariant, CommentNodeNames node) {
        //    if (!node.CanRedefine && currentVariant.ContainsKey(node))
        //        Error("使用了重复的标签 {0}", node.DisplayName);
        //}

        //#endregion

        #region 底层更新

        void UpdateText(Variant currentVariant, CommentNode1 node, string value) {
            if(currentVariant.ContainsKey(node)) {
                Error("同名标签覆盖: @summary {0} => {1}。", currentVariant[node], value);
            }

            currentVariant[node] = new string[] { value };
        }

        void UpdateName(Variant currentVariant, string name) {

            // 如果有 . 分隔为 memberOf 和 name
            int i = name.LastIndexOf('.');
            if(i > 0 && i < name.Length - 1) {
                string member = name.Substring(0, i);
                if(currentVariant.MemberOf != null) {
                    Error("同名标签覆盖: @memberOf {0} => {1}。", currentVariant.MemberOf, member);
                }
                currentVariant.MemberOf = member;
                name = name.Substring(i + 1);
            }

            if(currentVariant.Name != null) {
                Error("同名标签覆盖: @name {0} => {1}。", currentVariant.Name, name);
            }

            currentVariant.Name = name;
            currentVariant.UpdateNamespaceSetter();
        }

        void UpdateType(Variant currentVariant, string type) {
            if(currentVariant.Type != null) {
                Error("同名标签覆盖: @type {0} => {1}。", currentVariant.Type, type);
            }
            currentVariant.Type = type;
        }

        void UpdateMemberType(Variant currentVariant, MemberType memberType) {
            if(currentVariant.MemberType != MemberType.Default) {
                if(currentVariant.MemberType == memberType) {
                    Warning("重复声明了 @{0}", memberType.ToString().ToLowerInvariant());
                } else {
                    Error("同名标签覆盖: @{0} => @{1}。", currentVariant.MemberType.ToString().ToLowerInvariant(), memberType.ToString().ToLowerInvariant());
                }
            }
            currentVariant.MemberType = memberType;
            currentVariant.UpdateNamespaceSetter();
        }

        #endregion

        #region 底层更新的封装

        void TryUpdateText(Variant currentVariant, CommentNode1 node, ref int start) {
            string text = ReadText(ref start);
            if(text.Length > 0) {
                UpdateText(currentVariant, node, text);
            }
        }

        void TryUpdateName(Variant currentVariant, ref int start) {
            string name = ReadName(ref start);
            if(name.Length > 0) {
                UpdateName(currentVariant, name);
            }
        }

        void TryUpdateType(Variant currentVariant, ref int start) {
            string type = ReadType(ref start);
            if(type.Length > 0) {
                UpdateType(currentVariant, type);
            }
        }

        void TryUpdateDefaultValue(Variant currentVariant, ref int start) {
            string type = ReadDefaultValue(ref start);
            if(type != null) {
                UpdateText(currentVariant, CommentNode1.DefaultValue, type);
            }
        }

        void TryReadNextNode(Variant currentVariant, string currentName) {
            int start = 0;
            string s = ReadName(ref start);
            if(s.Length > 0) {
                _buffer.Remove(0, start);
                if(_commentNodes.ContainsKey(s))
                    ParseCommentTag(currentVariant, _commentNodes[s]);
                else
                    Error("@{0} 后有无效的字符串\"{1}\"", currentName.ToLowerInvariant(), s);
            }
        }

        #endregion

        string ReadToEnd() {
            return _buffer.ToString(_position, _buffer.Length - _position);
        }

        #endregion

    }
}
