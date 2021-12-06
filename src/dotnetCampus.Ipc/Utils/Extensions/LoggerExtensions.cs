using System;

using dotnetCampus.Ipc.Utils.Logging;

namespace dotnetCampus.Ipc.Utils.Extensions
{
    internal static class LoggerExtensions
    {
        public static void Trace(this ILogger? logger, string message)
        {
            logger?.Log(LogLevel.Trace, default, 0, null, (s, e) => message);
        }

        public static void Debug(this ILogger? logger, string message)
        {
            logger?.Log(LogLevel.Debug, default, 0, null, (s, e) => message);
        }

        public static void Information(this ILogger? logger, string message)
        {
            logger?.Log(LogLevel.Information, default, 0, null, (s, e) => message);
        }

        public static void Warning(this ILogger? logger, string message)
        {
            if (logger is null)
            {
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:hh:mm:ss.fff}] [IPC] [Warning] {message}");
            }
            else
            {
                logger?.Log(LogLevel.Warning, default, 0, null, (s, e) => message);
            }
        }

        public static void Error(this ILogger? logger, string message)
        {
            if (logger is null)
            {
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:hh:mm:ss.fff}] [IPC] [Error] {message}");
            }
            else
            {
                logger?.Log(LogLevel.Error, default, 0, null, (s, e) => message);
            }
        }

        public static void Error(this ILogger? logger, Exception exception, string? message = null)
        {
            logger?.Log(LogLevel.Error, default, 0, exception, (s, e) => message ?? "");
        }
    }
}
