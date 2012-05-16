using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocPlus.Javascript {
    public class ReturnCommentNode : ICommentNode {

        public string Type {
            get;
            set;
        }

        public string Summary {
            get;
            set;
        }


        public string ToJson() {
            return "\"return\"";
        }

        public override string ToString() {
            return String.Concat("{", Type, "} ", Summary);
        }
    }
}
