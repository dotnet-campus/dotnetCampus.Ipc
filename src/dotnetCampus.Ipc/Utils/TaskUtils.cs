using System.Threading.Tasks;

namespace dotnetCampus.Ipc.Utils
{
    internal static class TaskUtils
    {
        public static async Task<TTarget> As<TSource, TTarget>(this Task<TSource> sourceTask) where TSource : TTarget
        {
            return await sourceTask.ConfigureAwait(false);
        }

#if NET45
        private static readonly Task TheCompletedTask = Task.FromResult(0);
#endif

        public static Task CompletedTask
        {
            get
            {
#if NET45
                return TheCompletedTask;
#else
                return Task.CompletedTask;
#endif
            }
        }
    }
}
