using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocPlus.Javascript {

    /// <summary>
    /// 枚举已知的节点类型。
    /// </summary>
    public static class NodeNames {

        #region 常量

        public static string MemberType = "membertype";

        public static string MemberAccess = "membercccess";

        public static string MemberAttribute = "memberattribute";

        public static string Name = "name";

        public static string Type = "type";

        public static string Return = "return";

        public static string Summary = "summary";

        public static string Value = "defaultvalue";

        #endregion

        public static string Requires = "requires";

        public static string Param = "param";
        public static string MemberOf = "memberOf";

        public static string Exception { get; set; }
    }
}
