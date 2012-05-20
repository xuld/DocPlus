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

            for(int i = 0; i < _docCommentMap.Length; i++) {

                DocComment dc = _docCommentMap[i];

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
