using System;
using System.IO;

namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

public readonly struct JsonIpcDirectRoutedMessageLogState
{
    public JsonIpcDirectRoutedMessageLogState(string routedPath, string localPeerName, string remotePeerName, JsonIpcDirectRoutedLogStateMessageType messageType,
        MemoryStream stream)
    {
        RoutedPath = routedPath;
        LocalPeerName = localPeerName;
        RemotePeerName = remotePeerName;
        MessageType = messageType;
        Stream = stream;
    }

    public string RoutedPath { get; }
    public string LocalPeerName { get; }
    public string RemotePeerName { get; }
    public JsonIpcDirectRoutedLogStateMessageType MessageType { get; }
    private MemoryStream Stream { get; }

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

        return $"[JsonIpcDirectRouted][{action}] Path={state.RoutedPath} Remote={state.RemotePeerName} Local={state.LocalPeerName} Body={state.GetJsonText()}";
    }
}
