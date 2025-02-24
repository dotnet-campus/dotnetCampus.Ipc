using System;
using System.ComponentModel;

namespace dotnetCampus.Ipc.CompilerServices.Attributes;

/// <summary>
/// 指定成员的 IPC 代理访问方式和对接方式。
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
#if !IPC_ANALYZER
public
#endif
abstract class IpcMemberAttribute : Attribute
{
    /// <summary>
    /// 指定此成员的 IPC 代理访问方式和对接方式。
    /// </summary>
    protected IpcMemberAttribute()
    {
    }

    /// <summary>
    /// 如果指定为 true，则在 IPC 发生异常时会忽略这些异常，并返回默认值。
    /// <para>
    /// 如果同时设置了默认值，则异常时会使用此默认值；如果没有设置默认值，则异常时会使用类型默认值 default。
    /// </para>
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
    /// 设定此成员执行的超时时间（毫秒）。如果自此成员执行开始直至超时时间后依然没有返回，则：
    /// <list type="bullet">
    /// <item>默认会引发 <see cref="dotnetCampus.Ipc.Exceptions.IpcInvokingTimeoutException"/>。</item>
    /// <item>通过在类型或成员上设置 <see cref="IgnoresIpcException"/> 可阻止引发超时异常而改为返回默认值。</item>
    /// </list>
    /// 如果类型上已经标记了超时但不希望某个成员设置超时，可单独在此成员上标记 Timeout = 0。
    /// </summary>
    [DefaultValue(0)]
    public int Timeout { get; set; }
}
