namespace IpcUno.Presentation
{
    public partial class ServerViewModel : ObservableObject
    {
        public ServerViewModel(INavigator navigator)
        {
            _navigator = navigator;
        }

        private readonly INavigator _navigator;

        [ObservableProperty]
        private string _currentServerName = "dotnet_campus";

        [RelayCommand]
        private void NavigateMainPage()
        {
            _ = _navigator.NavigateViewModelAsync<MainViewModel>(this, data: new IpcServerEntity(CurrentServerName));
        }
    }
}
