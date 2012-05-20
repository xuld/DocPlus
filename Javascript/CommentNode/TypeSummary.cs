using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocPlus.Javascript {
    public sealed class TypeSummary {

        public string Type {
            get;
            set;
        }

        public string Summary {
            get;
            set;
        }

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// 	<see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString() {
            return Type == null ? Summary : String.Concat("{", Type, "} ", Summary);
        }
    }
}
