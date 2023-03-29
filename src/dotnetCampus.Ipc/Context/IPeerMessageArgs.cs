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
        IpcMessage Message { get; }

        /// <summary>
        /// 对方的名字，此名字是对方的服务器名字，可以用来连接
        /// </summary>
        string PeerName { get; }

        /// <summary>
        /// 尝试根据 <paramref name="requiredHeader"/> 获取有效负载内容。如果当前的 <see cref="Message"/> 不包含 <paramref name="requiredHeader"/> 头信息，将返回 false 值。如包含，则将 <see cref="Message"/> 去掉 <paramref name="requiredHeader"/> 长度之后作为 <paramref name="subMessage"/> 返回，同时返回 true 值
        /// </summary>
        /// <param name="requiredHeader"></param>
        /// <param name="subMessage"></param>
        /// <returns></returns>
        bool TryGetPayload(byte[] requiredHeader, out IpcMessage subMessage);
    }
}
