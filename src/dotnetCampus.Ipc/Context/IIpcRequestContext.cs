using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Context
{
    /// <summary>
    /// 客户端请求的上下文
    /// </summary>
    public interface IIpcRequestContext
    {
        /// <summary>
        /// 是否已处理
        /// </summary>
        bool Handled { get; set; }

        /// <summary>
        /// 收到客户端发生过来的消息
        /// </summary>
        IpcMessage IpcBufferMessage { get; }

        /// <summary>
        /// 发送请求的对方
        /// </summary>
        IPeerProxy Peer { get; }
    }

    internal interface ICoreIpcRequestContext
    {
        public CoreMessageType CoreMessageType { get; }
    }
}
