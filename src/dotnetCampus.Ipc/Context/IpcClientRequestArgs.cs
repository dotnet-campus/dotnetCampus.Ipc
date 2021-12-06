using System;

using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Context
{
    /// <summary>
    /// 来自客户端的请求事件参数
    /// </summary>
    public class IpcClientRequestArgs : EventArgs
    {
        /// <summary>
        /// 创建来自客户端的请求事件参数
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="ipcBufferMessage"></param>
        internal IpcClientRequestArgs(in IpcClientRequestMessageId messageId, in IpcMessageBody ipcBufferMessage, IpcMessageCommandType messageCommandType)
        {
            MessageId = messageId;
            IpcMessageBody = ipcBufferMessage;
            MessageCommandType = messageCommandType;
        }

        /// <summary>
        /// 消息号
        /// </summary>
        public IpcClientRequestMessageId MessageId { get; }

        /// <summary>
        /// 收到客户端发生过来的消息
        /// </summary>
        public IpcMessageBody IpcMessageBody { get; }

        internal IpcMessageCommandType MessageCommandType { get; }
    }
}
