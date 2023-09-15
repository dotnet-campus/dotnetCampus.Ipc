using System;
using System.Linq;
using System.Text;
using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Context.LoggingContext;

public readonly struct SendMessageBodiesLogState
{
    internal SendMessageBodiesLogState(IpcMessageBody[] ipcBufferMessageList, string localPeerName,
        string remotePeerName)
    {
        IpcBufferMessageList = ipcBufferMessageList;
        LocalPeerName = localPeerName;
        RemotePeerName = remotePeerName;
    }

    public IpcMessageBody[] IpcBufferMessageList { get; }
    public string LocalPeerName { get; }
    public string RemotePeerName { get; }

    public string FormatAsText()
    {
        var result = $"Send from {LocalPeerName} To {RemotePeerName}: ";
        foreach (var ipcMessageBody in IpcBufferMessageList)
        {
            result += Encoding.UTF8.GetString(ipcMessageBody.Buffer, ipcMessageBody.Length, ipcMessageBody.Length);
        }

        return result;
    }

    public string FormatAsBinary()
    {
        // Send from {LocalPeerName} To {RemotePeerName}:
        var length = IpcMessageBodyFormatter.GetSendHeaderLength(LocalPeerName, RemotePeerName);
        length += IpcBufferMessageList.Sum(t => t.Length) * 3 /*一个byte转成两个字符加一个空格*/;
        var stringBuilder = new StringBuilder(length);
        IpcMessageBodyFormatter.AppendSendHeader(stringBuilder, LocalPeerName, RemotePeerName);

        bool isFirst = true;
        foreach (var ipcMessageBody in IpcBufferMessageList)
        {
            if (!isFirst)
            {
                // 不是第一个的，需要加上空格。因为上一段记录结束没有加上空格
                stringBuilder.Append(' ');
            }

            isFirst = false;

            IpcMessageBodyFormatter.AppendIpcMessageBodyAsBinary(stringBuilder, in ipcMessageBody);
        }

        return stringBuilder.ToString();
    }

    public static string Format(SendMessageBodiesLogState state, Exception? exception)
    {
        return state.FormatAsBinary();
    }
}