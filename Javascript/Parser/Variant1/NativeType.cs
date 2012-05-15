using System;
using System.Collections.Generic;
using System.Text;

namespace DocPlus.Javascript {

    /// <summary>
    /// 表示 Javascript 内部类型的枚举。
    /// </summary>
    public enum NativeType {

        /// <summary>
        /// 表示任何对象类型。
        /// </summary>
        Object,

        /// <summary>
        /// 表示数字类型。
        /// </summary>
        Number,

        /// <summary>
        /// 表示字符串类型。
        /// </summary>
        String,

        /// <summary>
        /// 表示布尔类型。
        /// </summary>
        Boolean,

        /// <summary>
        /// 表示函数类型。
        /// </summary>
        Function,

        ///// <summary>
        ///// 表示正则类型。
        ///// </summary>
        //RegExp,

        ///// <summary>
        ///// 表示数组类型。
        ///// </summary>
        //Array,

        /// <summary>
        /// 表示未定义类型。
        /// </summary>
        Undefined,

        /// <summary>
        /// 表示空类型。
        /// </summary>
        Null

    }
}
