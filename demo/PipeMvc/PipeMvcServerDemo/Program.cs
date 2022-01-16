using System.Windows;
using System.Windows.Threading;

using dotnetCampus.Ipc.PipeMvcServer;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PipeMvcServerDemo;

public class Program
{
    static void Main(string[] args)
    {
        RunWpf();
        RunMvc(args);
    }

    private static void RunWpf()
    {
        Thread wpfUIThread = new Thread(() =>
        {
            Application application = new Application();
            application.Startup += (s, e) =>
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            };
            application.Run();
        });
        wpfUIThread.IsBackground = true;
        wpfUIThread.SetApartmentState(ApartmentState.STA);
        wpfUIThread.Start();
    }

    private static void RunMvc(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UsePipeIpcServer("PipeMvcServerDemo");
        builder.Services.AddControllers();
        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddProvider(new DemoLogProvider());
        });
        var app = builder.Build();
        app.MapControllers();
        app.Run();
    }

    class DemoLogProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new DemoLogger();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        class DemoLogger : ILogger
        {
            public IDisposable BeginScope<TState>(TState state)
            {
                return new Empty();
            }

            class Empty : IDisposable
            {
                /// <inheritdoc />
                public void Dispose()
                {
                }
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                var message =
                        $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}][{logLevel}][EventId={eventId.Id}:{eventId.Name}] {formatter(state, exception)}";
                Application.Current?.Dispatcher.InvokeAsync(() =>
                {
                    var mainWindow = (MainWindow) Application.Current.MainWindow;
                    mainWindow.Log(message);
                });
            }
        }
    }
}
