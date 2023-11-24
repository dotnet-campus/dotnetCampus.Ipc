using System.Reflection;
using System.Text;

namespace IpcUno.Presentation
{
    public sealed partial class ChatPage : Page
    {
        public ChatPage(ConnectedPeerModel model, string serverName)
        {
            DataContextChanged += ChatPage_DataContextChanged;
            DataContext = model;
            this.InitializeComponent();
            Model = model;
            ServerName = serverName;
        }

        private void ChatPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
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
