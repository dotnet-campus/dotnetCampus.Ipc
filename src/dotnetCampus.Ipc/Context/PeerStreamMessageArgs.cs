using System;
using System.Diagnostics;
using System.IO;

using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Context
{
    /// <summary>
    /// 替代 <see cref="PeerMessageArgs"/> 在 IPC 框架内进行高性能传递。
    /// </summary>
    internal class PeerStreamMessageArgs : EventArgs
    {
        /// <summary>
        /// 创建对方通讯的消息事件参数
        /// </summary>
        /// <param name="ipcMessageContext"></param>
        /// <param name="peerName"></param>
        /// <param name="messageStream"></param>
        /// <param name="ack"></param>
        /// <param name="messageCommandType"></param>
        [DebuggerStepThrough]
        internal PeerStreamMessageArgs(IpcMessageContext ipcMessageContext, string peerName, Stream messageStream, in Ack ack, IpcMessageCommandType messageCommandType)
        {
            IpcMessageContext = ipcMessageContext;
            PeerName = peerName;
            MessageStream = messageStream;
            Ack = ack;
            MessageCommandType = messageCommandType;
        }

        internal IpcMessageContext IpcMessageContext { get; }

        /// <summary>
        /// 用于读取消息的内容
        /// </summary>
        internal Stream MessageStream { get; }

        /// <summary>
        /// 消息编号
        /// </summary>
        public Ack Ack { get; }

        /// <summary>
        /// 对方的名字，此名字是对方的服务器名字，可以用来连接
        /// </summary>
        public string PeerName { get; }

        internal IpcMessageCommandType MessageCommandType { get; }

        /// <summary>
        /// 表示是否被上一级处理了，可以通过 <see cref="HandlerMessage"/> 了解处理者的信息
        /// </summary>
        public bool Handle { private set; get; }

        /// <summary>
        /// 处理者的消息
        /// </summary>
        /// 框架大了，不能只有 <see cref="Handle"/> 一个属性，还需要能做到调试，调试是谁处理了，因此加添加了这个属性
        public string? HandlerMessage { private set; get; }

        /// <summary>
        /// 设置被处理，同时添加 <paramref name="message"/> 用于调试的信息
        /// </summary>
        /// <param name="message">用于调试的信息，请记录是谁设置的，原因是什么</param>
        public void SetHandle(string message)
        {
            Handle = true;
            HandlerMessage = message;
        }

        internal PeerMessageArgs ToPeerMessageArgs()
        {
            var message = new IpcMessage("MessageReceived", new IpcMessageBody(IpcMessageContext.MessageBuffer, (int) MessageStream.Position, (int) (IpcMessageContext.MessageLength - MessageStream.Position)));
            return new PeerMessageArgs(PeerName, message, Ack, MessageCommandType);
        }
    }
}
