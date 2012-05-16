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
        /// 当前解析所属的解析器。
        /// </summary>
        DocParser _parser;

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
        DocComment _currentComment;

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
            _parser.Project.ProgressReporter.Error(_parser.CurrentSource, _line, message, 0);
        }

        void Warning(string message) {
            _parser.Project.ProgressReporter.Warning(_parser.CurrentSource, _line, message, 0);
        }

        void Error(string message, params object[] args) {
            _parser.Project.ProgressReporter.Error(_parser.CurrentSource, _line, String.Format(message, args), 0);
        }

        void Warning(string message, params object[] args) {
            _parser.Project.ProgressReporter.Warning(_parser.CurrentSource, _line, String.Format(message, args), 0);
        }

        void Redefined() {
            Error("重复的标签: @{0}", _currentNodeName);
        }

        /// <summary>
        /// 提示用户标签重复定义。
        /// </summary>
        /// <param name="nodeNameBefore">发生冲突的标签名。</param>
        void Redefined(string nodeNameBefore) {
            if(nodeNameBefore == _currentNodeName) {
                Error("重复的标签: @{0}", _currentNodeName);
            } else {
                Error("标签 @{0} 和 @{1} 不能同时使用，已忽略标签 {1}", nodeNameBefore, _currentNodeName);
            }
        }

        void MissContent() {
            Error("@{0} 标签之后需要内容", _currentNodeName);
        }

        /// <summary>
        /// 默认的不支持标签的声明。
        /// </summary>
        /// <param name="currentVariant">当前解析的目标变量。</param>
        /// <param name="node">正在解析的标签。</param>
        void NotSupport() {
            Warning("本版本不支持标签 @{0}", _currentNodeName);
        }

        #endregion

        #region 必备的函数

        /// <summary>
        /// 初始化 <see cref="DocPlus.DocParser.Javascript.JavaCommentParser"/> 的新实例。
        /// </summary>
        public JavaCommentParser(DocParser parser) {
            _parser = parser;
            Init();
        }

        /// <summary>
        /// 解析一个注释节点
        /// </summary>
        /// <param name="token">注释内容。</param>
        /// <returns>文档注释。</returns>
        public DocComment Parse(TokenInfo token) {

            #region 更新解析器状态

            // 记录行数，方便出错时提示错误。
            _line = token.StartLocation.Line;

            // 创建新的文档注释。
            _currentComment = new DocComment(token.StartLocation, token.EndLocation);

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

            return _currentComment;
        }

        /// <summary>
        /// 对单一的标签解析。
        /// </summary>
        void ParseCommentNode() {
            string currentNodeName = _currentNodeName.ToLowerInvariant();

            if(_defines[currentNodeName] != null) {
                _currentNodeName = _defines[currentNodeName];
                currentNodeName = _currentNodeName.ToLowerInvariant();
            }

            Action action;
            if(!_commentNodes.TryGetValue(currentNodeName, out action)) {
                action = ParseString;
            }


            action();
        }

        void ParseNext() {
            string value = ReadWord();
            if(value != null) {
                _currentNodeName = value;
                ParseCommentNode();
            }
        }

        #endregion

        #region 底层处理

        static bool IsWhiteSpace(int c) {
            return char.IsWhiteSpace((char)c);
        }

        static bool IsSeperator(int c) {
            return c == '|' || c == '\\' || c == '/' || c == ',' || c == '}' || c == '\r' || c == '\n' || char.IsWhiteSpace((char)c);
        }

        static bool IsNoneName(int i) {
            return !ECMACharType.IsIdentifierPart(i) && i != '.' && i != '#';
        }

        /// <summary>
        /// 读取名字。
        /// </summary>
        /// <returns></returns>
        static string ReadName(StringBuilder sb, ref int start) {

            // [DisplayName]

            SkipWhiteSpace(sb, ref start);

            return ReadIfNot(sb, ref start, IsNoneName);
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

        static string ReadDefaultValue(StringBuilder sb, ref int start) {

            SkipWhiteSpace(sb, ref start);
            if(start < sb.Length && sb[start] == '=') {
                start++;
                SkipWhiteSpace(sb, ref start);
                return ReadLiteral(sb, ref start);
            }

            return null;
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

        static string ReadWord(StringBuilder sb, ref int start) {
            return ReadIf(sb, ref start, ECMACharType.IsIdentifierPart);
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
        static string ReadIf(StringBuilder sb, ref int start, Predicate<int> cond) {

            int s = start;
            for(; start < sb.Length && cond(sb[start]); start++) ;

            return sb.ToString(s, start - s);
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

        string ReadName() {
            return ReadName(_buffer, ref _position);
        }

        string ReadDefaultValue() {
            return ReadDefaultValue(_buffer, ref _position);
        }

        string ReadText() {
            SkipWhiteSpace();

            int end = _buffer.Length;

            while(_position < --end && IsWhiteSpace(_buffer[end])) ;

            string value = _buffer.ToString(_position, end - _position + 1);
            _position = _buffer.Length;

            return value;
        }

        string ReadType(bool enableMap = true) {
            return ReadType(_buffer, ref _position, enableMap);
        }

        string ReadWord() {
            return ReadIf(_buffer, ref _position, ECMACharType.IsIdentifierPart);
        }

        string ReadNameAndEnsureEmpty() {

            string r = ReadName();

            EnsureEmpty();

            return r;

        }

        void EnsureEmpty() {

            SkipWhiteSpace();

            if(_position < _buffer.Length) {
                Error("检测到多余的字符:" + _buffer.ToString(_position, _buffer.Length - _position));
            }

        }

        void SkipWhiteSpace() {
            if(_position < _buffer.Length && char.IsWhiteSpace(_buffer[_position])) {
                while(++_position < _buffer.Length && char.IsWhiteSpace(_buffer[_position])) ;

            }
        }

        #endregion

        #region 底层更新

        void UpdateString(string nodeName, string value) {
            if(_currentComment[nodeName] != null) {
                Redefined();
            }

            if (value != null && value.Length > 0) {
                _currentComment[nodeName] = value;
            }
        }

        void UpdateName(string nodeName, string value) {

            // 如果有 . 分隔为 memberOf 和 name
            int i = value.LastIndexOf('.');
            if(i > 0 && i < value.Length - 1) {
                string memberOf = value.Substring(0, i);
                UpdateString("memberOf", memberOf);
                value = value.Substring(i + 1);
            }

            UpdateString(nodeName, value);
        }

        void UpdateMemberType(string nodeName) {
            string memberType = (string)_currentComment[NodeNames.MemberType];
            if(memberType != null) {
                Redefined(memberType);
            }
            _currentComment[NodeNames.MemberType] = nodeName == "member" ? null : nodeName;
        }

        #endregion

        #region 底层更新的封装

        void TryUpdateString(string nodeName) {
            UpdateString(nodeName, ReadText());
        }

        void TryUpdateName() {
            UpdateName(NodeNames.Name, ReadName());
        }

        void TryUpdateType() {
            UpdateString(NodeNames.Type, ReadType());
        }

        void TryUpdateDefaultValue() {
            UpdateString(NodeNames.Value, ReadDefaultValue());
        }

        #endregion

        #endregion

    }
}
