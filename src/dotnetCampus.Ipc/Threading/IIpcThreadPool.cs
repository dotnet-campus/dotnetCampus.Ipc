using System;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Utils.Logging;

namespace dotnetCampus.Ipc.Threading
{
    /// <summary>
    /// 提供给 IPC 框架内部使用的线程池。
    /// </summary>
    internal interface IIpcThreadPool
    {
        /// <summary>
        /// 在线程池挑选一个线程执行指定代码。
        /// 当任务确定已经开始执行之后就会返回第一层 <see cref="Task"/>，
        /// 在任务和超时时间先完成者会再次返回第二层 <see cref="Task"/>。
        /// </summary>
        /// <param name="action"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        /// <remarks>
        /// <para>特别注意！！！</para>
        /// 此方法不是线程安全的，调用方必须确保此方法的调用处于临界区。
        /// </remarks>
        Task<Task> Run(Action action, ILogger? logger);
    }

    /// <summary>
    /// 自定义的 IPC 所采用的线程池
    /// </summary>
    public abstract class CustomIpcThreadPoolBase : IIpcThreadPool
    {
        Task<Task> IIpcThreadPool.Run(Action action, ILogger? logger)
        {
            return Run(action);
        }

        /// <inheritdoc cref="IIpcThreadPool.Run"/>
        protected abstract Task<Task> Run(Action action);
    }
}
