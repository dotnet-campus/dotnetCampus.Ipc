using System;
using System.Diagnostics;
using System.Threading.Tasks;
using dotnetCampus.Ipc.Abstractions;
using dotnetCampus.Ipc.PipeCore.Context;
using dotnetCampus.Ipc.PipeCore.IpcPipe;
using dotnetCampus.Ipc.PipeCore.Utils;

namespace dotnetCampus.Ipc.PipeCore
{
    /// <summary>
    /// 用于表示远程的对方
    /// </summary>
    public class PeerProxy : IPeerProxy
    {
        internal PeerProxy(string peerName, IpcClientService ipcClientService, IpcContext ipcContext)
        {
            PeerName = peerName;
            IpcClientService = ipcClientService;
            IpcMessageWriter = new IpcMessageWriter(ipcClientService);

            IpcContext = ipcContext;

            ResponseManager = new ResponseManager();
        }


        internal PeerProxy(string peerName, IpcClientService ipcClientService, IpcInternalPeerConnectedArgs ipcInternalPeerConnectedArgs, IpcContext ipcContext) :
            this(peerName, ipcClientService, ipcContext)
        {
            Update(ipcInternalPeerConnectedArgs);
        }

        /// <summary>
        /// 对方的服务器名
        /// </summary>
        public string PeerName { get; }

        internal TaskCompletionSource<bool> WaitForFinishedTaskCompletionSource { get; } =
            new TaskCompletionSource<bool>();

        private IpcContext IpcContext { get; }

        /// <summary>
        /// 当收到消息时触发
        /// </summary>
        public event EventHandler<IPeerMessageArgs>? MessageReceived;

        private ResponseManager ResponseManager { get; }

        /// <inheritdoc />
        public async Task<IpcBufferMessage> GetResponseAsync(IpcRequestMessage request)
        {
            var ipcClientRequestMessage = ResponseManager.CreateRequestMessage(request);
            await IpcClientService.WriteMessageAsync(ipcClientRequestMessage.IpcBufferMessageContext);
            return await ipcClientRequestMessage.Task;
        }


        /// <summary>
        /// 用于写入数据
        /// </summary>
        public IpcMessageWriter IpcMessageWriter { get; }

        /// <summary>
        /// 表示作为客户端和对方连接
        /// </summary>
        /// 框架内使用
        internal IpcClientService IpcClientService { get; }

        /// <summary>
        /// 获取是否连接完成，也就是可以读取，可以发送
        /// </summary>
        public bool IsConnectedFinished { get; private set; }

        ///// <summary>
        ///// 当断开连接的时候触发
        ///// </summary>
        //public event EventHandler<PeerProxy>? Disconnected;

        internal void Update(IpcInternalPeerConnectedArgs ipcInternalPeerConnectedArgs)
        {
            Debug.Assert(ipcInternalPeerConnectedArgs.PeerName == PeerName);

            var serverStreamMessageReader = ipcInternalPeerConnectedArgs.ServerStreamMessageReader;

            serverStreamMessageReader.MessageReceived -= ServerStreamMessageReader_MessageReceived;
            serverStreamMessageReader.MessageReceived += ServerStreamMessageReader_MessageReceived;

            IsConnectedFinished = true;

            if (WaitForFinishedTaskCompletionSource.TrySetResult(true))
            {
            }
            else
            {
                Debug.Assert(false, "重复调用");
            }
        }

        private void ServerStreamMessageReader_MessageReceived(object? sender, PeerMessageArgs e)
        {
            ResponseManager.OnReceiveMessage(e);

            MessageReceived?.Invoke(sender, e);
        }
    }
}
