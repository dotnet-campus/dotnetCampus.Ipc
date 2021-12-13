using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.PipeMvcServer.IpcFramework
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
