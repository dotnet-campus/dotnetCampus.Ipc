using System;
using dotnetCampus.Ipc.Internals;

namespace dotnetCampus.Ipc.Context
{
    internal class IpcPipeServerMessageProviderPeerConnectionBrokenArgs : EventArgs
    {
        public IpcPipeServerMessageProviderPeerConnectionBrokenArgs(IpcPipeServerMessageProvider ipcPipeServerMessageProvider, PeerConnectionBrokenArgs peerConnectionBrokenArgs)
        {
            IpcPipeServerMessageProvider = ipcPipeServerMessageProvider;
            PeerConnectionBrokenArgs = peerConnectionBrokenArgs;
        }

        public PeerConnectionBrokenArgs PeerConnectionBrokenArgs { get; }
        public IpcPipeServerMessageProvider IpcPipeServerMessageProvider { get; }
    }
}
