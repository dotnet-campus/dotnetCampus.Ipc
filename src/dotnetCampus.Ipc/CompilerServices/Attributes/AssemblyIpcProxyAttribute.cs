using System;

namespace dotnetCampus.Ipc.CompilerServices.Attributes;

/// <summary>
/// 由编译器自动生成，将 IPC 契约类型与其自动生成的代理和对接类型关联起来。
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
internal class AssemblyIpcProxyJointAttribute : Attribute
{
    public AssemblyIpcProxyJointAttribute(Type contractType, Type proxyType, Type jointType)
    {
        ContractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        ProxyType = proxyType ?? throw new ArgumentNullException(nameof(proxyType));
        JointType = jointType ?? throw new ArgumentNullException(nameof(jointType));
    }

    /// <summary>
    /// 契约类型。
    /// </summary>
    public Type ContractType { get; set; }

    /// <summary>
    /// 代理类型。
    /// </summary>
    public Type ProxyType { get; set; }

    /// <summary>
    /// 对接类型。
    /// </summary>
    public Type JointType { get; set; }
}
