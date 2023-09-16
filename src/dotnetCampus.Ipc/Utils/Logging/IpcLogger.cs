using System;
using System.Diagnostics;

namespace dotnetCampus.Ipc.Utils.Logging
{
    /// <summary>
    /// 为 IPC 提供日志输出。
    /// </summary>
    public class IpcLogger : ILogger
    {
        /// <summary>
        /// 创建为 IPC 提供的日志
        /// </summary>
        /// <param name="name">当前的 Ipc 名。等同于管道名</param>
        public IpcLogger(string name)
        {
            Name = name;
        }

        private string Name { get; }

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Log(logLevel, state, exception, formatter);
        }

        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return IsEnabled(logLevel);
        }

        /// <summary>
        /// 设置或获取最低的日志等级，只有大于此等级的日志才会被记录到 IpcLogger 里
        /// </summary>
        public LogLevel MinLogLevel { get; set; } = LogLevel.Information;

        /// <summary>
        /// 判断当前的日志等级是否可记
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        protected virtual bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= MinLogLevel;
        }

        /// <summary>
        /// 记录日志内容
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        protected virtual void Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Debug.WriteLine($"[IPC][{logLevel}]{formatter(state, exception)}");
        }

        /// <summary>
        /// 返回此日志的名字。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{Name}]";
        }
    }
}
