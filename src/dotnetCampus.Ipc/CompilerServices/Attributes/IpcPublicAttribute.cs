using System;

namespace dotnetCampus.Ipc.CompilerServices.Attributes
{
    /// <summary>
    /// 指示此类型在 IPC 中公开，可被其他进程发现并使用。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
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
    }
}
