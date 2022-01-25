using System;
using System.ComponentModel;

namespace dotnetCampus.Ipc.CompilerServices.Attributes;

/// <summary>
/// 指示此接口在 IPC 中公开，可被其他进程发现并使用。
/// <para>
/// 这个特性不会在接口的继承树中传递。因此即使基接口标记了此特性，所有派生接口想要作为公开的 IPC 对象也必须依次标记。
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
#if !IPC_ANALYZER
public
#endif
class IpcPublicAttribute : Attribute
{
    /// <summary>
    /// 指示此接口在 IPC 中公开，可被其他进程发现并使用。
    /// </summary>
    public IpcPublicAttribute()
    {
    }

    /// <summary>
    /// 如果指定为 true，则本接口中的所有成员在 IPC 调用发生异常时会忽略这些异常，并返回默认值。
    /// <para>默认值会采用属性类型或方法返回值类型的默认值（即 default），如果希望额外指定默认值，请：</para>
    /// <list type="bullet">
    /// <item>在单独的属性上使用 IpcPropertyAttribute.DefaultReturn</item>
    /// <item>在单独的方法上使用 IpcMethodAttribute.DefaultReturn</item>
    /// <item>在单独的事件上使用 IpcEventAttribute.DefaultReturn</item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// 请注意：
    /// <list type="bullet">
    /// <item>此特性仅忽略 IPC 连接异常和 IPC 超时异常（例如进程退出、连接断开等），而不会忽略普通业务异常（例如业务实现中抛出了 <see cref="NullReferenceException"/> 等）。</item>
    /// <item>另外，如果 IPC 框架内部出现了 bug 导致了异常，也不会因此而忽略。</item>
    /// </list>
    /// </remarks>
    [DefaultValue(false)]
    public bool IgnoresIpcException { get; set; }

    /// <summary>
    /// 设定此接口中所有成员执行的默认超时时间（毫秒）。如果自成员执行开始直至超时时间后依然没有返回，则：
    /// <list type="bullet">
    /// <item>默认会引发 <see cref="dotnetCampus.Ipc.Exceptions.IpcInvokingTimeoutException"/>。</item>
    /// <item>通过在类型或成员上设置 <see cref="IgnoresIpcException"/> 可阻止引发超时异常而改为返回默认值。</item>
    /// </list>
    /// </summary>
    [DefaultValue(0)]
    public int Timeout { get; set; }
}
