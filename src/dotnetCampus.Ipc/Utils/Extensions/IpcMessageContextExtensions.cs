using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Utils.IO;

namespace dotnetCampus.Ipc.Utils.Extensions
{
    static class IpcMessageContextExtensions
    {
        public static ByteListMessageStream ToStream(this in IpcMessageContext ipcMessageContext)
        {
            return new ByteListMessageStream(ipcMessageContext);
        }
    }
}
