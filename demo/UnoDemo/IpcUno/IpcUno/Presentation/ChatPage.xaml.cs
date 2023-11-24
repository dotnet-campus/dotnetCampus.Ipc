using System.Reflection;
using System.Text;

using IpcUno.Utils;

namespace IpcUno.Presentation
{
    public sealed partial class ChatPage : Page
    {
        public ChatPage(ConnectedPeerModel model, string serverName)
        {
            DataContext = model;
            this.InitializeComponent();
            Model = model;
            ServerName = serverName;

            Loaded += ChatPage_Loaded;
        }

        private void ChatPage_Loaded(object sender, RoutedEventArgs e)
        {
            MessageListView.ScrollToBottom();

            // 有消息过来，自动滚动到最下
            Model.MessageList.CollectionChanged += (o, args) =>
            {
                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                 {
                     MessageListView.ScrollToBottom();
                 });
            };
        }

        public string ServerName { get; }

        public ConnectedPeerModel Model { get; }

        private async void SendButton_OnClick(object sender, RoutedEventArgs e)
        {
            Model.AddMessage(ServerName, MessageTextBox.Text);
            await Model.Peer.NotifyAsync(new dotnetCampus.Ipc.Messages.IpcMessage("CharPage", Encoding.UTF8.GetBytes(MessageTextBox.Text)));
        }
    }
}
