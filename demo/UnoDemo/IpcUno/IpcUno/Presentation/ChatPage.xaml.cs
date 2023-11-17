namespace IpcUno.Presentation
{
    public sealed partial class ChatPage : Page
    {
        public ChatPage(ConnectedPeerModel model)
        {
            this.InitializeComponent();
            Model = model;
        }

        public ConnectedPeerModel Model { get; }
    }
}
