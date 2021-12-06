using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;

namespace dotnetCampus.Ipc.Internals
{
    internal interface IClientMessageWriter : IRawMessageWriter
    {
        Task WriteMessageAsync(in IpcBufferMessageContext ipcBufferMessageContext);
    }
}
