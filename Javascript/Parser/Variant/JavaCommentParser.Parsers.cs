using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocPlus.Javascript {
    public sealed partial class JavaCommentParser {

        /// <summary>
        /// 初始化系统定义的标签。
        /// </summary>
        void InitNodes() {

            //// 全局标签
            //AddSystemTag(CommentNode1.Author, ParseGlobalTags);
            //AddSystemTag(CommentNode1.ProjectDescription, ParseGlobalTags);
            //AddSystemTag(CommentNode1.License, ParseGlobalTags);
            //AddSystemTag(CommentNode1.FileOverview, ParseGlobalTags);
            //AddSystemTag(CommentNode1.Copyright, ParseGlobalTags);
            //AddSystemTag(CommentNode1.File, ParseGlobalTags);
            //AddSystemTag(CommentNode1.Version, ParseGlobalTags);
            //AddSystemTag(CommentNode1.Progma, ParseProgma);
            //AddSystemTag(CommentNode1.Define, ParseDefine);
            ////  AddSystemTag(CommentNodeNames.Include);

            //// 特殊处理的标签

            //AddSystemTag(CommentNode1.Class, (v, c) => ParseMemberType(v, MemberType.Class));
            //AddSystemTag(CommentNode1.Enum, (v, c) => ParseMemberType(v, MemberType.Enum));
            //AddSystemTag(CommentNode1.Interface, (v, c) => ParseMemberType(v, MemberType.Interface));
            //AddSystemTag(CommentNode1.Member, (v, c) => ParseMemberType(v, MemberType.Default));
            //AddSystemTag(CommentNode1.Method, (v, c) => ParseMemberType(v, MemberType.Method));
            //AddSystemTag(CommentNode1.Event, (v, c) => ParseTypedMemberType(v, MemberType.Event));
            //AddSystemTag(CommentNode1.Property, (v, c) => ParseTypedMemberType(v, MemberType.Property));
            //AddSystemTag(CommentNode1.Field, (v, c) => ParseTypedMemberType(v, MemberType.Field));
            //AddSystemTag(CommentNode1.Getter, (v, c) => ParseTypedMemberType(v, MemberType.PropertyGetter));
            //AddSystemTag(CommentNode1.Setter, (v, c) => ParseTypedMemberType(v, MemberType.PropertySetter));
            //AddSystemTag(CommentNode1.Config, (v, c) => ParseTypedMemberType(v, MemberType.Config));

            //AddSystemTag(CommentNode1.Namespace, ParseNamespace);
            //AddSystemTag(CommentNode1.Constructor, ParseConstructor);
            //AddSystemTag(CommentNode1.Ignore, ParseIgnore);
            //AddSystemTag(CommentNode1.System, ParseSystem);
            //AddSystemTag(CommentNode1.Hide, ParseHide);
            //AddSystemTag(CommentNode1.MemberOf, ParseMemberOf);
            //AddSystemTag(CommentNode1.NameNode, ParseName);
            //AddSystemTag(CommentNode1.TypeNode, ParseType);

            _commentNodes["public"] = ParseMemberAccess;
            _commentNodes["private"] = ParseMemberAccess;
            _commentNodes["protected"] = ParseMemberAccess;
            _commentNodes["internal"] = ParseMemberAccess;

            _commentNodes["final"] = ParseMemberAttribute;
            _commentNodes["static"] = ParseMemberAttribute;
            _commentNodes["abstract"] = ParseMemberAttribute;
            _commentNodes["virtual"] = ParseMemberAttribute;
            _commentNodes["const"] = ParseMemberAttribute;
            _commentNodes["readonly"] = ParseMemberAttribute;
            _commentNodes["override"] = ParseMemberAttribute;

            //// 通用的标签

            //AddSystemTag(CommentNode1.Summary, ParseText);
            //AddSystemTag(CommentNode1.Remark, ParseMultiText);
            //AddSystemTag(CommentNode1.Example, ParseText);
            //AddSystemTag(CommentNode1.Syntax, ParseText);
            //AddSystemTag(CommentNode1.Category, ParseText);

            //AddSystemTag(CommentNode1.Alias, ParseMultiName);
            //AddSystemTag(CommentNode1.DefaultValue, ParseText);

            //AddSystemTag(CommentNode1.Extends, ParseExtendsAndImplements);
            //AddSystemTag(CommentNode1.Implements, ParseExtendsAndImplements);
            //AddSystemTag(CommentNode1.Exception, ParseException);
            //AddSystemTag(CommentNode1.Param, ParseParams);
            //AddSystemTag(CommentNode1.Return, ParseReturn);

            //AddSystemTag(CommentNode1.See, ParseMultiLink);
            //AddSystemTag(CommentNode1.SeeAlso, ParseMultiLink);
            //AddSystemTag(CommentNode1.Requires, ParseRequires);

            //AddSystemTag(CommentNode1.Since, ParseText);
            //AddSystemTag(CommentNode1.Deprecated, ParseText); ;

            //  AddSystemTag(CommentNodeNames.Throws);



            //  AddSystemTag(CommentNodeNames.Code, NotSupport);


            _typedefs["bool"] = "Boolean";
            _typedefs["boolean"] = "Boolean";
            _typedefs["int"] = "Integer";
            _typedefs["integer"] = "Integer";
            _typedefs["number"] = "Number";
            _typedefs["num"] = "Number";
            _typedefs["float"] = "Number";
            _typedefs["double"] = "Number";
            _typedefs["single"] = "Number";
            _typedefs["decimal"] = "Integer";
            _typedefs["string"] = "String";
            _typedefs["function"] = "Function";
            _typedefs["json"] = "Json";
            _typedefs["array"] = "Array";
            _typedefs["object"] = "Object";
            _typedefs["date"] = "Date";

            _defines["return"] = "returns";
            _defines["implement"] = "implements";

            _defines["extend"] = "extends";
            _defines["inherits"] = "extends";
            _defines["augments"] = "extends";

            _defines["params"] = "param";
            _defines["args"] = "param";
            _defines["arguments"] = "param";
            _defines["argument"] = "param";

            _defines["group"] = "category";
            _defines["throws"] = "exception";
            _defines["throw"] = "exception";

            _defines["inner"] = "internal";
            _defines["singleton"] = "static";
            _defines["lends"] = "memberof";
            _defines["description"] = "summary";

            _defines["value"] = "defaultvalue";
            _defines["default"] = "defaultvalue";

            _defines["constant"] = "const";
            _defines["sealed"] = "final";
            _defines["obsolete"] = "deprecated";

            _paramsTypes["node"] = "HTMLNode";
            _paramsTypes["elem"] = "HTMLElement";
            _paramsTypes["el"] = "HTMLElement";
            _paramsTypes["fn"] = "Function";
            _paramsTypes["date"] = "Date";
            _paramsTypes["func"] = "Function";
            _paramsTypes["attr"] = "String";
            _paramsTypes["options"] = "Object";
            _paramsTypes["opt"] = "Object";
            _paramsTypes["obj"] = "Object";
            _paramsTypes["checker"] = "Function";
            _paramsTypes["arr"] = "Array";
            _paramsTypes["str"] = "String";
            _paramsTypes["callback"] = "Function";
            _paramsTypes["id"] = "String";
        }

        #region 真正的标签解析逻辑

        #region 全局标签

        /// <summary>
        /// 解析全局性质的标签。
        /// </summary>
        /// <param name="currentVariant">当前解析的目标变量。</param>
        /// <param name="node">正在解析的标签。</param>
        void ParseGlobalTags(Variant currentVariant, CommentNode1 node) {
            int start = 0;
            string summary = ReadText(ref start);
            if(_globalConfigs.ContainsKey(node)) {
                Error("同名标签覆盖: @{0} {1} => {2}", node.Name, _globalConfigs[node], summary);
            }
            _globalConfigs[node] = summary;
        }

        void ParseProgma(Variant currentVariant, CommentNode1 node) {

        }

        void ParseDefine(Variant currentVariant, CommentNode1 node) {
            int start = 0;
            SkipWhiteSpace(ref start);

            if(start < _buffer.Length) {
                switch(_buffer[start]) {
                    case '{':
                        string fromType = ReadType(ref start, false);
                        string toType = ReadType(ref start, false);

                        if(toType.Length > 0)
                            _typedefs[fromType] = toType;
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
            TryUpdateText(currentVariant, CommentNode1.Summary, ref start);
            UpdateMemberType(currentVariant, memberType);
        }

        void ParseTypedMemberType(Variant currentVariant, MemberType memberType) {
            int start = 0;
            TryUpdateType(currentVariant, ref start);
            TryUpdateName(currentVariant, ref start);
            TryUpdateDefaultValue(currentVariant, ref start);
            TryUpdateText(currentVariant, CommentNode1.Summary, ref start);
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

        void ParseText(Variant currentVariant, CommentNode1 node) {
            int start = 0;
            TryUpdateText(currentVariant, node, ref start);
        }

        void ParseNamespace(Variant currentVariant, CommentNode1 node) {
            int start = 0;
            string name = ReadName(ref start);
            if(name.Length > 0) {
                //if (currentVariant.NamespaceSetter != null) {
                //    Error("同名标签覆盖: @namespace {1} => {2}", currentVariant.NamespaceSetter, name);
                //}
                if(currentVariant.Name == null)
                    currentVariant.Name = name;

                currentVariant.NamespaceSetter = name;
            }
        }

        void ParseName(Variant currentVariant, CommentNode1 node) {
            int start = 0;
            string name = ReadText(ref start);
            if(name.Length > 0) {
                UpdateName(currentVariant, name);
            }
        }

        void ParseType(Variant currentVariant, CommentNode1 node) {
            int start = 0;
            SkipWhiteSpace(ref start);
            if(start == _buffer.Length || _buffer[start] != '{') {
                if(start > 0)
                    _buffer[--start] = '{';
                else
                    _buffer.Insert(0, '{');

                _buffer.Append('}');
            }

            string type = ReadType(ref start);
            if(type.Length > 0) {
                EnsureEmpty(ref start);

                if(currentVariant.Type != null) {
                    Error("同名标签覆盖: @type {1} => {2}", currentVariant.Type, type);
                }

                currentVariant.Type = type;
            } else {
                Error("需要类型");
            }
        }

        void ParseRequires(Variant currentVariant, CommentNode1 node) {
            currentVariant[node] = Concat(currentVariant[node], ReadLink());
        }

        void ParseException(Variant currentVariant, CommentNode1 node) {
            if(currentVariant.Exceptions == null) {
                currentVariant.Exceptions = new List<string[]>();
            }
            currentVariant.Exceptions.Add(ReadReturn());
        }

        void ParseParams(Variant currentVariant, CommentNode1 node) {
            if(currentVariant.Params == null) {
                currentVariant.Params = new List<string[]>();
            }
            currentVariant.Params.Add(ReadParams(currentVariant.Summary == null));
        }

        void ParseMemberOf(Variant currentVariant, CommentNode1 node) {
            int start = 0;
            string name = ReadName(ref start);
            if(name.Length > 0) {
                if(currentVariant.MemberOf != null) {
                    Error("同名标签覆盖: @namespace {1} => {2}", currentVariant.NamespaceSetter, name);
                }

                currentVariant.MemberOf = name;
            }
        }

        void ParseReturn(Variant currentVariant, CommentNode1 node) {
            int start = 0;

            string type = ReadType(ref start);

            string summary = ReadText(ref start);

            if(type.Length > 0) {
                if(currentVariant.ReturnType != null) {
                    Error("同名标签覆盖: @return {{{1}}} => {{{2}}}", currentVariant.ReturnType, type);
                }

                currentVariant.ReturnType = type;
            }

            if(summary.Length > 0)
                currentVariant.ReturnSummary = summary;
        }

        void ParseLink(Variant currentVariant, CommentNode1 node) {
            currentVariant[node] = Concat(currentVariant[node], ReadLink());
        }

        void ParseExtendsAndImplements(Variant currentVariant, CommentNode1 node) {
            if(currentVariant.MemberType == MemberType.Default)
                currentVariant.MemberType = MemberType.Class;
            ParseMultiName(currentVariant, node);
        }

        void ParseMultiName(Variant currentVariant, CommentNode1 node) {
            string[] r = ReadName();
            if(!node.AllowEmpty && r[0].Length == 0)
                Error("{0} 后面不能为空。", node);
            else
                currentVariant[node] = Concat(currentVariant[node], r);
        }

        void ParseMultiText(Variant currentVariant, CommentNode1 node) {
            int start = 0;
            string r = ReadText(ref start);
            if(currentVariant.ContainsKey(node)) {
                currentVariant[node][0] += Environment.NewLine + r;
            } else {
                currentVariant[node] = new string[] { r };
            }
        }

        void ParseMultiLink(Variant currentVariant, CommentNode1 node) {
            string[] r = ReadLink();
            if(!node.AllowEmpty && r[0].Length == 0)
                Error("{0} 后面不能为空。", node);
            else
                currentVariant[node] = Concat(currentVariant[node], r);
        }

        void ParseIgnore(Variant currentVariant, CommentNode1 node) {
            int start = 0;
            string r = ReadNameAndEnsureEmpty(ref start);
            if(r.Length == 0)
                currentVariant.Ignore = true;
            else {
                if(_globalConfigs.Ignores == null) {
                    _globalConfigs.Ignores = new List<string>();
                }

                _globalConfigs.Ignores.Add(r);
            }
        }

        void ParseSystem(Variant currentVariant, CommentNode1 node) {
            currentVariant.System = true;
        }

        void ParseHide(Variant currentVariant, CommentNode1 node) {
            currentVariant.Hide = true;
        }

        void ParseConstructor(Variant currentVariant, CommentNode1 node) {
            int start = 0;
            TryUpdateName(currentVariant, ref start);
            TryUpdateName(currentVariant, ref start);
            TryUpdateText(currentVariant, CommentNode1.Summary, ref start);
            currentVariant.IsConstructor = true;
        }

        #endregion

        #endregion

        #region 通用

        void ParseStringNode() {
            _currentVariant.SetComment(_currentNodeName, new StringCommentNode(ReadToEnd()));
        }

        void ParseRichTextNode() {
            string value = ReadToEnd();
            _currentVariant.SetComment(_currentNodeName, new StringCommentNode(value));
        }

        #endregion

        #region 特殊

        void ParseParamCommentNode() {

        }

        void ParseMemberAttribute() {
            StringCommentNode c = (StringCommentNode)_currentVariant.GetComment(NodeNames.MemberAccess);
            if(c == null) {
                c = new StringCommentNode(_currentNodeName);
                _currentVariant.SetComment(NodeNames.MemberAttribute, c);
            } else if((c.Value == "static" && _currentNodeName == "readonly") || (c.Value == "readonly" && _currentNodeName == "static")) {
                c.Value = "static readonly";
            } else if((c.Value == "static" && _currentNodeName == "const") || (c.Value == "const" && _currentNodeName == "static")) {
                c.Value = "static const";
            } else if(c.Value == _currentNodeName) {
                Error("重复的标签: {0}", _currentNodeName);
            } else {
                Error("标签 {0} 和 {1} 不能同时使用，已忽略标签 {1}", c.Value, _currentNodeName);
            }

            string value = ReadName();
            if(value.Length > 0) {
                _currentNodeName = value;
                ParseCommentNode();
            }
        }

        void ParseMemberAccess() {
            StringCommentNode c = (StringCommentNode)_currentVariant.GetComment(NodeNames.MemberAccess);
            if(c == null) {
                c = new StringCommentNode(_currentNodeName);
                _currentVariant.SetComment(NodeNames.MemberAccess, c);
            } else if((c.Value == "protected" && _currentNodeName == "internal") || (c.Value == "internal" && _currentNodeName == "protected")) {
                c.Value = "protected internal";
            } else if(c.Value == _currentNodeName) {
                Error("重复的标签: @{0}。", _currentNodeName);
            } else {
                c.Value = _currentNodeName;
                Error("同性质的标签覆盖: @{0} => @{1}。", c.Value, _currentNodeName);
            }

            string value = ReadName();
            if(value.Length > 0) {
                _currentNodeName = value;
                ParseCommentNode();
            }
        }

        void ParseMemberType() {
            StringCommentNode c = (StringCommentNode)_currentVariant.GetComment(NodeNames.MemberType);
            if(c == null) {
                c = new StringCommentNode(_currentNodeName);
                _currentVariant.SetComment(NodeNames.MemberAccess, c);
            } else if((c.Value == "protected" && _currentNodeName == "internal") || (c.Value == "internal" && _currentNodeName == "protected")) {
                c.Value = "protected internal";
            } else if(c.Value == _currentNodeName) {
                Error("重复的标签: @{0}。", _currentNodeName);
            } else {
                c.Value = _currentNodeName;
                Error("同性质的标签覆盖: @{0} => @{1}。", c.Value, _currentNodeName);
            }

            string value = ReadName();
            if(value.Length > 0) {
                _currentNodeName = value;
                ParseCommentNode();
            }
        }

        #endregion


        /// <summary>
        /// 默认的不支持标签的声明。
        /// </summary>
        /// <param name="currentVariant">当前解析的目标变量。</param>
        /// <param name="node">正在解析的标签。</param>
        void NotSupport(Variant currentVariant, CommentNode1 node) {
            Error("本版本不支持标签 @{0}", node.Name);
        }
    }
}
