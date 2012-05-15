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
        DocProject _project;

        public DocMerger(DocProject project) {
            _project = project;
        }

        Variant scope;

        /// <summary>
        /// 解析一个变量树。
        /// </summary>
        /// <param name="scope"></param>
        public void Parse(VariantMap variantMap) {

        }

        void ParseChildren(Variant obj, bool? isStatic) {


         //   scope.Remove("this");
         //   scope.Remove("window");
        }
    }
}
