using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace DocPlus.Javascript {

    /// <summary>
    /// 由文档解析器分析得到的关于整个文档的配置属性。
    /// </summary>
    public sealed class GlobalConfigs : NameValueCollection {

        public List<string> Ignores {
            get;
            set;
        }

        public GlobalConfigs() {
            Ignores = new List<string>();
        }
    }
}
