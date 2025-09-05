using System;

namespace dotnetCampus.Ipc.CompilerServices.Attributes;

/// <summary>
/// 标记一个类型是 IPC 形状代理。这个类型的所有成员都没有实现，唯一的作用就是定义 IPC 类型中的每个方法应如何通过 IPC 代理来访问。
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
#if !IPC_ANALYZER
public
#endif
class IpcShapeAttribute : IpcPublicAttribute
{
    /// <summary>
    /// 标记一个类型是 IPC 形状代理。这个类型的所有成员都没有实现，唯一的作用就是定义 IPC 类型中的每个方法应如何通过 IPC 代理来访问。
    /// </summary>
    /// <param name="contractType">此形状代理所代理的契约类型。</param>
    public IpcShapeAttribute(Type contractType)
    {
        ContractType = contractType;
    }

    /// <summary>
    /// 此形状代理所代理的契约类型。
    /// <para>一个形状代理只能代理一个契约类型。</para>
    /// </summary>
    public Type ContractType { get; }
}
