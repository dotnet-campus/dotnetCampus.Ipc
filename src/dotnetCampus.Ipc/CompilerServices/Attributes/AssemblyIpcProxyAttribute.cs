#if NET461_OR_GREATER || NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_0_OR_GREATER
using System;

namespace dotnetCampus.Ipc.CompilerServices.Attributes;

/// <summary>
/// 由编译器自动生成，将 IPC 类型与其自动生成的代理类型关联起来（没有关联对接类型）。
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
#if !IPC_ANALYZER
public
#endif
class AssemblyIpcProxyAttribute : Attribute
{
    /// <summary>
    /// 由编译器自动生成，将 IPC 类型与其自动生成的代理和对接类型关联起来。
    /// </summary>
    /// <param name="contractType">契约接口类型。</param>
    /// <param name="ipcType">IPC 类型（即标记了 <see cref="IpcShapeAttribute"/> 的类型）。</param>
    /// <param name="proxyType">代理类型。</param>
    public AssemblyIpcProxyAttribute(Type contractType, Type ipcType, Type proxyType)
    {
        ContractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        IpcType = ipcType ?? throw new ArgumentNullException(nameof(ipcType));
        ProxyType = proxyType ?? throw new ArgumentNullException(nameof(proxyType));
    }

    /// <summary>
    /// 契约类型。
    /// </summary>
    public Type ContractType { get; }

    /// <summary>
    /// IPC 类型（即标记了 <see cref="IpcShapeAttribute"/> 的类型）。
    /// </summary>
    public Type IpcType { get; set; }

    /// <summary>
    /// 代理类型。
    /// </summary>
    public Type ProxyType { get; set; }
}
#endif
