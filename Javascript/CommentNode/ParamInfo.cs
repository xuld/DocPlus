﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocPlus.Javascript {

    public sealed class ParamInfo {
        public string Name;
        public string Type;
        public string Summary;
        public string DefaultValue;

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// 	<see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString() {
            return String.Concat("{", Type, "} ", Name, DefaultValue == null ? "" : DefaultValue.Length == 0 ? "?" : (" = " + DefaultValue), " ", Summary);
        }
    }
}
