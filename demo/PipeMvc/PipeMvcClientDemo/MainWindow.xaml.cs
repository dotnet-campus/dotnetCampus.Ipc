using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        var json = JsonSerializer.Serialize(new FooContent
        {
            Foo1 = "Foo PostFooWithArgumentButton",
            Foo2 = null,
        });
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _ipcPipeMvcClient.PostAsync("api/Foo/PostFoo", content);
        var m = await response.Content.ReadAsStringAsync();
        Log($"[Response][Post] IpcPipeMvcServer://api/Foo/PostFoo {response.StatusCode} {m}");
    }
}
