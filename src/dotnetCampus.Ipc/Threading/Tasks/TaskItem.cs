using System;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Utils.Logging;

namespace dotnetCampus.Ipc.Threading.Tasks
{
    internal abstract class TaskItem
    {
        protected TaskItem(ILogger? logger)
        {
            Logger = logger;
        }

        public ILogger? Logger { get; }

        internal abstract void Run();
    }

    internal sealed class TaskItem<T> : TaskItem
    {
        private readonly TaskCompletionSource<T> _source;

        public TaskItem(Func<Task<T>> func, ILogger? logger)
            : base(logger)
        {
            _source = new TaskCompletionSource<T>();
            Func = func;
        }

        public Func<Task<T>> Func { get; }

        public Task<T> AsTask() => _source.Task;

        internal override async void Run()
        {
            try
            {
                var value = await Func().ConfigureAwait(false);
                _source.SetResult(value);
            }
            catch (Exception ex)
            {
                _source.SetException(ex);
            }
        }
    }
}
