global using dotnetCampus.Ipc.Tests.Properties;

namespace dotnetCampus.Ipc.Tests.Properties;

public static class Compatibility
{
#if !NET5_0_OR_GREATER
    public static async Task<T> WaitAsync<T>(this Task<T> task, TimeSpan timeout)
    {
        var timeoutTask = Task.Delay(timeout);
        var completedTask = await Task.WhenAny(task, timeoutTask);
        if (completedTask == timeoutTask)
        {
            throw new TimeoutException("The operation has timed out.");
        }

        return await task;
    }
#endif
}

#if !NET5_0_OR_GREATER
public sealed class TaskCompletionSource
{
    private readonly TaskCompletionSource<bool> _tcs = new();

    public Task Task => _tcs.Task;

    public void SetResult() => _tcs.SetResult(true);

    public void TrySetResult() => _tcs.TrySetResult(true);
}
#endif
