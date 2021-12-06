using System;
using System.Threading.Tasks;

namespace dotnetCampus.Ipc.Tests
{
    static class TaskExtension
    {
        public static async Task WaitTimeout(this Task task, TimeSpan? timeout = null)
        {
            timeout ??= TimeSpan.FromSeconds(2);
            await Task.WhenAny(task, Task.Delay(timeout.Value));

#if DEBUG
            // 进入断点，也许上面的时间太短
            if (!task.IsCompleted)
            {
                await Task.WhenAny(task, Task.Delay(TimeSpan.FromMinutes(5)));
            }
#endif
        }
    }
}
