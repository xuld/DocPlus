using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace DocPlus.Javascript {

    /// <summary>
    /// 表示一个变量对象。
    /// </summary>
    public class Variant : Dictionary<string, Variant> {

        /// <summary>
        /// 获取或设置此变量的注释。
        /// </summary>
        public DocComment DocComment {
            get;
            set;
        }

        /// <summary>
        /// 获取当前变量下的子成员。
        /// </summary>
        public Variant Members {
            get {
                Variant v;
                return TryGetValue("prototype", out v) ? v : null;
            }
        }

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// 	<see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString() {
            return DocComment == null ? base.ToString() : DocComment.ToString();
        }
    }
}
