using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Utils.Logging;

namespace dotnetCampus.Ipc.Threading.Tasks
{
    /// <summary>
    /// 管理一组 IPC 任务。
    /// 加入此 <see cref="IpcTask"/> 的任务严格按照先到先执行的原则依次执行；
    /// 但与普通队列不同的是，一旦任务开始执行后，便可根据配置决定是否并发执行后续任务（而不必等待任务完全执行完成）；
    /// 并且支持限制并发数量（以避免潜在的性能影响）。
    /// </summary>
    internal sealed class IpcTask
    {
        private volatile int _isRunning;
        private readonly ConcurrentQueue<TaskItem> _queue = new();
        private readonly IIpcThreadPool _threadPool;

        /// <summary>
        /// 由于原子操作仅提供高性能的并发处理而不保证准确性，因此需要一个锁来同步 <see cref="_isRunning"/> 中值为 0 时所指的不确定情况。
        /// 不能使用一个锁来同步所有情况是因为在锁中使用 async/await 是不安全的，因此避免在锁中执行异步任务；我们使用原子操作来判断异步任务的执行条件。
        /// </summary>
        private readonly object _locker = new();

        public IpcTask(IIpcThreadPool threadPool)
        {
            _threadPool = threadPool;
        }

        /// <summary>
        /// 支持并发进入的 IPC 任务。
        /// 被此 <see cref="IpcTask"/> 管理的异步任务将按调用此方法的顺序依次开始执行，至于开始后多少个任务可以同时运行或者执行多长时间后算超时而执行下一个取决于此 <see cref="IpcTask"/> 的配置。
        /// </summary>
        /// <param name="action"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public Task Run(Action action, ILogger? logger = null) => Run(() =>
        {
            action();
            return Task.FromResult(0);
        }, logger);

        /// <summary>
        /// 支持并发进入的 IPC 任务。
        /// 被此 <see cref="IpcTask"/> 管理的异步任务将按调用此方法的顺序依次开始执行，至于开始后多少个任务可以同时运行或者执行多长时间后算超时而执行下一个取决于此 <see cref="IpcTask"/> 的配置。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public Task<T> Run<T>(Func<Task<T>> task, ILogger? logger = null)
        {
            var item = new TaskItem<T>(task, logger);
            _queue.Enqueue(item);
            ResumeRunning();
            return item.AsTask();
        }

        private async void ResumeRunning()
        {
            try
            {
                var isRunning = Interlocked.CompareExchange(ref _isRunning, 1, 0);
                if (isRunning is 1)
                {
                    lock (_locker)
                    {
                        if (_isRunning is 1)
                        {
                            // 当前已经在执行队列，因此无需继续执行。
                            return;
                        }
                    }
                }

                var hasTask = true;
                while (hasTask)
                {
                    // 当前还没有任何队列开始执行，因此需要开始执行队列。
                    while (_queue.TryDequeue(out var taskItem))
                    {
                        // 内部已包含异常处理，因此外面可以无需捕获或者清理。
                        await ConsumeTaskItemAsync(taskItem).ConfigureAwait(false);
                    }

                    lock (_locker)
                    {
                        hasTask = _queue.TryPeek(out _);
                        if (!hasTask)
                        {
                            _isRunning = 0;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // 当前是后台线程了，不能接受任何的抛出
                // 理论上这里框架层是不会抛出任何异常的
#if DEBUG
                Debugger.Break();
#endif
            }
        }

        /// <summary>
        /// 运行在其中一个调用线程中的临界区。
        /// 请勿执行耗时操作。
        /// </summary>
        /// <param name="taskItem"></param>
        private async Task ConsumeTaskItemAsync(TaskItem taskItem)
        {
            // 第一层等待，非常必要，这可以确保任务一定按顺序开始执行。
            var timeout = await _threadPool.Run(() => taskItem.Run(), taskItem.Logger).ConfigureAwait(false);
            // timeout 是第二层等待，即任务完全执行完成（而这里我们并不关心，所以无视）。
        }
    }
}
