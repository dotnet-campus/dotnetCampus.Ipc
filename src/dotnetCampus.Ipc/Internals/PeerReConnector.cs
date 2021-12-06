using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Pipes;

namespace dotnetCampus.Ipc.Internals
{
    /// <summary>
    /// 重新连接器
    /// </summary>
    class PeerReConnector
    {
        public PeerReConnector(PeerProxy peerProxy, IpcProvider ipcProvider)
        {
            _peerProxy = peerProxy;
            _ipcProvider = ipcProvider;

            peerProxy.PeerConnectionBroken += PeerProxy_PeerConnectionBroken;
        }

        private void PeerProxy_PeerConnectionBroken(object? sender, IPeerConnectionBrokenArgs e)
        {
            Reconnect();
        }

        private readonly PeerProxy _peerProxy;
        private readonly IpcProvider _ipcProvider;

        private async void Reconnect()
        {
            var ipcClientService = _ipcProvider.CreateIpcClientService(_peerProxy.PeerName);
             await ipcClientService.Start();
            _peerProxy.Reconnect(ipcClientService);
        }
    }
}
