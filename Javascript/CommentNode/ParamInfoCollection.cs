﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocPlus.Javascript {
    public class ParamInfoCollection : List<ParamInfo> {

        public void Add(string name, string type = null, string summary = null, string defaultValue = null) {

            foreach(var p in this) {
                if(p.Name == name) {
                    p.Type = type ?? p.Type;
                    p.Summary = summary ?? p.Summary;
                    p.DefaultValue = defaultValue ?? p.DefaultValue;
                }
            }

            Add(new ParamInfo() {
                Name = name,
                Type = type,
                Summary = summary,
                DefaultValue = defaultValue
            });
        }

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// 	<see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            foreach(ParamInfo p in this) {
                sb.AppendLine(p.ToString());
            }
            return sb.ToString();
        }
    }
}
