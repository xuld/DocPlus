using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace DocPlus.Javascript {

    /// <summary>
    /// 表示文档解析之后返回的数据。
    /// </summary>
    public sealed class DocData {

        public NameValueCollection Properties {
            get;
            set;
        }

        public NameValueCollection Files {
            get;
            set;
        }

        public Dictionary<string, DocComment> DocComments {
            get;
            set;
        }

        public Variant Global {
            get;
            set;
        }

        public DocData() {
            Properties = new NameValueCollection();
            Files = new NameValueCollection();
            DocComments = new Dictionary<string, DocComment>();
            Global = new Variant();
        }
    }
}
