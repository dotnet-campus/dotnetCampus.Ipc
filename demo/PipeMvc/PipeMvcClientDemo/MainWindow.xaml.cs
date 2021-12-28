using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        if(_ipcPipeMvcClient is null)
        {
            return;
        }

        Log($"[Request] IpcPipeMvcServer://api/Foo");
        var response = await _ipcPipeMvcClient.GetStringAsync("api/Foo");
        Log($"[Response] IpcPipeMvcServer://api/Foo {response}");
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
}
