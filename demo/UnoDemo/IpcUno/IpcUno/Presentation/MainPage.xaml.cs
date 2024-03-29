using System.Diagnostics;

using IpcUno.Utils;

namespace IpcUno.Presentation
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            DataContextChanged += MainPage_DataContextChanged;
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ShowAddConnectPage();
        }

        private void MainPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            // 在这个时机可以拿到 ViewModel 对象
            ViewModel.AddedLogMessage += ViewModel_AddedLogMessage;
        }

        private void ViewModel_AddedLogMessage(object? sender, string message)
        {
            // 收到日志
            LogTextBox.Text += $"{DateTime.Now:hh:mm:ss,fff} {message}\r\n";
            // 延迟一下，防止界面还没刷新就执行滚动
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                LogTextBox.ScrollToBottom();
            });
        }

        public MainViewModel ViewModel => (MainViewModel) DataContext;

        private void ConnectedPeerListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                var connectedPeerModel = (ConnectedPeerModel) e.AddedItems[0]!;
                MainPanelContentControl.Content = new ChatPage(connectedPeerModel, ViewModel.CurrentServerName);
            }
        }

        private void AddConnectButton_Click(object sender, RoutedEventArgs e)
        {
            ShowAddConnectPage();
        }

        private void ShowAddConnectPage()
        {
            ConnectedPeerListView.SelectedItem = null;

            AddConnectPage addConnectPage = new AddConnectPage();
            addConnectPage.ServerConnecting += async (s, e) =>
            {
                var serverName = e;
                await ViewModel.ConnectAsync(serverName);

                ConnectedPeerListView.SelectedItem = ViewModel.ConnectedPeerModelList.FirstOrDefault(t => t.PeerName == serverName);
            };

            MainPanelContentControl.Content = addConnectPage;
        }
    }
}
