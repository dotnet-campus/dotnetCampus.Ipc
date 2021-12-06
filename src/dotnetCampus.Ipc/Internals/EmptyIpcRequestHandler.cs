using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Internals
{
    class EmptyIpcRequestHandler : IIpcRequestHandler
    {
        public Task<IIpcResponseMessage> HandleRequest(IIpcRequestContext requestContext)
        {
            // 我又不知道业务，不知道怎么玩……
            var responseMessage = new IpcMessage(nameof(EmptyIpcRequestHandler), new IpcMessageBody(new byte[0]));
            return Task.FromResult((IIpcResponseMessage) new IpcHandleRequestMessageResult(responseMessage));
        }
    }
}
