using System;
using System.ComponentModel;

namespace dotnetCampus.Ipc.CompilerServices.Attributes
{
    /// <summary>
    /// 指示此类型在 IPC 中公开，可被其他进程发现并使用。
    /// <para>
    /// 这个特性不会在类型的继承树中传递。因此即使基类标记了此特性，所有派生类想要公开 IPC 对象也必须依次标记。
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class IpcPublicAttribute : Attribute
    {
        /// <summary>
        /// 指示此类型在 IPC 中公开，可被其他进程发现并使用。
        /// </summary>
        /// <param name="contractType">
        /// 此类型对 IPC 公开时，应以此契约类型公开。
        /// 其他希望访问此 IPC 公开类型的进程能通过此契约类型找到并使用此类型的实例。
        /// </param>
        public IpcPublicAttribute(Type contractType)
        {
            ContractType = contractType;
        }

        /// <summary>
        /// 此类型对 IPC 公开时，应以此契约类型公开。
        /// 其他希望访问此 IPC 公开类型的进程能通过此契约类型找到并使用此类型的实例。
        /// </summary>
        public Type ContractType { get; }

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
        /// 设定此类型中所有成员执行的默认超时时间。如果自成员执行开始直至超时时间后依然没有返回，则：
        /// <list type="bullet">
        /// <item>默认会引发 <see cref="dotnetCampus.Ipc.Exceptions.IpcInvokingTimeoutException"/>。</item>
        /// <item>通过在类型或成员上设置 <see cref="IgnoresIpcException"/> 可阻止引发超时异常而改为返回默认值。</item>
        /// </list>
        /// </summary>
        [DefaultValue(0)]
        public int Timeout { get; set; }
    }
}
