using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using CorePlus.Parser.Javascript;
using DocPlus.Core;

namespace DocPlus.Javascript {

    /// <summary>
    /// 分析变量，解析得到最终的文档。
    /// </summary>
    public class DocMerger {
        
        /// <summary>
        /// 正在处理的项目配置。
        /// </summary>
        DocParser _parser;

        DocComment[] _docCommentMap;

        public DocMerger(DocParser parser) {
            _parser = parser;
        }

        void AutoFillMemberOf() {

            string currentNamespace = "window";
            bool currentNamespaceIsClass = false;

            for(int i = 0; i < _docCommentMap.Length; i++) {

                DocComment dc = _docCommentMap[i];

                string memberType = dc.MemberType;
                bool isType = false;

                if (dc.NamespaceSetter != null) {
                    if (dc.NamespaceSetter.Length == 0) {
                        currentNamespace = dc.FullName;
                    } else {
                        currentNamespace = dc.NamespaceSetter;
                    }
                    currentNamespaceIsClass = false;
                }else if(memberType == "class"){
                    currentNamespace = dc.FullName;
                    currentNamespaceIsClass = !dc.IsStatic;
                    isType = true;
                } else if (memberType == "enum" || memberType == "interface") {
                    currentNamespace = dc.FullName;
                    currentNamespaceIsClass = false;
                    isType = true;

                    // 只有不存在 MemberOf 的时候才填充。
                } else {
                    if (memberType == "member") {
                        if (dc.Type == "Function" || dc[NodeNames.Param] != null || dc[NodeNames.Return] != null) {
                            dc.MemberType = memberType = "method";
                        } else {
                            dc.MemberType = memberType = "field";
                        }
                    }

                    if (dc.MemberOf == null) {
                        string parentNamspace = currentNamespace;
                        bool parentIsClass = currentNamespaceIsClass;
                        if (dc.Parent != null) {
                            parentNamspace = dc.Parent.FullName;
                            parentIsClass = dc.Parent.MemberType == "class";
                        } 
                        
                        if (parentIsClass) {
                            if (dc.IsStatic) {
                                dc.MemberOf = parentNamspace;
                            } else {
                                dc.MemberOf = parentNamspace + ".prototype";
                            }
                        } else {
                            dc.MemberOf = parentNamspace;
                        }
                    }
                }


                if (dc.Name != null) {

                    if (isType || !String.IsNullOrEmpty(dc.MemberOf)) {
                        _parser.Data.DocComments[dc.FullName] = dc;
                    }

                }


            }


        }

        /// <summary>
        /// 解析一个变量树。
        /// </summary>
        /// <param name="scope"></param>
        public void Parse(DocComment[] docCommentMap) {
            _docCommentMap = docCommentMap;
            AutoFillMemberOf();

            // 应用忽略列表。
            foreach(string t in _parser.Ignores) {
                _parser.Data.DocComments.Remove(t);
            }
        }
    }
}
