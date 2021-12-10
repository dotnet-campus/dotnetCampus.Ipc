using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.PipeMvcServer
{
    class IpcResponseMessageResult : IIpcResponseMessage
    {
        public IpcResponseMessageResult(IpcMessage responseMessage)
        {
            ResponseMessage = responseMessage;
        }

        public IpcMessage ResponseMessage { get; }
    }
}