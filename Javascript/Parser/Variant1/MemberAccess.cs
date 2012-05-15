
namespace DocPlus.Javascript {

    /// <summary>
    /// 成员的访问修饰符。
    /// </summary>
    public enum MemberAccess {

        /// <summary>
        /// 默认。
        /// </summary>
        Default = 0,

        /// <summary>
        /// 公开。
        /// </summary>
        Public = 1,

        /// <summary>
        /// 保护。
        /// </summary>
        Protected = 2,

        /// <summary>
        /// 私有。
        /// </summary>
        Private = 4,

        /// <summary>
        /// 内部。
        /// </summary>
        Internal = 8,

        /// <summary>
        /// 保护内部。
        /// </summary>
        ProtectedInternal = Protected | Internal,

        /// <summary>
        /// 在指定的名字空间访问。
        /// </summary>
        Namespace = 16
    }
}
