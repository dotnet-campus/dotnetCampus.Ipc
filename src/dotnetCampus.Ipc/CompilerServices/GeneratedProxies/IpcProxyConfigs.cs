using System;
using System.ComponentModel;

using dotnetCampus.Ipc.CompilerServices.Attributes;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies;

// 注意：本文件中的配置类因为未来对修改开放，所以**不适合**提取接口。如坚持提取，未来定会造成兼容性问题！

/// <summary>
/// 为 IPC 代理访问提供配置。
/// <para>注意：</para>
/// <list type="number">
/// <item>使用此方式指定的 IPC 代理配置优先级为“最低”，低于 <see cref="IpcPublicAttribute"/> 指定的配置，更低于在接口的成员上单独指定的配置。因此仅在接口上或接口的成员上没有指定 IPC 代理配置时才会生效。</item>
/// <item>如需覆盖接口上指定的 IPC 代理配置，请额外生成一个 IPC 形状代理（）</item>
/// </list>
/// </summary>
public class IpcProxyConfigs
{
    /// <summary>
    /// 如果指定为 true，则本类型在 IPC 调用发生异常时会忽略这些异常，并返回默认值。
    /// <para>默认值会采用属性类型或方法返回值类型的默认值（即 default），如果希望额外指定默认值，请：</para>
    /// <list type="bullet">
    /// <item>在单独的属性上使用 IpcPropertyAttribute.DefaultReturn</item>
    /// <item>在单独的属性上使用 IpcMethodAttribute.DefaultReturn</item>
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
    /// 设定此类型中所有成员执行的默认超时时间（毫秒）。如果自成员执行开始直至超时时间后依然没有返回，则：
    /// <list type="bullet">
    /// <item>默认会引发 <see cref="dotnetCampus.Ipc.Exceptions.IpcInvokingTimeoutException"/>。</item>
    /// <item>通过在类型或成员上设置 <see cref="IgnoresIpcException"/> 可阻止引发超时异常而改为返回默认值。</item>
    /// </list>
    /// </summary>
    [DefaultValue(0)]
    public int Timeout { get; set; }
}
