using System;
using System.Linq;
using System.Text;
using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Context.LoggingContext;

/// <summary>
/// 发送消息的日志信息
/// </summary>
public readonly struct SendMessageBodiesLogState
{
    /// <summary>
    /// 发送消息的日志信息
    /// </summary>
    internal SendMessageBodiesLogState(IpcMessageBody[] ipcBufferMessageList, string localPeerName,
        string remotePeerName)
    {
        IpcBufferMessageList = ipcBufferMessageList;
        LocalPeerName = localPeerName;
        RemotePeerName = remotePeerName;
    }

    /// <summary>
    /// 发送的消息内容
    /// </summary>
    public IpcMessageBody[] IpcBufferMessageList { get; }

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
        var result = $"Send from {LocalPeerName} To {RemotePeerName}: ";
        foreach (var ipcMessageBody in IpcBufferMessageList)
        {
            result += Encoding.UTF8.GetString(ipcMessageBody.Buffer, ipcMessageBody.Length, ipcMessageBody.Length);
        }

        return result;
    }

    /// <summary>
    /// 格式化二进制文本
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 格式化
    /// </summary>
    /// <param name="state"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static string Format(SendMessageBodiesLogState state, Exception? exception)
    {
        return state.FormatAsBinary();
    }
}
