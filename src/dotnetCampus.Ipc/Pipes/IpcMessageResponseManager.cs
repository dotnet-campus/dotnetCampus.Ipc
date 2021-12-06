using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Pipes
{
    /// <summary>
    /// 响应管理器
    /// </summary>
    /// 这个类还没设计好，和 <see cref="IpcMessageRequestManager"/> 是重复的
    /// 完全可以删除
    class IpcMessageResponseManager : IpcMessageManagerBase
    {
        public IpcBufferMessageContext CreateResponseMessage(IpcClientRequestMessageId messageId,
           in IpcMessage response)
            => CreateResponseMessageInner(messageId, in response);
    }
}
