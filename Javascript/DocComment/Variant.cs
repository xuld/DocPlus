using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace DocPlus.Javascript {

    public class Variant : SortedList<string, Variant> {

        public DocComment DocComment {
            get;
            set;
        }

        public Variant Members {
            get {
                Variant v;
                return TryGetValue("prototype", out v) ? v : null;
            }
        }

    }
}
