using dotnetCampus.Ipc.Abstractions;

namespace dotnetCampus.Ipc.PipeCore.Context
{
    class IpcHandleRequestMessageResult : IIpcHandleRequestMessageResult
    {
        public IpcHandleRequestMessageResult(IpcRequestMessage returnMessage)
        {
            ReturnMessage = returnMessage;
        }

        public IpcRequestMessage ReturnMessage { get; }
    }
}
