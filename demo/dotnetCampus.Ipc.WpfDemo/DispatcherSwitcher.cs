using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;

// ReSharper disable once CheckNamespace
namespace Walterlv.ThreadSwitchingTasks
{
    public static class DispatcherSwitcher
    {
        public static ThreadPoolAwaiter ResumeBackground() => new ThreadPoolAwaiter();

        public static ThreadPoolAwaiter ResumeBackground(this Dispatcher dispatcher)
            => new ThreadPoolAwaiter();

        public static DispatcherAwaiter ResumeForeground(this Dispatcher dispatcher) =>
            new DispatcherAwaiter(dispatcher);

        public class ThreadPoolAwaiter : INotifyCompletion
        {
            public void OnCompleted(Action continuation)
            {
                Task.Run(() =>
                {
                    IsCompleted = true;
                    continuation();
                });
            }

            public bool IsCompleted { get; private set; }

            public void GetResult()
            {
            }

            public ThreadPoolAwaiter GetAwaiter() => this;
        }

        public class DispatcherAwaiter : INotifyCompletion
        {
            private readonly Dispatcher _dispatcher;

            public DispatcherAwaiter(Dispatcher dispatcher) => _dispatcher = dispatcher;

            public void OnCompleted(Action continuation)
            {
                _dispatcher.InvokeAsync(() =>
                {
                    IsCompleted = true;
                    continuation();
                });
            }

            public bool IsCompleted { get; private set; }

            public void GetResult()
            {
            }

            public DispatcherAwaiter GetAwaiter() => this;
        }
    }
}
