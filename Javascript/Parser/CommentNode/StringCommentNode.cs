using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocPlus.Javascript {
    public class StringCommentNode : ICommentNode {

        public StringCommentNode(string value) {
            Value = value;
        }

        public string Value {
            get;
            set;
        }

        public string ToJson() {
            return new CorePlus.Json.JsonString(Value).ToString();
        }

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// 	<see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString() {
            return Value;
        }
    }
}
