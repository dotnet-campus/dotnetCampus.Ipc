using System.Collections.ObjectModel;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Pipes;

using Microsoft.UI.Dispatching;

using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace IpcUno.Presentation
{
    public partial class MainViewModel : ObservableObject, IInjectable<IServiceProvider>
    {
        [ObservableProperty]
        private string _currentServerName = "dotnet_campus";
        private readonly IpcProvider _ipcProvider;

        public ObservableCollection<ConnectedPeerModel> ConnectedPeerModelList { get; } = new ObservableCollection<ConnectedPeerModel>();

        public MainViewModel(IpcServerEntity entity)
        {
            _currentServerName = entity.Name;
            _ipcProvider = new IpcProvider(_currentServerName);
            _ipcProvider.PeerConnected += IpcProvider_PeerConnected;
            _ipcProvider.StartServer();
        }

        private void IpcProvider_PeerConnected(object? sender, dotnetCampus.Ipc.Context.PeerConnectedArgs e)
        {
            _ = AddPeer(e.Peer);
        }

        private async Task AddPeer(PeerProxy peer)
        {
            //var dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread(); // WinUI: null
            //var currentView = CoreApplication.GetCurrentView();// WinUI: System.Runtime.InteropServices.COMException:“Element not found.
            //var dispatcher = ((IpcUno.App) IpcUno.App.Current).Dispatcher;
            //await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    var currentPeer = ConnectedPeerModelList.FirstOrDefault(temp => temp.PeerName == peer.PeerName);
            //    if (currentPeer != null)
            //    {
            //        currentPeer.Peer.PeerConnectionBroken -= Peer_PeerConnectBroke;
            //        ConnectedPeerModelList.Remove(currentPeer);
            //    }

            //    ConnectedPeerModelList.Add(new ConnectedPeerModel(peer));

            //    peer.PeerConnectionBroken += Peer_PeerConnectBroke;
            //});

            var dispatcher = ((IpcUno.App) IpcUno.App.Current).Dispatcher;
            dispatcher.TryEnqueue(() =>
            {
                var currentPeer = ConnectedPeerModelList.FirstOrDefault(temp => temp.PeerName == peer.PeerName);
                if (currentPeer != null)
                {
                    currentPeer.Peer.PeerConnectionBroken -= Peer_PeerConnectBroke;
                    ConnectedPeerModelList.Remove(currentPeer);
                }

                ConnectedPeerModelList.Add(new ConnectedPeerModel(peer));

                peer.PeerConnectionBroken += Peer_PeerConnectBroke;
            });
        }

        private void Peer_PeerConnectBroke(object? sender, IPeerConnectionBrokenArgs e)
        {
            var peer = (PeerProxy) sender!;
            Log($"[连接断开] {peer.PeerName}");
        }

        private void Log(string message)
        {

        }

        public void Inject(IServiceProvider entity)
        {
        }

        public async Task ConnectAsync(string serverName)
        {
            var peer = await _ipcProvider.GetAndConnectToPeerAsync(serverName).ConfigureAwait(false);
            await AddPeer(peer);
        }
    }
}
