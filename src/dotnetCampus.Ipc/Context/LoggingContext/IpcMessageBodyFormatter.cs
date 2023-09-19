using System.Text;
using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Context.LoggingContext;

static class IpcMessageBodyFormatter
{
    public static int GetSendHeaderLength(string localPeerName, string remotePeerName)
    {
        // Send from {LocalPeerName} To {RemotePeerName}:
        return SendText.Length + FromText.Length + localPeerName.Length + ToText.Length +
               remotePeerName.Length +
               " : ".Length;
    }

    public static void AppendSendHeader(StringBuilder stringBuilder, string localPeerName, string remotePeerName)
    {
        stringBuilder.Append(SendText)
            .Append(FromText)
            .Append(localPeerName)
            .Append(ToText)
            .Append(remotePeerName)
            .Append(" : ");
    }

    public static int GetReceiveHeaderLength(string localPeerName, string remotePeerName)
    {
        // Receive from {RemotePeerName} To {LocalPeerName}:
        return ReceiveText.Length + FromText.Length + localPeerName.Length + ToText.Length +
               remotePeerName.Length +
               " : ".Length;
    }

    public static void AppendReceiveHeader(StringBuilder stringBuilder, string localPeerName, string remotePeerName)
    {
        stringBuilder.Append(ReceiveText)
            .Append(FromText)
            .Append(remotePeerName)
            .Append(ToText)
            .Append(localPeerName)
            .Append(" : ");
    }

    public static int GetIpcMessageBodyAsBinaryLength(IpcMessageBody ipcMessageBody)
    {
        return ipcMessageBody.Length * 3 /*一个byte转成两个字符加一个空格*/;
    }

    public static void AppendIpcMessageBodyAsBinary(StringBuilder stringBuilder, in IpcMessageBody ipcMessageBody)
    {
        for (int i = ipcMessageBody.Start; i < ipcMessageBody.Length; i++)
        {
            stringBuilder.Append(ipcMessageBody.Buffer[i].ToString("X2"));
            if (i != ipcMessageBody.Length - 1)
            {
                // 不是最后一个
                stringBuilder.Append(' ');
            }
        }
    }

    private const string ReceiveText = "Receive ";
    private const string SendText = "Send ";
    private const string FromText = "from ";
    private const string ToText = " To ";
}
