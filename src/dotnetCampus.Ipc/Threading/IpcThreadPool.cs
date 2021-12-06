using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Utils.Extensions;
using dotnetCampus.Ipc.Utils.Logging;

using Producer = System.Collections.Concurrent.BlockingCollection<int>;

namespace dotnetCampus.Ipc.Threading
{
    /// <summary>
    /// 提供给 IPC 框架内部使用的线程池。
    /// 专为调用时长不确定的业务代码而设计。
    /// 此类型的所有公共方法都不是线程安全的，因此你需要确保在临界区调用这些代码。
    /// 但内部方法是线程安全的。
    /// </summary>
    internal sealed class IpcThreadPool
    {
        /// <summary>
        /// 为每一个创建的线程名称准备一个序号。
        /// </summary>
        private volatile int _threadIndex;

        /// <summary>
        /// 目前正在执行 IPC 任务的线程总数（含正在执行任务和和正在等待执行任务的）。
        /// </summary>
        private volatile int _threadCount;

        /// <summary>
        /// 目前正在等待分配任务的线程数。
        /// </summary>
        private volatile int _availableCount;

        /// <summary>
        /// 最近一次回收线程的时间，如果短时间内大量触发则不回收。
        /// </summary>
        private DateTime _lastRecycleTime;

        /// <summary>
        /// 防止回收重入。
        /// </summary>
        private volatile int _isRecycling;

        /// <summary>
        /// 当前正在等待延时启动线程的任务。
        /// </summary>
        private readonly ConcurrentQueue<CancellationTokenSource> _delayStartWaitings = new();

        /// <summary>
        /// 任务队列，与 <see cref="_workingThreads"/> 配合使用。
        /// <para>这里放真实的任务。</para>
        /// <list type="bullet">
        /// <item>为什么不放到 <see cref="_workingThreads"/> 里呢？</item>
        /// <item>是因为那按时间顺序放的任务，到那里会等线程调度，真正执行的时候可能顺序就乱了。</item>
        /// <item>于是到真正调度到的时候再取任务，可以更大概率避免调度时间差导致的顺序问题。</item>
        /// </list>
        /// </summary>
        private readonly ConcurrentQueue<IpcStartEndTaskItem> _taskQueue = new();

        /// <summary>
        /// 线程队列，与 <see cref="_taskQueue"/> 配合使用。
        /// <para>这里放此刻可供调度的线程资源。</para>
        /// <list type="bullet">
        /// <item>存一个线程和其对应的调度器，不放具体的任务。</item>
        /// <item>不放具体任务是因为按时间顺序调度的任务，真正执行的时候可能顺序会乱了。</item>
        /// </list>
        /// </summary>
        private readonly ConcurrentQueue<(Thread thread, Producer producer)> _workingThreads = new();

        /// <summary>
        /// 在线程池挑选一个线程执行指定代码。
        /// 当任务确定已经开始执行之后就会返回第一层 <see cref="Task"/>，
        /// 在任务和超时时间先完成者会再次返回第二层 <see cref="Task"/>。
        /// 注意，当线程池中线程数量不足时，第一层 <see cref="Task"/> 的返回会延迟，直到空出新的可供执行的线程资源。
        /// </summary>
        /// <param name="action"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        /// <remarks>
        /// <para>特别注意！！！</para>
        /// 此方法不是线程安全的，调用方必须确保此方法的调用处于临界区。
        /// </remarks>
        public Task<Task> Run(Action action, ILogger? logger)
        {
            var taskItem = new IpcStartEndTaskItem(action);
            RunRecursively(taskItem, logger);
            return taskItem.AsTask();
        }

        private async void RunRecursively(IpcStartEndTaskItem taskItem, ILogger? logger)
        {
            // 如果打算开始一个任务，则从线程池中取一个线程。
            if (_workingThreads.TryDequeue(out var result))
            {
                // 线程池中有空闲线程。
                var producer = result.producer;
                _taskQueue.Enqueue(taskItem);
                producer.Add(0);
            }
            else
            {
                // 线程池中所有线程繁忙。
                var count = _threadCount;
                var delayTime = GetCurrentStartThreadDelayTime(count);
                if (delayTime == TimeSpan.Zero)
                {
                    // 线程池中虽没有空闲线程，但打算立即启动新线程。
                    var producer = new Producer();
                    StartThread(producer, logger);
                    _taskQueue.Enqueue(taskItem);
                    producer.Add(0);
                }
                else
                {
                    // 线程池中没有空闲线程，因此等待一段时间后重试。
                    Log(logger, $"因为线程资源紧张（{count} 个），延迟 {delayTime.TotalMilliseconds}ms 后重新调度。");
                    await DelayOrAnyThreadAvailable(delayTime);
                    // 等待结束后重跑线程调度。
                    RunRecursively(taskItem, logger);
                }
            }
        }

        private void StartThread(Producer c, ILogger? logger)
        {
            var index = Interlocked.Increment(ref _threadIndex);
            var t = new Thread(OnThreadStart)
            {
                Name = $"IPC-{index.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0')}",
                IsBackground = true,
            };
            var count = Interlocked.Increment(ref _threadCount);
            t.Start((c, logger));
        }

        private void OnThreadStart(object? arg)
        {
            var thread = Thread.CurrentThread;
            var (producer, logger) = ((Producer, ILogger?)) arg!;
            Interlocked.Increment(ref _availableCount);
            Log(logger, $"线程启动 {thread.Name}");
            foreach (var _ in producer.GetConsumingEnumerable())
            {
                if (!_taskQueue.TryDequeue(out var taskItem))
                {
                    throw new InvalidOperationException("100% BUG。先加入的任务，后面必定可以取到。");
                }
                var action = taskItem.Action;
                try
                {
                    // 开始执行任务。
                    Interlocked.Decrement(ref _availableCount);
                    taskItem.Start();
                    action();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("调用 IpcThreadPool.Run 方法时传入的动作必须完全自行处理好异常，不允许让任何异常泄漏出来。", ex);
                }
                finally
                {
                    // 完成执行任务。
                    taskItem.End();
                    // 回收线程（先回收线程，以防把本线程回收掉，导致任何任务都新开启线程执行）。
                    RecycleUselessThreads(logger);
                    // 重新回到线程池。
                    _workingThreads.Enqueue((thread, producer));
                    // 可用线程数 +1。
                    Interlocked.Increment(ref _availableCount);
                    // 如果有等待线程启动，则立即结束等待。
                    ClearWaitings();
                }
            }
            producer.Dispose();
            Log(logger, $"线程退出 {thread.Name}");
        }

        private void ClearWaitings()
        {
            while (_delayStartWaitings.TryDequeue(out var tokenSource))
            {
                tokenSource.Cancel();
            }
        }

        /// <summary>
        /// 等待一个制定的时间。但如果有任何一个线程空闲，则等待立即完成。
        /// </summary>
        /// <param name="delayTime"></param>
        /// <returns></returns>
        private async Task DelayOrAnyThreadAvailable(TimeSpan delayTime)
        {
            var taskSource = new CancellationTokenSource();
            _delayStartWaitings.Enqueue(taskSource);
            try
            {
                while (taskSource.IsCancellationRequested && delayTime > TimeSpan.Zero)
                {
                    await Task.Delay(15, taskSource.Token);
                    delayTime -= TimeSpan.FromMilliseconds(15);
                }
            }
            catch (OperationCanceledException)
            {
                // 等待已结束。
            }
        }

        private TimeSpan GetCurrentStartThreadDelayTime(int count)
        {
            if (_availableCount > 0)
            {
                // 如果当前存在可供使用的线程资源，则直接开启线程。
                return TimeSpan.Zero;
            }

            var delay = ThreadCountToDelayTime(count);
            return delay;
        }

        /// <summary>
        /// 根据当前已有的线程总数来决定新启动下一个线程的等待时间。
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TimeSpan ThreadCountToDelayTime(int count) => count switch
        {
            0 => TimeSpan.Zero,
            1 => TimeSpan.Zero,
            2 => TimeSpan.Zero,
            3 => TimeSpan.Zero,
            4 => TimeSpan.Zero,
            5 => TimeSpan.FromMilliseconds(250),
            6 => TimeSpan.FromMilliseconds(500),
            7 => TimeSpan.FromSeconds(1),
            8 => TimeSpan.FromSeconds(2),
            9 => TimeSpan.FromSeconds(4),
            10 => TimeSpan.FromSeconds(10),
            _ => TimeSpan.FromSeconds(30),
        };

        /// <summary>
        /// 回收线程
        /// </summary>
        /// <param name="logger"></param>
        private void RecycleUselessThreads(ILogger? logger)
        {
            var now = DateTime.Now;
            var elapsed = now - _lastRecycleTime;
            // 1 秒内最多回收一次线程。
            if (elapsed > TimeSpan.FromSeconds(1))
            {
                var isRecycling = Interlocked.CompareExchange(ref _isRecycling, 1, 0);
                if (isRecycling is 1)
                {
                    return;
                }

                _lastRecycleTime = now;
                // 期望回收数为当前空闲线程数的一半。

                try
                {
                    RecycleCore();
                }
                finally
                {
                    _isRecycling = 0;
                }
            }

            void RecycleCore()
            {
                var desiredDisposeCount = _workingThreads.Count / 2;
                while (desiredDisposeCount > 0)
                {
                    desiredDisposeCount--;
                    if (_workingThreads.TryDequeue(out var result))
                    {
                        var (thread, producer) = result;
                        Log(logger, $"回收线程 {thread.Name}");
                        producer.CompleteAdding();
                    }
                }
            }
        }

        private void Log(ILogger? logger, string message)
        {
            logger.Information($"[{nameof(IpcThreadPool)}] {message}");
        }
    }
}
