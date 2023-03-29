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
}
