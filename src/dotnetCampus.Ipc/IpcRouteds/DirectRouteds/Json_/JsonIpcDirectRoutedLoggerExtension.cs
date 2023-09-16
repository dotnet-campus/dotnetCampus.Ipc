using System.IO;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Context.LoggingContext;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Utils.Logging;

namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

internal static class JsonIpcDirectRoutedLoggerExtension
{
    /// <summary>
    /// [客户端] 记录发送请求
    /// </summary>
    /// <param name="context"></param>
    /// <param name="routedPath"></param>
    /// <param name="remotePeerName"></param>
    /// <param name="ipcMessageBody"></param>
    public static void LogSendJsonIpcDirectRoutedRequest(this IpcContext context, string routedPath,
        string remotePeerName, in IpcMessageBody ipcMessageBody)
    {
        const LogLevel logLevel = LogLevel.Debug;
        if (!context.Logger.IsEnabled(logLevel))
        {
            return;
        }

        var eventId = LoggerEventIds.ReceiveJsonIpcDirectRoutedResponseEventId;
        var stream = ipcMessageBody.ToMemoryStream();
        var state = new JsonIpcDirectRoutedMessageLogState(routedPath, context.PipeName, remotePeerName, JsonIpcDirectRoutedLogStateMessageType.SendRequest, stream);

        context.Logger.Log(logLevel, eventId, state, null,
            JsonIpcDirectRoutedMessageLogState.Format);
    }

    /// <summary>
    /// [客户端] 记录收到响应
    /// </summary>
    /// <param name="context"></param>
    /// <param name="routedPath"></param>
    /// <param name="remotePeerName"></param>
    /// <param name="stream"></param>
    public static void LogReceiveJsonIpcDirectRoutedResponse(this IpcContext context, string routedPath,
        string remotePeerName, MemoryStream stream)
    {
        const LogLevel logLevel = LogLevel.Debug;
        if (!context.Logger.IsEnabled(logLevel))
        {
            return;
        }

        var eventId = LoggerEventIds.ReceiveJsonIpcDirectRoutedResponseEventId;
        var state = new JsonIpcDirectRoutedMessageLogState(routedPath, context.PipeName, remotePeerName, JsonIpcDirectRoutedLogStateMessageType.ReceiveResponse, stream);

        context.Logger.Log(logLevel, eventId, state, null,
            JsonIpcDirectRoutedMessageLogState.Format);
    }

    /// <summary>
    /// [客户端] 记录发送通知
    /// </summary>
    /// <param name="context"></param>
    /// <param name="routedPath"></param>
    /// <param name="remotePeerName"></param>
    /// <param name="ipcMessageBody"></param>
    public static void LogSendJsonIpcDirectRoutedNotify(this IpcContext context, string routedPath,
        string remotePeerName, in IpcMessageBody ipcMessageBody)
    {
        const LogLevel logLevel = LogLevel.Debug;
        if (!context.Logger.IsEnabled(logLevel))
        {
            return;
        }

        var eventId = LoggerEventIds.SendJsonIpcDirectRoutedNotifyEventId;
        using var stream = ipcMessageBody.ToMemoryStream();
        var state = new JsonIpcDirectRoutedMessageLogState(routedPath, context.PipeName, remotePeerName, JsonIpcDirectRoutedLogStateMessageType.SendNotify, stream);

        context.Logger.Log(logLevel, eventId, state, null,
            JsonIpcDirectRoutedMessageLogState.Format);
    }

    /// <summary>
    /// [服务端] 记录发送响应
    /// </summary>
    /// <param name="context"></param>
    /// <param name="routedPath"></param>
    /// <param name="remotePeerName"></param>
    /// <param name="ipcMessageBody"></param>
    public static void LogSendJsonIpcDirectRoutedResponse(this IpcContext context, string routedPath,
        string remotePeerName, in IpcMessageBody ipcMessageBody)
    {
        const LogLevel logLevel = LogLevel.Debug;
        if (!context.Logger.IsEnabled(logLevel))
        {
            return;
        }

        var eventId = LoggerEventIds.SendJsonIpcDirectRoutedResponseEventId;
        using var stream = ipcMessageBody.ToMemoryStream();
        var state = new JsonIpcDirectRoutedMessageLogState(routedPath, context.PipeName, remotePeerName, JsonIpcDirectRoutedLogStateMessageType.SendResponse, stream);

        context.Logger.Log(logLevel, eventId, state, null,
            JsonIpcDirectRoutedMessageLogState.Format);
    }

    /// <summary>
    /// [服务端] 记录收到请求
    /// </summary>
    /// <param name="context"></param>
    /// <param name="routedPath"></param>
    /// <param name="remotePeerName"></param>
    /// <param name="stream"></param>
    public static void LogReceiveJsonIpcDirectRoutedRequest(this IpcContext context, string routedPath,
        string remotePeerName, MemoryStream stream)
    {
        const LogLevel logLevel = LogLevel.Debug;
        if (!context.Logger.IsEnabled(logLevel))
        {
            return;
        }

        var eventId = LoggerEventIds.ReceiveJsonIpcDirectRoutedRequestEventId;

        var state = new JsonIpcDirectRoutedMessageLogState(routedPath, context.PipeName, remotePeerName, JsonIpcDirectRoutedLogStateMessageType.ReceiveRequest, stream);

        context.Logger.Log(logLevel, eventId, state, null,
            JsonIpcDirectRoutedMessageLogState.Format);
    }

    /// <summary>
    /// [服务端] 记录发送通知
    /// </summary>
    /// <param name="context"></param>
    /// <param name="routedPath"></param>
    /// <param name="remotePeerName"></param>
    /// <param name="stream"></param>
    public static void LogReceiveJsonIpcDirectRoutedNotify(this IpcContext context, string routedPath,
        string remotePeerName, MemoryStream stream)
    {
        const LogLevel logLevel = LogLevel.Debug;
        if (!context.Logger.IsEnabled(logLevel))
        {
            return;
        }

        var eventId = LoggerEventIds.ReceiveJsonIpcDirectRoutedNotifyEventId;

        var state = new JsonIpcDirectRoutedMessageLogState(routedPath, context.PipeName, remotePeerName, JsonIpcDirectRoutedLogStateMessageType.ReceiveNotify, stream);

        context.Logger.Log(logLevel, eventId, state, null,
            JsonIpcDirectRoutedMessageLogState.Format);
    }
}
