using System;
using System.Text;
using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Context.LoggingContext;

/// <summary>
/// 发送消息的日志信息
/// </summary>
public readonly struct SendMessageBodyLogState
{
    /// <summary>
    /// 发送消息的日志信息
    /// </summary>
    public SendMessageBodyLogState(IpcMessageBody ipcMessageBody, string localPeerName, string remotePeerName)
    {
        IpcMessageBody = ipcMessageBody;
        LocalPeerName = localPeerName;
        RemotePeerName = remotePeerName;
    }

    /// <summary>
    /// 发送的消息内容
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
        return
            $"Send from {LocalPeerName} To {RemotePeerName}: {Encoding.UTF8.GetString(IpcMessageBody.Buffer, IpcMessageBody.Start, IpcMessageBody.Length)}";
    }

    /// <summary>
    /// 格式化二进制文本
    /// </summary>
    /// <returns></returns>
    public string FormatAsBinary()
    {
        var length = IpcMessageBodyFormatter.GetSendHeaderLength(LocalPeerName, RemotePeerName)
                     + IpcMessageBodyFormatter.GetIpcMessageBodyAsBinaryLength(IpcMessageBody);
        var stringBuilder = new StringBuilder(length);
        IpcMessageBodyFormatter.AppendSendHeader(stringBuilder, LocalPeerName, RemotePeerName);
        IpcMessageBodyFormatter.AppendIpcMessageBodyAsBinary(stringBuilder, IpcMessageBody);

        return stringBuilder.ToString();
    }

    /// <summary>
    /// 格式化
    /// </summary>
    /// <param name="state"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static string Format(SendMessageBodyLogState state, Exception? exception)
    {
        return state.FormatAsBinary();
    }
}
