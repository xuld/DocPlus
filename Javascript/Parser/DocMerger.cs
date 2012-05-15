using System;
using System.Collections.Generic;
using System.Text;
using DocPlus.Core;
using System.IO;
using CorePlus.Parser.Javascript;
using System.Collections.Specialized;

namespace DocPlus.Javascript {

    /// <summary>
    /// 分析变量，解析得到 <see cref="ApiDoc"/> 实例。
    /// </summary>
    public class DocMerger {

        /// <summary>
        /// 正在处理的项目配置。
        /// </summary>
        DocProject _project;

        public DocMerger(DocProject project) {
            _project = project;
        }

        /// <summary>
        /// 解析一个变量树。
        /// </summary>
        /// <param name="scope"></param>
        public void Parse(Variant scope, NameValueCollection value) {

            scope.Remove("this");
            scope.Remove("window");
            ParseChildren(scope, null);
        }

        void ParseChildren(Variant obj, bool? isStatic) {


        }
    }
}
