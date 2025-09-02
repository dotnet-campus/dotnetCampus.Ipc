﻿#pragma warning disable format
using System;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies;

partial class GeneratedIpcProxy
{
    /// <summary>
    /// 仅供 <see cref="GeneratedIpcProxy"/> 的自动生成的派生类与基类传递参数使用，包含参数传递所需的各种个性化需求。
    /// </summary>
    protected class IpcProxyMemberNamedValues
    {
        /// <summary>
        /// 设定此方法执行的超时时间（毫秒）。如果自此方法执行开始直至超时时间后依然没有返回，则会引发 <see cref="dotnetCampus.Ipc.Exceptions.IpcInvokingTimeoutException"/>。
        /// </summary>
        public int? Timeout { get; set; }

        /// <summary>
        /// 在指定了 <see cref="IgnoresIpcException"/> 或者 <see cref="Timeout"/> 的情况下，如果真的发生了异常或超时，则会使用此默认值。
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
        public object? DefaultReturn { get; set; }

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
        public bool? IgnoresIpcException { get; set; }

        /// <summary>
        /// 标记一个属性是对于 IPC 代理访问来说是只读的。
        /// 当通过 IPC 访问过一次这个属性后，此属性不再变化，后续无需再通过 IPC 读取，可直接使用本地缓存的值。
        /// </summary>
        public bool? IsReadonly { get; set; }

        /// <summary>
        /// 如果一个方法返回值是 void，那么此属性决定代理调用此方法时是否需要等待对方执行完成。
        /// 默认为 false，即不等待对方执行完成。
        /// </summary>
        public bool? WaitsVoid { get; set; }
    }
}
