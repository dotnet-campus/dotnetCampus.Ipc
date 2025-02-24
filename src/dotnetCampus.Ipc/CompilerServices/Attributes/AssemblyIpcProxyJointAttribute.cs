#if NET461_OR_GREATER || NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_0_OR_GREATER
using System;

namespace dotnetCampus.Ipc.CompilerServices.Attributes;

/// <summary>
/// 由编译器自动生成，将 IPC 类型与其自动生成的代理和对接类型关联起来。
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
#if !IPC_ANALYZER
public
#endif
class AssemblyIpcProxyJointAttribute : Attribute
{
    /// <summary>
    /// 由编译器自动生成，将 IPC 类型与其自动生成的代理和对接类型关联起来。
    /// </summary>
    /// <param name="ipcType">契约类型（即标记了 <see cref="IpcPublicAttribute"/> 或 <see cref="IpcShapeAttribute"/> 的类型）。</param>
    /// <param name="proxyType">代理类型。</param>
    /// <param name="jointType">对接类型。</param>
    public AssemblyIpcProxyJointAttribute(Type ipcType, Type proxyType, Type jointType)
    {
        IpcType = ipcType ?? throw new ArgumentNullException(nameof(ipcType));
        ProxyType = proxyType ?? throw new ArgumentNullException(nameof(proxyType));
        JointType = jointType ?? throw new ArgumentNullException(nameof(jointType));
    }

    /// <summary>
    /// 契约类型（即标记了 <see cref="IpcPublicAttribute"/> 或 <see cref="IpcShapeAttribute"/> 的类型）。
    /// </summary>
    public Type IpcType { get; set; }

    /// <summary>
    /// 代理类型。
    /// </summary>
    public Type ProxyType { get; set; }

    /// <summary>
    /// 对接类型。
    /// </summary>
    public Type JointType { get; set; }
}
#endif
