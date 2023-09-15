using System;
using System.Text;
using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Context.LoggingContext;

public readonly struct SendMessageBodyLogState
{
    public SendMessageBodyLogState(IpcMessageBody ipcMessageBody, string localPeerName, string remotePeerName)
    {
        IpcMessageBody = ipcMessageBody;
        LocalPeerName = localPeerName;
        RemotePeerName = remotePeerName;
    }

    public IpcMessageBody IpcMessageBody { get; }
    public string LocalPeerName { get; }
    public string RemotePeerName { get; }

    public string FormatAsText()
    {
        return
            $"Send from {LocalPeerName} To {RemotePeerName}: {Encoding.UTF8.GetString(IpcMessageBody.Buffer, IpcMessageBody.Start, IpcMessageBody.Length)}";
    }

    public string FormatAsBinary()
    {
        var length = IpcMessageBodyFormatter.GetSendHeaderLength(LocalPeerName, RemotePeerName)
                     + IpcMessageBodyFormatter.GetIpcMessageBodyAsBinaryLength(IpcMessageBody);
        var stringBuilder = new StringBuilder(length);
        IpcMessageBodyFormatter.AppendSendHeader(stringBuilder, LocalPeerName, RemotePeerName);
        IpcMessageBodyFormatter.AppendIpcMessageBodyAsBinary(stringBuilder, IpcMessageBody);

        return stringBuilder.ToString();
    }

    public static string Format(SendMessageBodyLogState state, Exception? exception)
    {
        return state.FormatAsBinary();
    }
}