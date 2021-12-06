using System;
using System.Threading.Tasks;

namespace dotnetCampus.Ipc.Threading
{
    internal sealed class IpcStartEndTaskItem
    {
        private readonly TaskCompletionSource<TaskCompletionSource<bool>> _actionStartedTaskSource = new();
        private readonly TaskCompletionSource<bool> _actionEndedTaskSource = new();

        public IpcStartEndTaskItem(Action action)
        {
            Action = action;
        }

        public Action Action { get; }

        public async Task<Task> AsTask()
        {
            var endTask = await _actionStartedTaskSource.Task.ConfigureAwait(false);
            return endTask.Task;
        }

        internal void Start()
        {
            _actionStartedTaskSource.SetResult(_actionEndedTaskSource);
        }

        internal void End()
        {
            _actionEndedTaskSource.SetResult(true);
        }
    }
}
