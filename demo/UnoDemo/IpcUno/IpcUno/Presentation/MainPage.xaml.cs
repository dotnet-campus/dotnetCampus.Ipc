namespace IpcUno.Presentation
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            DataContextChanged += MainPage_DataContextChanged;
        }

        private void MainPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

        }

        public MainViewModel ViewModel => (MainViewModel) DataContext;

        private void ConnectedPeerListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                var connectedPeerModel = (ConnectedPeerModel) e.AddedItems[0]!;
                MainPanelContentControl.Content = new ChatPage(connectedPeerModel);
            }
        }

        private void AddConnectButton_Click(object sender, RoutedEventArgs e)
        {
            AddConnectPage addConnectPage = new AddConnectPage();

            MainPanelContentControl.Content = addConnectPage;
        }
    }
}
