using System;
using System.Text;

using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Context.LoggingContext;

/// <summary>
/// 收到的消息
/// </summary>
public readonly struct ReceiveMessageLog
{
    /// <summary>
    /// 收到的消息
    /// </summary>
    public ReceiveMessageLog(IpcMessageBody ipcMessageBody, string localPeerName, string? remotePeerName, bool isBusinessMessage)
    {
        IpcMessageBody = ipcMessageBody;
        LocalPeerName = localPeerName;
        IsBusinessMessage = isBusinessMessage;
        RemotePeerName = remotePeerName ?? string.Empty;
    }

    /// <summary>
    /// 是否业务消息。如果不是，那消息包含消息头
    /// </summary>
    public bool IsBusinessMessage { get; }

    /// <summary>
    /// 消息内容
    /// </summary>
    public IpcMessageBody IpcMessageBody { get; }
    /// <summary>
    /// 本地当前的 Peer 名
    /// </summary>
    public string LocalPeerName { get; }
    /// <summary>
    /// 远端对方的 Peer 名
    /// </summary>
    public string RemotePeerName { get; }

    /// <summary>
    /// 格式化为 UTF8 字符串
    /// </summary>
    /// <returns></returns>
    public string FormatAsText()
    {
        return $"Receive from {RemotePeerName} To {LocalPeerName}: {Encoding.UTF8.GetString(IpcMessageBody.Buffer, IpcMessageBody.Start, IpcMessageBody.Length)}";
    }

    /// <summary>
    /// 格式化二进制文本
    /// </summary>
    /// <returns></returns>
    public string FormatAsBinary()
    {
        var length = IpcMessageBodyFormatter.GetReceiveHeaderLength(LocalPeerName, RemotePeerName)
                     + IpcMessageBodyFormatter.GetIpcMessageBodyAsBinaryLength(IpcMessageBody);
        var stringBuilder = new StringBuilder(length);
        IpcMessageBodyFormatter.AppendReceiveHeader(stringBuilder, LocalPeerName, RemotePeerName);
        IpcMessageBodyFormatter.AppendIpcMessageBodyAsBinary(stringBuilder, IpcMessageBody);

        return stringBuilder.ToString();
    }

    /// <summary>
    /// 格式化
    /// </summary>
    /// <param name="state"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static string Format(ReceiveMessageLog state, Exception? exception)
    {
        return state.FormatAsBinary();
    }
}
