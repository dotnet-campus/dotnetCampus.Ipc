using System;
using System.ComponentModel;

namespace dotnetCampus.Ipc.CompilerServices.Attributes;

/// <summary>
/// 指定成员的 IPC 代理访问方式和对接方式。
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
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

    /// <summary>
    /// 在指定了 <see cref="IgnoreIpcException"/> 或者 <see cref="Timeout"/> 的情况下，如果真的发生了异常或超时，则会使用此默认值。
    /// <list type="bullet">
    /// <item>不指定此值时，会使用属性或返回值类型的默认值（即 default）。</item>
    /// <item>如果此值可使用编译时常量来表示，则直接写在这里即可。</item>
    /// <item>如果此值无法写成编译时常量，请使用字符串形式编写（例如 "IntPtr.Zero" ，含英文引号），编译后会自动将其转为真实代码。</item>
    /// </list>
    /// </summary>
    public object? DefaultReturn { get; set; }

    /// <summary>
    /// 如果指定为 true，则在 IPC 发生异常时会忽略这些异常，并返回默认值。
    /// <para>
    /// 如果同时设置了默认值，则异常时会使用此默认值；如果没有设置默认值，则异常时会使用类型默认值 default。
    /// </para>
    /// </summary>
    public bool IgnoreIpcException { get; set; }

    /// <summary>
    /// 设定此方法执行的超时时间。如果自此方法执行开始直至超时时间后依然没有返回，则会引发 <see cref="dotnetCampus.Ipc.Exceptions.IpcInvokingTimeoutException"/>。
    /// </summary>
    public int Timeout { get; set; }
}
