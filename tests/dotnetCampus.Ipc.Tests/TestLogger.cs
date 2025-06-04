using dotnetCampus.Ipc.Utils.Logging;

namespace dotnetCampus.Ipc.Tests;

class TestLogger : IpcLogger
{
    public TestLogger() : base(nameof(TestLogger))
    {
    }

    protected override bool IsEnabled(LogLevel logLevel)
    {
        return true; // 这里为了测试，全部都开启
    }

    protected override void Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        lock (LogMessage)
        {
            LogMessage.Add($"[{DateTime.Now:HH:mm:ss,fff}] [IPC][{logLevel}]{formatter(state, exception)}");
        }
    }

    public List<string> LogMessage { get; } = [];

    public string GetAllLogMessage()
    {
        lock (LogMessage)
        {
            return string.Join("\n", LogMessage);
        }
    }
}
