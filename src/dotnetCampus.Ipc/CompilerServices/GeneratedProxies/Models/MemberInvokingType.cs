namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models
{
    /// <summary>
    /// 自动生成的 IPC 调用代码中，目前支持的所有成员调用类型。
    /// </summary>
    /// <remarks>
    /// 后续支持更多调用类型时，将扩充此枚举。
    /// </remarks>
    internal enum MemberInvokingType
    {
        /// <summary>
        /// 未知的调用类型。
        /// 此值作为 IPC 传输过程中因为未传输或传输出错后的默认值。
        /// </summary>
        Unknown,

        /// <summary>
        /// 获取属性的值。
        /// </summary>
        GetProperty,

        /// <summary>
        /// 设置属性的值。
        /// </summary>
        SetProperty,

        /// <summary>
        /// 方法调用。
        /// </summary>
        Method,

        /// <summary>
        /// 异步方法调用。
        /// </summary>
        AsyncMethod,
    }
}
