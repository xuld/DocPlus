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
        }

        public void Add(string name, string type, string summary) {
            Add(new ParamInfo() {
                Name = name,
                Type = type,
                Summary = summary
            });
        }

        public string ToJson() {
            return "['param']";
        }
    }
}
