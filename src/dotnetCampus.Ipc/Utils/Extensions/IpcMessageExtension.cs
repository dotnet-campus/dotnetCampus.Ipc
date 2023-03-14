using System;
using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Utils.Extensions;

static class IpcMessageExtension
{
    public static IpcMessage Skip(this IpcMessage ipcMessage, int length)
    {
        return new IpcMessage(ipcMessage.Tag, new IpcMessageBody(ipcMessage.Body.Buffer,
            ipcMessage.Body.Start + length,
            ipcMessage.Body.Length - length));
    }

    public static bool TryReadBusinessHeader(this in IpcMessage ipcMessage, out ulong header)
    {
        var length = sizeof(ulong);
        if (ipcMessage.Body.Length < length)
        {
            header = 0;
            return false;
        }
        else
        {
            header = BitConverter.ToUInt64(ipcMessage.Body.Buffer, ipcMessage.Body.Start);
            return true;
        }
    }
}
