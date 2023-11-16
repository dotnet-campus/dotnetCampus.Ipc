namespace IpcUno.Presentation
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public MainViewModel ViewModel => (MainViewModel) DataContext;
    }
}
