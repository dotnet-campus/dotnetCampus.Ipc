using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

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
    /// <item>请使用字符串形式编写值（例如 "IntPtr.Zero" ，含英文引号），编译后会自动将其转为真实代码。</item>
    /// <item>
    /// 接上条，以下写法会在编译时转成对应的语句：
    /// <code>
    /// | 写法（含引号）   | 转成的语句       |
    /// |------------------|------------------|
    /// | null             | null             |
    /// | "null"           | null             |
    /// | default          | default          |
    /// | "default"        | default          |
    /// | "\"text\""       | "text"           |
    /// | @"""text"""      | "text"           |
    /// | "new Foo(2)"     | new Foo(2)       |
    /// | "SomeEnum.Value" | SomeEnum.Value   |
    /// </code>
    /// </item>
    /// </list>
    /// </summary>
    [DefaultValue(null)]
#if NET8_0_OR_GREATER
    [StringSyntax("csharp")]
#endif
    public string? DefaultReturn { get; set; }

    /// <summary>
    /// 如果一个方法返回值是 void，那么此属性决定代理调用此方法时是否需要等待对方执行完成。
    /// 默认为 false，即不等待对方执行完成。
    /// </summary>
    [DefaultValue(false)]
    public bool WaitsVoid { get; set; }
}
