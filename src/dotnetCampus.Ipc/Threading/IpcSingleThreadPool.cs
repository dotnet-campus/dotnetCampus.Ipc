using System;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Utils.Logging;

namespace dotnetCampus.Ipc.Threading
{
    /// <summary>
    /// 提供给 IPC 框架内部使用的线程池。
    /// 所有的调度都将确定地按顺序执行，一个执行完毕后才会执行下一个。
    /// </summary>
    internal class IpcSingleThreadPool : IIpcThreadPool
    {
        public Task<Task> Run(Action action, ILogger? logger)
        {
            action();
            // 因为此方法的调用方能保证依次执行而不并发，所以这里直接 Task.Run 也不会浪费线程。
            //await Task.Run(action).ConfigureAwait(false);
            return Task.FromResult(Task.FromResult(0));
        }
    }
}
