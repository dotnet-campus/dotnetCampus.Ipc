#pragma warning disable format
using System;

using dotnetCampus.Ipc.CompilerServices.Attributes;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies;

#if !IPC_ANALYZER
partial class GeneratedIpcProxy
{
#endif
    /// <summary>
    /// 仅供 <see cref="GeneratedIpcProxy"/> 的自动生成的派生类与基类传递参数使用，包含参数传递所需的各种个性化需求。
    /// </summary>
#if !IPC_ANALYZER
    protected
#endif
    readonly struct IpcProxyMemberAttributes
    {
        public IpcProxyMemberAttributes(object? defaultReturn, int? timeout,
            bool ignoreIpcException = false, bool isReadonly = false, bool waitsVoid = false)
        {
            DefaultReturn = defaultReturn;
            Timeout = timeout;
            Flags = IpcMemberAttributeFlags.None;

            if (ignoreIpcException)
            {
                Flags |= IpcMemberAttributeFlags.IgnoreIpcException;
            }
            if (isReadonly)
            {
                Flags |= IpcMemberAttributeFlags.IsReadonly;
            }
            if (waitsVoid)
            {
                Flags |= IpcMemberAttributeFlags.WaitsVoid;
            }
        }

        internal IpcMemberAttributeFlags Flags { get; }

        internal int? Timeout { get; }

        internal object? DefaultReturn { get; }
    }

    /// <summary>
    /// 仅供 <see cref="GeneratedIpcProxy"/> 的自动生成的派生类与基类传递参数使用，包含所有参数合为一体的标记位。
    /// </summary>
    [Flags]

#if !IPC_ANALYZER
    protected
#endif
    enum IpcMemberAttributeFlags
    {
        /// <summary>
        /// 没有任何标记。
        /// </summary>
        None = 0x0000_0000,

        /// <summary>
        /// 参见 <see cref="IpcMemberAttribute.IgnoreIpcException"/>。
        /// </summary>
        IgnoreIpcException = 0x0000_0001,

        /// <summary>
        /// 参见 <see cref="IpcPropertyAttribute.IsReadonly"/>。
        /// </summary>
        IsReadonly = 0x0000_0002,

        /// <summary>
        /// 参见 <see cref="IpcMethodAttribute.WaitsVoid"/>。
        /// </summary>
        WaitsVoid = 0x0000_0004,
    }
#if !IPC_ANALYZER
}
#endif
