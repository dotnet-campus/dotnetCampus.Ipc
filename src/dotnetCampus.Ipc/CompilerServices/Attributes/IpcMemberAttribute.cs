using System;

namespace dotnetCampus.Ipc.CompilerServices.Attributes;

/// <summary>
/// 指定成员的 IPC 代理访问方式和对接方式。
/// </summary>
public abstract class IpcMemberAttribute : Attribute
{
    /// <summary>
    /// 指定此成员的 IPC 代理访问方式和对接方式。
    /// </summary>
    protected IpcMemberAttribute()
    {
    }

    /// <summary>
    /// 指定此 <paramref name="contractType"/> 契约对应此成员的 IPC 代理访问方式和对接方式。
    /// </summary>
    /// <param name="contractType">
    /// IPC 代理访问和对接方式仅限于针对此契约类型。
    /// </param>
    protected IpcMemberAttribute(Type contractType)
    {
        ContractType = contractType;
    }

    /// <summary>
    /// 获取成员设置 IPC 访问方式时指定的特定契约类型。
    /// </summary>
    public Type? ContractType { get; }
}
