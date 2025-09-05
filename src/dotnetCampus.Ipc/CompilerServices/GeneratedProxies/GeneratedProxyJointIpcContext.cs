using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Serialization;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies
{
    /// <summary>
    /// 绑定到 <see cref="IIpcProvider"/> 后可提供基于 .NET 类型的 IPC 传输。
    /// </summary>
    public class GeneratedProxyJointIpcContext
    {
        /// <summary>
        /// 创建 <see cref="GeneratedProxyJointIpcContext"/> 的新实例。
        /// </summary>
        /// <param name="ipcContext"></param>
        internal GeneratedProxyJointIpcContext(IpcContext ipcContext)
        {
            ObjectSerializer = ipcContext.IpcConfiguration.IpcObjectSerializer;
            JointManager = new PublicIpcJointManager(this);
            RequestHandler = new GeneratedProxyJointIpcRequestHandler(this, ipcContext);
        }

        /// <summary>
        /// 包含 IPC 对接的管理。
        /// </summary>
        internal PublicIpcJointManager JointManager { get; }

        /// <summary>
        /// 请将此属性赋值给 IpcConfiguration.DefaultIpcRequestHandler 以获得 .NET 类型的 IPC 传输访问能力。
        /// </summary>
        public IIpcRequestHandler RequestHandler { get; }

        /// <summary>
        /// 用于序列化和反序列化 IPC 对象的序列化器。
        /// </summary>
        public IIpcObjectSerializer ObjectSerializer { get; set; }
    }
}
