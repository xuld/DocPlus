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

        public DocMerger(DocParser parser) {
            _parser = parser;
        }

        /// <summary>
        /// 解析指定的文档数据。
        /// </summary>
        /// <param name="docCommentMap">文档。</param>
        public void Parse(DocComment[] docCommentMap) {
            GetDocMembers(docCommentMap);

            RemoveIgnores();
        }

        /// <summary>
        /// 获取全部可用的成员。
        /// </summary>
        /// <param name="docCommentMap"></param>
        void GetDocMembers(DocComment[] docCommentMap) {

            for(int i = 0; i < docCommentMap.Length; i++) {

                DocComment dc = docCommentMap[i];

                if (dc.Name != null && !dc.Ignore) {

                    string key = dc.FullName;
                    DocComment old;
                    if (_parser.Data.DocComments.TryGetValue(dc.FullName, out old)) {
                        old.Merge(dc);
                    } else {
                        _parser.Data.DocComments[dc.FullName] = dc;
                    }
                }


            }


        }

        /// <summary>
        /// 应用忽略列表。
        /// </summary>
        void RemoveIgnores() {
            foreach(string t in _parser.Ignores) {
                _parser.Data.DocComments.Remove(t);
            }

            _parser.Ignores.Clear();
        }

        public void Process() {

            DocComment[] dcs = new DocComment[_parser.Data.DocComments.Count];

            _parser.Data.DocComments.Values.CopyTo(dcs, 0);


            foreach(DocComment dc in dcs) {
                ProcessSignle(dc);
            }

            _parser.Data.Global.DocComment = new DocComment() { Type = "Object", Name = "window", System =true };

            AutoFill(_parser.Data.Global);
        }

        void ProcessSignle(DocComment dc) {

            Variant p = _parser.Data.Global;
            if(!String.IsNullOrEmpty(dc.MemberOf)) {
                string baseMemberName = null;
                foreach(string memberName in dc.MemberOf.Split('.')) {
                    string newBaseMemberName = baseMemberName + memberName;
                    if(!p.ContainsKey(memberName)) {

                        // 生成父对象。
                        p[memberName] = new Variant();

                        DocComment dc2;
                        if(!_parser.Data.DocComments.TryGetValue(newBaseMemberName, out dc2)) {
                            dc2 = new DocComment() { Type = "Object", Name = memberName, MemberOf = baseMemberName, System = true };
                            _parser.Data.DocComments[newBaseMemberName] = dc2;
                        }

                        p[memberName].DocComment = dc2;
                    }

                    p = p[memberName];

                    baseMemberName = newBaseMemberName + ".";
                }
            }

            if(dc.IsMember) {
                if(!p.ContainsKey("prototype")) {
                    p["prototype"] = new Variant() { DocComment = new DocComment() { Type = "Object", Name = "prototype", MemberOf = dc.MemberOf, System = true, MemberType = "namespace" } };
                }

                p = p["prototype"];
            }

            if(!p.ContainsKey(dc.Name)) {
               p[dc.Name] = new Variant();
            }
            p[dc.Name].DocComment = dc;
        }

        void AutoFill(Variant variant) {

            DocComment dc = variant.DocComment;
            if(dc == null)
                return;
            dc.Variant = variant;

            string memberType = dc.MemberType;
            
            if(String.IsNullOrEmpty(memberType)) {
                if(dc.Type == "Function" || dc[NodeNames.Param] != null || dc[NodeNames.Returns] != null) {
                    dc.MemberType = memberType = dc.IsMember ? "method" : "function";
                } else if(variant.Count > 0 && variant.Members != null) {
                    dc.MemberType = "class";
                } else if(variant.Count > 0 ) {
                    dc.MemberType = "namespace";
                } else {
                    dc.MemberType = "field";
                }
            } else if(memberType == "member"){
                if(dc.Type == "Function" || dc[NodeNames.Param] != null || dc[NodeNames.Returns] != null) {
                    dc.MemberType = memberType = dc.IsMember ? "method" : "function";
                } else {
                    dc.MemberType = "field";
                }
            } else if(memberType == "getter" || memberType == "setter"){
                 dc.MemberType = "property";
                 dc[NodeNames.PropertyAttribute] = memberType == "getter" ? "get" : "set";
            }

            if(memberType == "function" || memberType == "method") {
                
                if(dc.Type == "Function")
                    dc.Remove(NodeNames.Type);


                if (dc.Name == "constructor") {
                    dc.MemberType = "constructor";
                }
            } else if (memberType == "class" && dc.MemberAttribute == "static") {
                dc.Remove(NodeNames.MemberAttribute);
            }

            foreach(var vk in variant) {
                AutoFill(vk.Value);
            }
        }
    }
}
