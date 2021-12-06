using System.IO;

using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Context
{
    /// <summary>
    /// 收到的对方的信息事件参数
    /// </summary>
    public interface IPeerMessageArgs
    {
        /// <summary>
        /// 来自其他端的消息。
        /// </summary>
        public IpcMessage Message { get; }

        /// <summary>
        /// 对方的名字，此名字是对方的服务器名字，可以用来连接
        /// </summary>
        public string PeerName { get; }
    }
}
