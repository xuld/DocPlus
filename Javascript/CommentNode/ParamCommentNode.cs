using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocPlus.Javascript {
    public class ParamCommentNode : List<ParamCommentNode.ParamInfo>, ICommentNode {

        public sealed class ParamInfo {
            public string Name;
            public string Type;
            public string Summary;
            public string DefaultValue;
        }

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

        public string ToJson() {
            return "['param']";
        }
    }
}
