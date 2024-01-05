namespace IpcUno.Presentation
{
    public sealed partial class ServerPage : Page
    {
        public ServerPage()
        {
            this.InitializeComponent();            
        }

        public ServerViewModel ViewModel => (ServerViewModel) DataContext;
    }
}
