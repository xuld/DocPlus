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

                if(memberType == "Class"){
                    currentNamespace = dc.FullName;
                    currentNamespaceIsClass = !dc.IsStatic;
                } else if(memberType == "Enum" || memberType == "Interface" || memberType == "Namespace"){
                    currentNamespace = dc.FullName;
                    currentNamespaceIsClass = false;

                // 只有不存在 MemberOf 的时候才填充。
                } else if(dc.MemberOf == null){
                    if(currentNamespaceIsClass) {
                        if(dc.IsStatic) {
                            dc.MemberOf = currentNamespace;
                        } else {
                            dc.MemberOf = currentNamespace + ".prototype";
                        }
                    } else {
                        dc.MemberOf = currentNamespace;
                    }
                }

            }


        }

        void SubmitDocComments() {
            

            foreach(DocComment dc in _docCommentMap){
                if(dc.MemberOf != null) {
                    _parser.Data.DocComments[dc.FullName] = dc;
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


            SubmitDocComments();
        }
    }
}
