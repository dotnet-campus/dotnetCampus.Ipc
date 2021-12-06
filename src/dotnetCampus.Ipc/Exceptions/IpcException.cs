using System;

namespace dotnetCampus.Ipc.Exceptions
{
    /// <summary>
    /// 所有 IPC 相关的异常的基类。
    /// </summary>
    public class IpcException : Exception
    {
        /// <summary>
        /// 创建 <see cref="IpcException"/> 的新实例。
        /// </summary>
        public IpcException() : base()
        {
        }

        /// <summary>
        /// 创建带有自定义消息的 <see cref="IpcException"/> 的新实例。
        /// </summary>
        /// <param name="message">自定义消息。</param>
        public IpcException(string message) : base(message)
        {
        }

        /// <summary>
        /// 创建带有自定义消息和内部异常的 <see cref="IpcException"/> 的新实例。
        /// </summary>
        /// <param name="message">自定义消息。</param>
        /// <param name="innerException">内部异常。</param>
        public IpcException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
