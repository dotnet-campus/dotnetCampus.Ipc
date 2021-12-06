using System;

namespace dotnetCampus.Ipc.Exceptions
{
    /// <summary>
    /// 所有由远端问题导致的 IPC 异常。
    /// </summary>
    public class IpcRemoteException : IpcException
    {
        private readonly string? _remoteStackTrace;

        /// <summary>
        /// 创建 <see cref="IpcRemoteException"/> 的新实例。
        /// </summary>
        public IpcRemoteException()
        {
        }

        /// <summary>
        /// 创建带有自定义消息的 <see cref="IpcRemoteException"/> 的新实例。
        /// </summary>
        /// <param name="message">自定义消息。</param>
        public IpcRemoteException(string message) : base(message)
        {
        }

        /// <summary>
        /// 创建带有自定义消息和远端堆栈的 <see cref="IpcRemoteException"/> 的新实例。
        /// </summary>
        /// <param name="message">自定义消息。</param>
        /// <param name="remoteStackTrace">远端堆栈。</param>
        public IpcRemoteException(string message, string? remoteStackTrace) : base(message)
        {
            _remoteStackTrace = remoteStackTrace;
        }

        /// <summary>
        /// 创建带有自定义消息和内部异常的 <see cref="IpcRemoteException"/> 的新实例。
        /// </summary>
        /// <param name="message">自定义消息。</param>
        /// <param name="innerException">内部异常。</param>
        public IpcRemoteException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// 远端出现异常时的调用堆栈。
        /// </summary>
        public override string StackTrace => _remoteStackTrace ?? base.StackTrace;
    }
}
