using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocPlus.Javascript {
    public sealed partial class JavaCommentParser {

        /// <summary>
        /// 初始化系统定义的标签。
        /// </summary>
        void Init() {

            // 全局标签
            _commentNodes["author"] = ParseGlobalNode;
            _commentNodes["projectdescription"] = ParseGlobalNode;
            _commentNodes["license"] = ParseGlobalNode;
            _commentNodes["fileoverview"] = ParseGlobalNode;
            _commentNodes["copyright"] = ParseGlobalNode;
            _commentNodes["file"] = ParseGlobalNode;
            _commentNodes["version"] = ParseGlobalNode;

            _commentNodes["progma"] = ParseProgma;
            _commentNodes["define"] = ParseDefine;
            _commentNodes["include"] = ParseInclude;

            // 特殊处理的标签

            _commentNodes["class"] = ParseMemberType;
            _commentNodes["enum"] = ParseMemberType;
            _commentNodes["interface"] = ParseMemberType;
            _commentNodes["interface"] = ParseMemberType;
            _commentNodes["constructor"] = ParseMemberType;
            _commentNodes["member"] = ParseMemberType;
            _commentNodes["method"] = ParseMemberType;
            _commentNodes["event"] = ParseMemberType;
            _commentNodes["property"] = ParseTypedMemberType;
            _commentNodes["getter"] = ParseTypedMemberType;
            _commentNodes["setter"] = ParseTypedMemberType;
            _commentNodes["field"] = ParseTypedMemberType;
            _commentNodes["config"] = ParseTypedMemberType;

            _commentNodes["namespace"] = ParseNamespace;
            _commentNodes["ignore"] = ParseIgnore;
            _commentNodes["system"] = ParseSystem;
            _commentNodes["memberof"] = ParseMemberOf;

            _commentNodes["name"] = ParseName;
            _commentNodes["type"] = ParseType;

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

            _commentNodes["summary"] = ParseRichText;
            _commentNodes["remark"] = ParseRichText;
            _commentNodes["example"] = ParseRichText;
            _commentNodes["syntax"] = ParseRichText;

            _commentNodes["alias"] = ParseName;
            _commentNodes["defaultvalue"] = ParseString;
            _commentNodes["override"] = ParseString;
            _commentNodes["since"] = ParseString;
            _commentNodes["deprecated"] = ParseString;

            _commentNodes["extends"] = ParseExtends;
            _commentNodes["implements"] = ParseImplements;

            _commentNodes["exception"] = ParseException;
            _commentNodes["param"] = ParseParam;
            _commentNodes["returns"] = ParseReturn;

            _commentNodes["see"] = ParseSee;
            _commentNodes["seeAlso"] = ParseSee;

            _commentNodes["requires"] = ParseRequires;


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

        #region 通用

        void ParseString() {
            string value = ReadText();
            if (value.Length > 0) {
                UpdateString(_currentNodeName, value);
            }
        }

        MarkdownSharp.Markdown _md = new MarkdownSharp.Markdown();

        void ParseRichText() {
            string value = ReadText();
            if (value.Length > 0) {
                value = _md.Transform(value);
                UpdateString(_currentNodeName, value);
            }
        }

        /// <summary>
        /// 解析全局性质的标签。
        /// </summary>
        /// <param name="currentVariant">当前解析的目标变量。</param>
        /// <param name="node">正在解析的标签。</param>
        void ParseGlobalNode() {
            string summary = ReadText();
            _parser.Data.Properties[_currentNodeName] = summary;
        }

        #endregion

        #region 特殊

        void ParseMemberAttribute() {
            string memberAttribute = (string)_currentComment[NodeNames.MemberAttribute];
            if(memberAttribute == null) {
                memberAttribute = _currentNodeName;
            } else if((memberAttribute == "static" && _currentNodeName == "readonly") || (memberAttribute == "readonly" && _currentNodeName == "static")) {
                memberAttribute = "static readonly";
            } else if((memberAttribute == "static" && _currentNodeName == "const") || (memberAttribute == "const" && _currentNodeName == "static")) {
                memberAttribute = "static const";
            } else {
                Redefined(memberAttribute);
                memberAttribute = _currentNodeName;
            }
            _currentComment[NodeNames.MemberAttribute] = memberAttribute;

            ParseNext();
        }

        void ParseMemberAccess() {
            string memberAccess = (string)_currentComment[NodeNames.MemberAccess];
            if(memberAccess == null) {
                memberAccess = _currentNodeName;
            } else if((memberAccess == "protected" && _currentNodeName == "internal") || (memberAccess == "internal" && _currentNodeName == "protected")) {
                memberAccess = "protected internal";
            } else {
                Redefined(memberAccess);
                memberAccess = _currentNodeName;
            }
            _currentComment[NodeNames.MemberAccess] = memberAccess;

            ParseNext();
        }

        void ParseMemberType() {
            UpdateMemberType(_currentNodeName);
            TryUpdateName();
            TryUpdateString(NodeNames.Summary);
        }

        void ParseTypedMemberType() {
            UpdateMemberType(_currentNodeName);
            TryUpdateType();
            TryUpdateName();
            TryUpdateDefaultValue();
            TryUpdateString(NodeNames.Summary);
        }

        void ParseParam() {

            string type = ReadType();
            
            SkipWhiteSpace();

            byte foundLB = 0;

            if(_position < _buffer.Length && _buffer[_position] == '[') {
                foundLB = 1;
                _position++;
            }

            string name = ReadIfNot(_buffer, ref _position, IsNoneName);

            if(foundLB > 0) {
                SkipWhiteSpace();
                if(_position < _buffer.Length && _buffer[_position] == ']') {
                    _position++;
                    foundLB++;
                }
            }

            string dftValue = ReadDefaultValue();

            if(foundLB > 0) {
                if(dftValue == null)
                    dftValue = String.Empty;

                if(foundLB == 1) {
                    SkipWhiteSpace();
                    if(_position < _buffer.Length && _buffer[_position] == ']')
                        _position++;
                    else
                        Error("@param 可选参数定义缺少 ]");
                }
            }

            string summary = ReadText();

            if(_parser.Project.AutoCreateParamComment && name.Length > 0) {

                if(type.Length == 0) {
                    if(_typedefs[name] != null) {
                        type = _typedefs[name];
                    } else if(name.StartsWith("is") || name.StartsWith("has") || name.StartsWith("can")) {
                        type = "Boolean";
                    } else if(name.StartsWith("str")) {
                        type = "String";
                    } else if(name.StartsWith("obj")) {
                        type = "Object";
                    } else if(name.StartsWith("fn")) {
                        type = "Function";
                    }

                } else if(_typedefs[name] == null) {
                    _typedefs[name] = type;
                }

            }

            ParamInfoCollection p = (ParamInfoCollection)(_currentComment[NodeNames.Param] ?? (_currentComment[NodeNames.Param] = new ParamInfoCollection()));

            p.Add(name, type, summary, dftValue);
        }

        void ParseNamespace() {
            _currentComment.NamespaceSetter = ReadNameAndEnsureEmpty();
        }

        void ParseRequires() {
          //  string[] requires = _currentComment[NodeNames.Requires];
            ParseGlobalNode();
        }

        void ParseException() {
            AddArrayProxy(new TypeSummary() {
                Type = ReadType(),
                Summary = ReadText()
            });
        }

        void ParseMemberOf() {
            string value = ReadNameAndEnsureEmpty();
            if (_currentComment[_currentNodeName] != null) {
                Redefined(_currentNodeName);
            }

            if (value != null && value.Length > 0) {
                _currentComment.MemberOf = value;
            }
        }

        void ParseReturn() {

            if(_currentComment[NodeNames.Returns] != null) {
                Redefined(NodeNames.Returns);
            }

            _currentComment[NodeNames.Returns] = new TypeSummary() {
                Type = ReadType(),
                Summary = ReadText()
            };

        }

        void ParseLink() {
            _currentComment[_currentNodeName] = ReadText();
        }

        void ParseImplements() {
            AddArrayProxy(ReadNameAndEnsureEmpty());
        }

        void ParseExtends() {
            UpdateString(_currentNodeName, ReadNameAndEnsureEmpty());
        }

        void ParseIgnore() {
            string value = ReadNameAndEnsureEmpty();
            if(value.Length == 0)
                _currentComment.Ignore = true;
            else {
                _parser.Ignores.Add(value);
            }
        }

        void ParseSystem() {
            _currentComment.System = true;
        }

        void ParseConstructor() {
            _currentComment.AutoFill(NodeNames.MemberType, "class");
        }

        void ParseName() {
            string value = ReadNameAndEnsureEmpty();
            if(value.Length > 0) {
                UpdateName(_currentNodeName, value);
            } else {
                MissContent();
            }
        }

        void ParseType() {
            SkipWhiteSpace();
            if(_position == _buffer.Length || _buffer[_position] != '{') {
                if(_position > 0)
                    _buffer[--_position] = '{';
                else
                    _buffer.Insert(0, '{');

                _buffer.Append('}');
            }

            string value = ReadType();
            if(value.Length > 0) {
                EnsureEmpty();

                UpdateString(NodeNames.Type, value);
            } else {
                MissContent();
            }
        }

        void ParseSee() {
            AddArrayProxy(ReadNameAndEnsureEmpty());
        }

        void ParseDefine() {
            SkipWhiteSpace();

            if(_position < _buffer.Length) {
                switch(_buffer[_position]) {
                    case '{':
                        string fromType = ReadType(false);
                        string toType = ReadType(false);

                        if(toType.Length > 0)
                            _typedefs[fromType] = toType;
                        else
                            Error("@define 需要第2个参数");
                        break;

                    default:
                        SkipWhiteSpace();
                        string fromWord = ReadWord();
                        SkipWhiteSpace();
                        string toWord = ReadWord();

                        _defines[fromWord] = toWord;
                        break;
                }

                EnsureEmpty();
            } else {
                Error("@define 后面缺少内容");
            }
        }

        void ParseProgma() {
            NotSupport();
        }

        void ParseInclude() {
            NotSupport();
        }

        #endregion

    }

}
