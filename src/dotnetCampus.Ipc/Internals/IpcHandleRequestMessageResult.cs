using System.Diagnostics;

using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Internals
{
    class IpcHandleRequestMessageResult : IIpcResponseMessage
    {
        [DebuggerStepThrough]
        public IpcHandleRequestMessageResult(IpcMessage returnMessage)
        {
            ResponseMessage = returnMessage;
        }

        public IpcMessage ResponseMessage { get; }
    }
}
