using System;
using System.ComponentModel;

namespace dotnetCampus.Ipc.CompilerServices.Attributes;

/// <summary>
/// 指定此方法的 IPC 代理访问方式和对接方式。
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
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
    /// 在指定了 <see cref="IpcMemberAttribute.IgnoresIpcException"/> 或者 <see cref="IpcMemberAttribute.Timeout"/> 的情况下，如果真的发生了 IPC 连接异常或超时，则会使用此默认值。
    /// <list type="number">
    /// <item>不指定此值时，会使用属性或返回值类型的默认值（即 default）。</item>
    /// <item>如果此值可使用编译时常量来表示，则直接写在这里即可。</item>
    /// <item>如果此值无法写成编译时常量，请使用字符串形式编写（例如 "IntPtr.Zero" ，含英文引号），编译后会自动将其转为真实代码。</item>
    /// <item>
    /// 接上条，如果你确实是一个 object 返回值但默认值是一个字符串，请写成以下两种中的一种：
    /// <list type="bullet">
    /// <item>@"""字符串值"""（含“@”及所有的英文引号）</item>
    /// <item>"\"字符串值\""（含所有的英文引号及斜杠）</item>
    /// </list>
    /// </item>
    /// </list>
    /// </summary>
    [DefaultValue(null)]
    public object? DefaultReturn { get; set; }

    /// <summary>
    /// 如果一个方法返回值是 void，那么此属性决定代理调用此方法时是否需要等待对方执行完成。
    /// 默认为 false，即不等待对方执行完成。
    /// </summary>
    [DefaultValue(false)]
    public bool WaitsVoid { get; set; }
}
