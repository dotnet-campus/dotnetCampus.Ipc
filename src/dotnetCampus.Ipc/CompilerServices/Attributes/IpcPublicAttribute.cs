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
        /// <param name="userSideProxyType">
        /// 生成一个 IPC 代理类。当需要使用远端的此类型的对象时，应通过此代理类来访问。名字可以随意指定，会在编译时自动生成对应名字的代理类。
        /// </param>
        /// <param name="ownerSideJointType">
        /// 生成一个 IPC 对接类。当其他进程试图访问此类型时，会通过此对接类来间接访问。名字可以随意指定，会在编译时自动生成对应名字的对接类。
        /// </param>
        public IpcPublicAttribute(Type contractType, Type userSideProxyType, Type ownerSideJointType)
        {
            ContractType = contractType;
            ProxyType = userSideProxyType;
            JointType = ownerSideJointType;
        }

        /// <summary>
        /// 此类型对 IPC 公开时，应以此契约类型公开。
        /// 其他希望访问此 IPC 公开类型的进程能通过此契约类型找到并使用此类型的实例。
        /// </summary>
        public Type ContractType { get; }

        /// <summary>
        /// IPC 代理类。当需要使用远端的此类型的对象时，应通过此代理类来访问。名字可以随意指定，会在编译时自动生成对应名字的代理类。
        /// </summary>
        public Type ProxyType { get; }

        /// <summary>
        /// IPC 对接类。当其他进程试图访问此类型时，会通过此对接类来间接访问。名字可以随意指定，会在编译时自动生成对应名字的对接类。
        /// </summary>
        public Type JointType { get; }
    }
}
