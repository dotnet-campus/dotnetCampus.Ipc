using System;
using System.IO;

namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

/// <summary>
/// 用于日志记录 JsonIpcDirectRouted 消息的结构体
/// </summary>
public readonly struct JsonIpcDirectRoutedMessageLogState
{
    internal JsonIpcDirectRoutedMessageLogState(string routedPath, string localPeerName, string remotePeerName,
        JsonIpcDirectRoutedLogStateMessageType messageType,
        MemoryStream stream)
    {
        RoutedPath = routedPath;
        LocalPeerName = localPeerName;
        RemotePeerName = remotePeerName;
        MessageType = messageType;
        Stream = stream;
    }

    /// <summary>
    /// 路由地址
    /// </summary>
    public string RoutedPath { get; }

    /// <summary>
    /// 本地当前的 Peer 名
    /// </summary>
    public string LocalPeerName { get; }

    /// <summary>
    /// 远端对方的 Peer 名
    /// </summary>
    public string RemotePeerName { get; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public JsonIpcDirectRoutedLogStateMessageType MessageType { get; }

    private MemoryStream Stream { get; }

    /// <summary>
    /// 获取消息体的 Json 文本
    /// </summary>
    /// <returns></returns>
    public string GetJsonText()
    {
        var position = Stream.Position;
        var streamReader = new StreamReader(Stream);
        var jsonText = streamReader.ReadToEnd();
        Stream.Position = position;
        return jsonText;
    }

    /// <summary>
    /// 格式化
    /// </summary>
    /// <param name="state"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static string Format(JsonIpcDirectRoutedMessageLogState state, Exception? exception)
    {
        var action = state.MessageType switch
        {
            JsonIpcDirectRoutedLogStateMessageType.ReceiveNotify => "Receive Notify",
            JsonIpcDirectRoutedLogStateMessageType.ReceiveRequest => "Receive Request",
            JsonIpcDirectRoutedLogStateMessageType.SendResponse => "Send Response",

            JsonIpcDirectRoutedLogStateMessageType.SendNotify => "Send Notify",
            JsonIpcDirectRoutedLogStateMessageType.SendRequest => "Send Request",
            JsonIpcDirectRoutedLogStateMessageType.ReceiveResponse => "Receive Request",

            _ => string.Empty
        };

        return
            $"[JsonIpcDirectRouted][{action}] Path={state.RoutedPath} Remote={state.RemotePeerName} Local={state.LocalPeerName} Body={state.GetJsonText()}";
    }
}
