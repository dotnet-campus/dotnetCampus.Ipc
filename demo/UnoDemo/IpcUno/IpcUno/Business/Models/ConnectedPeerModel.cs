using System.Collections.ObjectModel;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Pipes;

using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace IpcUno.Business.Models
{
    public class ConnectedPeerModel
    {
        public ConnectedPeerModel()
        {
            Peer = null!;
        }

        public ConnectedPeerModel(PeerProxy peer)
        {
            Peer = peer;
            peer.MessageReceived += Peer_MessageReceived;
        }

        private void Peer_MessageReceived(object? sender, IPeerMessageArgs e)
        {
            var streamReader = new StreamReader(e.Message.Body.ToMemoryStream());
            var message = streamReader.ReadToEnd();

            _ = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => 
            {
                AddMessage(PeerName, message);
            });
        }

        public void AddMessage(string name, string message)
        {
            MessageList.Add($"{name} {DateTime.Now:yyyy/MM/dd hh:mm:ss.fff}:\r\n{message}");
        }

        public ObservableCollection<string> MessageList { get; } = new ObservableCollection<string>();

        public PeerProxy Peer { get; }

        public string PeerName => Peer.PeerName;
    }
}
