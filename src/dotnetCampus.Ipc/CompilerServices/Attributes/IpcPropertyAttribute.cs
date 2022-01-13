using System;
using System.ComponentModel;

namespace dotnetCampus.Ipc.CompilerServices.Attributes;

/// <summary>
/// 指定此属性的 IPC 代理访问方式和对接方式。
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
#if !IPC_ANALYZER
public
#endif
sealed class IpcPropertyAttribute : IpcMemberAttribute
{
    /// <summary>
    /// 指定此属性的 IPC 代理访问方式和对接方式。
    /// </summary>
    public IpcPropertyAttribute()
    {
    }

    /// <summary>
    /// 指定此 <paramref name="contractType"/> 契约对应此属性的 IPC 代理访问方式和对接方式。
    /// </summary>
    /// <param name="contractType">
    /// IPC 代理访问和对接方式仅限于针对此契约类型。
    /// </param>
    public IpcPropertyAttribute(Type contractType) : base(contractType)
    {
    }

    /// <summary>
    /// 标记一个属性是对于 IPC 代理访问来说是只读的。
    /// 当通过 IPC 访问过一次这个属性后，此属性不再变化，后续无需再通过 IPC 读取，可直接使用本地缓存的值。
    /// </summary>
    [DefaultValue(false)]
    public bool IsReadonly { get; set; }
}
