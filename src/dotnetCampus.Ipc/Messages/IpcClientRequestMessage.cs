using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;

namespace dotnetCampus.Ipc.Messages
{
    class IpcClientRequestMessage
    {
        public IpcClientRequestMessage(IpcBufferMessageContext ipcBufferMessageContext, Task<IpcMessageBody> task, IpcClientRequestMessageId messageId)
        {
            IpcBufferMessageContext = ipcBufferMessageContext;
            Task = task;
            MessageId = messageId;
        }

        public IpcBufferMessageContext IpcBufferMessageContext { get; }

        /// <summary>
        /// 用于等待消息被对方回复完成
        /// </summary>
        public Task<IpcMessageBody> Task { get; }

        public IpcClientRequestMessageId MessageId { get; }
    }
}
