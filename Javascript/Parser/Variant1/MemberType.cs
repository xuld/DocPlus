
namespace DocPlus.Javascript {

    /// <summary>
    /// 表示一个成员的类型。
    /// </summary>
    public enum MemberType {

        /// <summary>
        /// 默认。未知的成员类型。
        /// </summary>
        Default = 0,

        /// <summary>
        /// 表示成员是一个名字空间。
        /// </summary>
        Package,

        /// <summary>
        /// 表示成员是一个类。
        /// </summary>
        Class,

        /// <summary>
        /// 表示成员是一个枚举。
        /// </summary>
        Enum,

        /// <summary>
        /// 表示成员是一个接口。
        /// </summary>
        Interface,

        /// <summary>
        /// 表示成员是一个字段。
        /// </summary>
        Field,

        /// <summary>
        /// 表示成员是一个方法。
        /// </summary>
        Method,

        /// <summary>
        /// 表示成员是一个事件。
        /// </summary>
        Event,

        /// <summary>
        /// 表示成员是一个属性。
        /// </summary>
        Property,

        /// <summary>
        /// 表示成员是一个只读属性。
        /// </summary>
        PropertyGetter,

        /// <summary>
        /// 表示成员是一个只写属性。
        /// </summary>
        PropertySetter,

        /// <summary>
        /// 表示成员是一个配置字段。
        /// </summary>
        Config

    }
}
