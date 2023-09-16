using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Utils.IO;
using dotnetCampus.Ipc.Utils.Logging;

namespace dotnetCampus.Ipc.Context.LoggingContext;

internal static class LoggerExtension
{
    /// <summary>
    /// 记录接收到业务消息
    /// </summary>
    /// <param name="context"></param>
    /// <param name="stream"></param>
    /// <param name="remotePeerName"></param>
    public static void LogReceiveMessage(this IpcContext context, ByteListMessageStream stream,
        string remotePeerName)
    {
        const LogLevel logLevel = LogLevel.Debug;
        if (!context.Logger.IsEnabled(logLevel))
        {
            return;
        }

        var ipcMessageBody = new IpcMessageBody(stream.GetBuffer(), (int) stream.Position,
            (int) (stream.Length - stream.Position));

        context.Logger.Log(logLevel, LoggerEventIds.ReceiveMessageEventId,
            new ReceiveMessageBodyLogState(ipcMessageBody, context.PipeName, remotePeerName, isBusinessMessage: true),
            null,
            ReceiveMessageBodyLogState.Format);
    }

    /// <summary>
    /// 记录接收到原始裸消息，包含消息头信息
    /// </summary>
    /// <param name="context"></param>
    /// <param name="ipcMessageResult"></param>
    /// <param name="remotePeerName"></param>
    public static void LogReceiveOriginMessage(this IpcContext context, IpcMessageResult ipcMessageResult,
        string? remotePeerName)
    {
        const LogLevel logLevel = LogLevel.Trace; // 一般框架信息是不关注的
        if (!context.Logger.IsEnabled(logLevel))
        {
            return;
        }

        var ipcMessageBody = new IpcMessageBody(ipcMessageResult.IpcMessageContext.MessageBuffer, 0,
            (int) ipcMessageResult.IpcMessageContext.MessageLength);
        context.Logger.Log(logLevel, LoggerEventIds.ReceiveOriginMessageEventId,
            new ReceiveMessageBodyLogState(ipcMessageBody, context.PipeName, remotePeerName, isBusinessMessage: false),
            null,
            ReceiveMessageBodyLogState.Format);
    }

    /// <summary>
    /// 记录发送消息
    /// </summary>
    /// <param name="context"></param>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <param name="remotePeerName"></param>
    public static void LogSendMessage(this IpcContext context, byte[] buffer, int offset, int count,
        string remotePeerName)
    {
        LogSendMessage(context, new IpcMessageBody(buffer, offset, count), remotePeerName);
    }

    /// <summary>
    /// 记录发送消息
    /// </summary>
    /// <param name="context"></param>
    /// <param name="ipcMessageBody"></param>
    /// <param name="remotePeerName"></param>
    public static void LogSendMessage(this IpcContext context, in IpcMessageBody ipcMessageBody, string remotePeerName)
    {
        const LogLevel logLevel = LogLevel.Debug;
        if (!context.Logger.IsEnabled(logLevel))
        {
            return;
        }

        context.Logger.Log(logLevel, LoggerEventIds.SendMessageEventId,
            new SendMessageBodyLogState(ipcMessageBody, context.PipeName, remotePeerName),
            null,
            SendMessageBodyLogState.Format);
    }

    /// <summary>
    /// 记录发送消息
    /// </summary>
    /// <param name="context"></param>
    /// <param name="message"></param>
    /// <param name="remotePeerName"></param>
    public static void LogSendMessage(this IpcContext context, in IpcBufferMessageContext message,
        string remotePeerName)
    {
        const LogLevel logLevel = LogLevel.Debug;
        if (!context.Logger.IsEnabled(logLevel))
        {
            return;
        }

        context.Logger.Log(logLevel, LoggerEventIds.SendMessageEventId,
            new SendMessageBodiesLogState(message.IpcBufferMessageList, context.PipeName, remotePeerName), null,
            SendMessageBodiesLogState.Format);
    }
}
