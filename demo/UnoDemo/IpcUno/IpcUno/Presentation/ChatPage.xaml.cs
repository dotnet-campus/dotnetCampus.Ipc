namespace IpcUno.Presentation
{
    public sealed partial class ChatPage : Page
    {
        public ChatPage(ConnectedPeerModel model, string serverName)
        {
            this.InitializeComponent();
            Model = model;
            ServerName = serverName;
        }

        public string ServerName { get; }

        public ConnectedPeerModel Model { get; }
    }
}
