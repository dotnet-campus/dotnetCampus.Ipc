using System;
using System.Threading;
using System.Threading.Tasks;

using dotnetCampus.Threading;

namespace dotnetCampus.Ipc.Utils.Extensions
{
    static class DoubleBufferTaskExtensions
    {
        private static volatile int _count = 0;

        public static async Task AddTaskAsync(this DoubleBufferTask<Func<Task>> doubleBufferTask, Func<Task> task)
        {
            var count = Interlocked.Increment(ref _count);

            LoggerExtensions.Trace(null, $"[{count}] [AddTaskAsync]");
            var taskCompletionSource = new TaskCompletionSource<bool>();

            doubleBufferTask.AddTask(async () =>
            {
                LoggerExtensions.Trace(null, $"[{count}] [doubleBufferTask.AddTask]");
                try
                {
                    await task().ConfigureAwait(false);
                    LoggerExtensions.Trace(null, $"[{count}] [doubleBufferTask.AddTask] Completed");
                    taskCompletionSource.SetResult(true);
                }
                catch (Exception e)
                {
                    LoggerExtensions.Trace(null, $"[{count}] [doubleBufferTask.AddTask] Exception");
                    taskCompletionSource.SetException(e);
                }
            });

            await taskCompletionSource.Task.ConfigureAwait(false);
            LoggerExtensions.Trace(null, $"[{count}] [taskCompletionSource.Task] Completed");
        }
    }
}
