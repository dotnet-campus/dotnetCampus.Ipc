using dotnetCampus.Ipc.Abstractions;
using dotnetCampus.Ipc.Abstractions.Context;
using dotnetCampus.Ipc.PipeCore.Context;

namespace dotnetCampus.Ipc.PipeCore.IpcPipe
{
    class IpcRequestMessageContext : IIpcRequestContext
    {
        public IpcRequestMessageContext(IpcBufferMessage ipcBufferMessage)
        {
            IpcBufferMessage = ipcBufferMessage;
        }

        public IpcBufferMessage IpcBufferMessage { get; }
    }
}
