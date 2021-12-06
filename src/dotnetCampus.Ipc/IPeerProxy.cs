using System;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc
{
    /// <summary>
    /// 用于表示远程的对方
    /// </summary>
    public interface IPeerProxy
    {
        /// <summary>
        /// 对方的服务器名
        /// </summary>
        string PeerName { get; }

        /// <summary>
        /// 发送请求给对方，请求对方的响应。这是客户端-服务器端模式
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task NotifyAsync(IpcMessage request);

        /// <summary>
        /// 发送请求给对方，请求对方的响应。这是客户端-服务器端模式
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<IpcMessage> GetResponseAsync(IpcMessage request);

        /// <summary>
        /// 当收到消息时触发
        /// </summary>
        event EventHandler<IPeerMessageArgs> MessageReceived;

        /// <summary>
        /// 对方连接断开事件
        /// </summary>
        event EventHandler<IPeerConnectionBrokenArgs> PeerConnectionBroken;

        /// <summary>
        /// 对方断开重连
        /// </summary>
        event EventHandler<IPeerReconnectedArgs> PeerReconnected;
    }
}
