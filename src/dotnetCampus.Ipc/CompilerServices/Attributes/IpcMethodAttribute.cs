using System;
using System.ComponentModel;

namespace dotnetCampus.Ipc.CompilerServices.Attributes;

/// <summary>
/// 指定此方法的 IPC 代理访问方式和对接方式。
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
#if !IPC_ANALYZER
public
#endif
class IpcMethodAttribute : IpcMemberAttribute
{
    /// <summary>
    /// 指定此方法的 IPC 代理访问方式和对接方式。
    /// </summary>
    public IpcMethodAttribute()
    {
    }

    /// <summary>
    /// 指定此 <paramref name="contractType"/> 契约对应此方法的 IPC 代理访问方式和对接方式。
    /// </summary>
    /// <param name="contractType">
    /// IPC 代理访问和对接方式仅限于针对此契约类型。
    /// </param>
    public IpcMethodAttribute(Type contractType) : base(contractType)
    {
    }

    /// <summary>
    /// 如果一个方法返回值是 void，那么此属性决定代理调用此方法时是否需要等待对方执行完成。
    /// 默认为 false，即不等待对方执行完成。
    /// </summary>
    [DefaultValue(false)]
    public bool WaitsVoid { get; set; }
}
