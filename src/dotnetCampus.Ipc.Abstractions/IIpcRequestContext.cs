using dotnetCampus.Ipc.Abstractions.Context;
using dotnetCampus.Ipc.PipeCore.Context;

namespace dotnetCampus.Ipc.Abstractions
{
    /// <summary>
    /// 客户端请求的上下文
    /// </summary>
    public interface IIpcRequestContext
    {
        /// <summary>
        /// 收到客户端发生过来的消息
        /// </summary>
        IpcBufferMessage IpcBufferMessage { get; }
    }
}
