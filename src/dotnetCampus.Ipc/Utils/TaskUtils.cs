using System.Threading.Tasks;

namespace dotnetCampus.Ipc.Utils
{
    internal static class TaskUtils
    {
        public static async Task<TTarget> As<TSource, TTarget>(this Task<TSource> sourceTask) where TSource : TTarget
        {
            return await sourceTask.ConfigureAwait(false);
        }
    }
}
