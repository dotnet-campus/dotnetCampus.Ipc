using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Context.LoggingContext;
using dotnetCampus.Ipc.Utils.Logging;

namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

internal static class JsonIpcDirectRoutedLoggerExtension
{
    public static void LogReceiveJsonIpcDirectRoutedRequest(this IpcContext context, string routedPath,
        string remotePeerName, MemoryStream stream)
    {
        const LogLevel logLevel = LogLevel.Debug;
        if (!context.Logger.IsEnabled(logLevel))
        {
            return;
        }

        var eventId = LoggerEventIds.ReceiveJsonIpcDirectRoutedRequestEventId;

        var state = new JsonIpcDirectRoutedMessageLogState(routedPath, context.PipeName, remotePeerName, JsonIpcDirectRoutedLogStateMessageType.Request, stream);

        context.Logger.Log(logLevel, eventId, state, null,
            JsonIpcDirectRoutedMessageLogState.Format);
    }

    public static void LogReceiveJsonIpcDirectRoutedNotify(this IpcContext context, string routedPath,
        string remotePeerName, MemoryStream stream)
    {
        const LogLevel logLevel = LogLevel.Debug;
        if (!context.Logger.IsEnabled(logLevel))
        {
            return;
        }

        var eventId = LoggerEventIds.ReceiveJsonIpcDirectRoutedNotifyEventId;

        var state = new JsonIpcDirectRoutedMessageLogState(routedPath, context.PipeName, remotePeerName, JsonIpcDirectRoutedLogStateMessageType.Notify, stream);

        context.Logger.Log(logLevel, eventId, state, null,
            JsonIpcDirectRoutedMessageLogState.Format);
    }
}
