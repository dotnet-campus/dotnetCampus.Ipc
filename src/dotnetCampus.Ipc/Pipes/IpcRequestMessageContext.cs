using System.Diagnostics;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Pipes
{
    class IpcRequestMessageContext : ICoreIpcRequestContext, IIpcRequestContext
    {
        [DebuggerStepThrough]
        internal IpcRequestMessageContext(IpcMessage ipcBufferMessage, IPeerProxy peer, CoreMessageType coreMessageType)
        {
            IpcBufferMessage = ipcBufferMessage;
            Peer = peer;
            CoreMessageType = coreMessageType;
        }

        /// <inheritdoc />
        public bool Handled { get; set; }

        /// <inheritdoc />
        public IpcMessage IpcBufferMessage { get; }

        public IPeerProxy Peer { get; }

        public CoreMessageType CoreMessageType { get; }
    }
}
