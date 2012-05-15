using System;
using System.Collections.Generic;
using System.Text;

namespace DocPlus.Javascript {
    
    /// <summary>
    /// 表示成员属性的枚举。
    /// </summary>
    public enum MemberAttributes {

        /// <summary>
        /// 没有额外的属性。
        /// </summary>
        None = 0,

        /// <summary>
        /// 表示这是静态的属性。
        /// </summary>
        Static = 1,

        /// <summary>
        /// 表示这是虚成员。
        /// </summary>
        Virtual = 2,

        /// <summary>
        /// 表示这是重载的成员。
        /// </summary>
        Override = 4,

        /// <summary>
        /// 表示这是抽象的成员。
        /// </summary>
        Abstract = 8,

        /// <summary>
        /// 表示这是抽象的成员。
        /// </summary>
        Sealed = 16,

        /// <summary>
        /// 表示这是抽象的成员。
        /// </summary>
        ReadOnly = 32,

        /// <summary>
        /// 表示这是抽象的成员。
        /// </summary>
        Const = 64,

        /// <summary>
        /// 表示这是抽象的成员。
        /// </summary>
        StaticReadOnly = Static | ReadOnly,

        /// <summary>
        /// 表示这是抽象的成员。
        /// </summary>
        StaticConst = Static | Const,

    }

}
