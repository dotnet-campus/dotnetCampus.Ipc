using System;

using dotnetCampus.Ipc.Utils.Logging;

namespace dotnetCampus.Ipc.Utils.Extensions
{
    internal static class LoggerExtensions
    {
        public static void Trace(this ILogger? logger, string message)
        {
            logger?.Log(LogLevel.Trace, default, message, null, FormatOnlyMessage);
        }

        public static void Debug(this ILogger? logger, string message)
        {
            logger?.Log(LogLevel.Debug, default, message, null, FormatOnlyMessage);
        }

        public static void Information(this ILogger? logger, string message)
        {
            logger?.Log(LogLevel.Information, default, message, null, FormatOnlyMessage);
        }

        public static void Warning(this ILogger? logger, string message)
        {
            if (logger is null)
            {
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:hh:mm:ss.fff}] [IPC] [Warning] {message}");
            }
            else
            {
                logger?.Log(LogLevel.Warning, default, message, null, FormatOnlyMessage);
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
                logger?.Log(LogLevel.Error, default, message, null, FormatOnlyMessage);
            }
        }

        public static void Error(this ILogger? logger, Exception exception, string? message = null)
        {
            logger?.Log(LogLevel.Error, default, message, exception, StandardFormatMessage);
        }

        private static Func<string, Exception?, string> FormatOnlyMessage => static (s, _) => s;

        private static Func<string?, Exception?, string> StandardFormatMessage => static (s, e) =>
        {
            if (s is null && e != null)
            {
                return e.ToString();
            }
            return $"[IPC Error] {s} {e}";
        };
    }
}
