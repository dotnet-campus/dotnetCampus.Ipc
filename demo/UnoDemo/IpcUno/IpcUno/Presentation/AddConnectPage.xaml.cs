namespace IpcUno.Presentation
{
    public sealed partial class AddConnectPage : Page
    {
        public AddConnectPage()
        {
            this.InitializeComponent();
        }

        private void ConnectServerButton_OnClick(object sender, RoutedEventArgs e)
        {
            ServerConnecting?.Invoke(this, ServerNameTextBox.Text);
        }

        private void StartServerButton_OnClick(object sender, RoutedEventArgs e)
        {
            ServerStarting?.Invoke(this, ServerNameTextBox.Text);
            BuildServerName();
        }

        private void BuildServerName()
        {
            ServerNameTextBox.Text = System.IO.Path.GetRandomFileName();
        }

        public event EventHandler<string>? ServerConnecting;

        public event EventHandler<string>? ServerStarting;
    }
}
