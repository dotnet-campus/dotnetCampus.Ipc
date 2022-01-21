using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using dotnetCampus.Ipc.PipeMvcClient;
using PipeMvcServerDemo;

namespace PipeMvcClientDemo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Log($"Start create PipeMvcClient.");

        var ipcPipeMvcClient = await IpcPipeMvcClientProvider.CreateIpcMvcClientAsync("PipeMvcServerDemo");
        _ipcPipeMvcClient = ipcPipeMvcClient;

        Log($"Finish create PipeMvcClient.");
    }

    private HttpClient? _ipcPipeMvcClient;

    private async void GetFooButton_Click(object sender, RoutedEventArgs e)
    {
        if (_ipcPipeMvcClient is null)
        {
            return;
        }

        Log($"[Request][Get] IpcPipeMvcServer://api/Foo");
        var response = await _ipcPipeMvcClient.GetStringAsync("api/Foo");
        Log($"[Response][Get] IpcPipeMvcServer://api/Foo {response}");
    }

    private void Log(string message)
    {
        Dispatcher.InvokeAsync(() =>
        {
            TraceTextBlock.Text += message + "\r\n";

            if (TraceTextBlock.Text.Length > 10000)
            {
                TraceTextBlock.Text = TraceTextBlock.Text[5000..];
            }
        });
    }

    private async void GetFooWithArgumentButton_Click(object sender, RoutedEventArgs e)
    {
        if (_ipcPipeMvcClient is null)
        {
            return;
        }

        Log($"[Request][Get] IpcPipeMvcServer://api/Foo/Add");
        var response = await _ipcPipeMvcClient.GetStringAsync("api/Foo/Add?a=1&b=1");
        Log($"[Response][Get] IpcPipeMvcServer://api/Foo/Add {response}");
    }

    private async void PostFooButton_Click(object sender, RoutedEventArgs e)
    {
        if (_ipcPipeMvcClient is null)
        {
            return;
        }

        Log($"[Request][Post] IpcPipeMvcServer://api/Foo");
        var response = await _ipcPipeMvcClient.PostAsync("api/Foo", new StringContent(""));
        var m = await response.Content.ReadAsStringAsync();
        Log($"[Response][Post] IpcPipeMvcServer://api/Foo {response.StatusCode} {m}");
    }

    private async void PostFooWithArgumentButton_Click(object sender, RoutedEventArgs e)
    {
        if (_ipcPipeMvcClient is null)
        {
            return;
        }

        Log($"[Request][Post] IpcPipeMvcServer://api/Foo");

        var json = JsonSerializer.Serialize(new FooContent { Foo1 = "Foo PostFooWithArgumentButton", Foo2 = null, });
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _ipcPipeMvcClient.PostAsync("api/Foo/PostFoo", content);
        var m = await response.Content.ReadAsStringAsync();
        Log($"[Response][Post] IpcPipeMvcServer://api/Foo/PostFoo {response.StatusCode} {m}");
    }

    private void MultiThreadButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (_ipcPipeMvcClient is null)
        {
            return;
        }

        var count = 10;
        for (int i = 0; i < count; i++)
        {
            var id = i;
            Task.Run(async () =>
            {
                Log($"[{id}][Request][Post] IpcPipeMvcServer://api/Foo");
                var response = await _ipcPipeMvcClient.PostAsync("api/Foo", new StringContent(""));
                var m = await response.Content.ReadAsStringAsync();
                Log($"[{id}][Response][Post] IpcPipeMvcServer://api/Foo {response.StatusCode} {m}");

                var a = Random.Shared.Next();
                var b = Random.Shared.Next();

                Log($"[{id}][Request][Get] IpcPipeMvcServer://api/Foo/Add?a={a}&b={b}");
                m = await _ipcPipeMvcClient.GetStringAsync($"api/Foo/Add?a={a}&b={b}");
                Log($"[{id}][Response][Get] IpcPipeMvcServer://api/Foo/Add {m} {a}+{b}={a + b}");

                Log($"[{id}][Request][Post] IpcPipeMvcServer://api/Foo");
                response = await _ipcPipeMvcClient.PostAsync("api/Foo", new StringContent(BuildRandomText()));
                m = await response.Content.ReadAsStringAsync();
                Log($"[Response][Post] IpcPipeMvcServer://api/Foo {response.StatusCode} {m}");

                Log($"[{id}][Request][Post] IpcPipeMvcServer://api/Foo");

                var json = JsonSerializer.Serialize(new FooContent
                {
                    Foo1 = Random.Shared.Next(2) == 1 ? BuildRandomText() : null,
                    Foo2 = Random.Shared.Next(2) == 1 ? BuildRandomText() : null,
                });
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                response = await _ipcPipeMvcClient.PostAsync("api/Foo/PostFoo", content);
                m = await response.Content.ReadAsStringAsync();
                Log($"[{id}][Response][Post] IpcPipeMvcServer://api/Foo/PostFoo {response.StatusCode} {m}");
            });
        }
    }

    private static string BuildRandomText()
    {
        var count = 10;
        var text = new StringBuilder();
        for (int i = 0; i < count; i++)
        {
            text.Append((char) Random.Shared.Next('a', 'z' + 1));
        }

        return text.ToString();
    }
}
