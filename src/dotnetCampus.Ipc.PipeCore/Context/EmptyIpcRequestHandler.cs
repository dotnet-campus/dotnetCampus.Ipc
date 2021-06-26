using dotnetCampus.Ipc.Abstractions;
using dotnetCampus.Ipc.Abstractions.Context;

namespace dotnetCampus.Ipc.PipeCore.Context
{
    class EmptyIpcRequestHandler : IIpcRequestHandler
    {
        public IIpcHandleRequestMessageResult HandleRequestMessage(IIpcRequestContext requestContext)
        {
            // 我又不知道业务，不知道怎么玩……
            var responseMessage = new IpcRequestMessage(nameof(EmptyIpcRequestHandler), new IpcBufferMessage(new byte[0]));
            return new IpcHandleRequestMessageResult(responseMessage);
        }
    }
}