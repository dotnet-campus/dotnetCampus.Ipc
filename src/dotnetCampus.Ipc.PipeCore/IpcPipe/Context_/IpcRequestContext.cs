using dotnetCampus.Ipc.Abstractions;
using dotnetCampus.Ipc.PipeCore.Context;

namespace dotnetCampus.Ipc.PipeCore.IpcPipe
{
    class IpcRequestContext : IIpcRequestContext
    {
        public IpcRequestContext(IpcBufferMessage ipcBufferMessage)
        {
            IpcBufferMessage = ipcBufferMessage;
        }

        public IpcBufferMessage IpcBufferMessage { get; }
    }
}
