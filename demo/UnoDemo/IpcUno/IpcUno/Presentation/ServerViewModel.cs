using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace IpcUno.Presentation
{
    public partial class ServerViewModel : ObservableObject
    {
        public ServerViewModel(INavigator navigator)
        {
            _navigator = navigator;

            Task.Run(async () =>
            {
                while (true)
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        CurrentServerName = $"dotnet_campus {Path.GetRandomFileName()}";
                    });
                    await Task.Delay(100);
                }
            });
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
