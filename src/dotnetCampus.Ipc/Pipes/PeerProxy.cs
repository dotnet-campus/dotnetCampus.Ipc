using System;
using System.Diagnostics;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Diagnostics;
using dotnetCampus.Ipc.Exceptions;
using dotnetCampus.Ipc.Internals;
using dotnetCampus.Ipc.Messages;
using Newtonsoft.Json.Schema;

namespace dotnetCampus.Ipc.Pipes
{
    /// <summary>
    /// 用于表示远程的对方
    /// </summary>
    public class PeerProxy : IPeerProxy
    // 为什么 PeerProxy 不加上 IDisposable 方法
    // 因为这个类在上层业务使用，如果被上层业务调释放了，框架层就没得玩
    //, IDisposable
    {
        internal PeerProxy(string peerName, IpcClientService ipcClientService, IpcContext ipcContext)
        {
            PeerName = peerName;
            IpcClientService = ipcClientService;

            IpcContext = ipcContext;

            IpcMessageRequestManager = new IpcMessageRequestManager();
            IpcMessageRequestManager.OnIpcClientRequestReceived += ResponseManager_OnIpcClientRequestReceived;
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

        internal TaskCompletionSource<bool> WaitForFinishedTaskCompletionSource { private set; get; } =
            new TaskCompletionSource<bool>();

        internal IpcContext IpcContext { get; }

        /// <summary>
        /// 当收到消息时触发
        /// </summary>
        public event EventHandler<IPeerMessageArgs>? MessageReceived;

        internal IpcMessageRequestManager IpcMessageRequestManager { get; }

        /// <inheritdoc />
        public async Task NotifyAsync(IpcMessage request)
        {
            // 追踪业务消息。
            var requestTracker = new IpcMessageTracker<IpcMessageBody>(IpcContext.PipeName, PeerName, request.Body, request.Tag, IpcContext.Logger);
            requestTracker.CriticalStep("Send", null, request.Body);

            await WaitConnectAsync(requestTracker);

            // 发送带有追踪的请求。
            await IpcClientService.WriteMessageAsync(requestTracker).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IpcMessage> GetResponseAsync(IpcMessage request)
        {
            // 追踪业务消息。
            var requestTracker = new IpcMessageTracker<IpcMessage>(IpcContext.PipeName, PeerName, request, request.Tag, IpcContext.Logger);
            requestTracker.CriticalStep("Send", null, request.Body);
            await WaitConnectAsync(requestTracker);

            // 将业务消息封装成请求消息，并追踪。
            var ipcClientRequestMessage = IpcMessageRequestManager.CreateRequestMessage(request);
            var ipcBufferMessageContextTracker = requestTracker.TrackNext(ipcClientRequestMessage.IpcBufferMessageContext);

            // 发送带有追踪的请求。
            await IpcClientService.WriteMessageAsync(ipcBufferMessageContextTracker).ConfigureAwait(false);

            // 等待响应，并追踪。
            var messageBody = await ipcClientRequestMessage.Task.ConfigureAwait(false);
            return new IpcMessage($"[{PeerName}]", messageBody);
        }

        /// <inheritdoc />
        public event EventHandler<IPeerConnectionBrokenArgs>? PeerConnectionBroken;

        /// <inheritdoc />
        public event EventHandler<IPeerReconnectedArgs>? PeerReconnected;

        #region 框架

        /// <summary>
        /// 用于写入裸数据
        /// </summary>
        /// 框架内使用
        internal IpcMessageWriter IpcMessageWriter { private set; get; }
        // 由构造函数初始化 IpcClientService 时，自动初始化此属性，因此不为空
            = null!;

        /// <summary>
        /// 表示作为客户端和对方连接
        /// </summary>
        /// 框架内使用
        internal IpcClientService IpcClientService
        {
            private set
            {
                IpcMessageWriter = new IpcMessageWriter(value);
                _ipcClientService = value;
            }
            get
            {
                return _ipcClientService;
            }
        }

        private IpcClientService _ipcClientService = null!;

        /// <summary>
        /// 是否已断开
        /// </summary>
        /// 特别和 <see cref="IsConnectedFinished"/> 分开两个属性，一个对外，一个给框架内使用
        /// 核心是用来实现重连的逻辑
        internal bool IsBroken { get; private set; }

        /// <summary>
        /// 获取是否连接完成，也就是可以读取，可以发送
        /// </summary>
        /// 连接过程中，断开，此时依然是 true 的值。在此时进行写入的内容，都会加入到缓存里面
        public bool IsConnectedFinished { get; private set; }

        /// <summary>
        /// 用于记录数据，记录是否附加上重新连接
        /// </summary>
        internal PeerReConnector? PeerReConnector { set; get; }

        private bool AutoReconnectPeers => _ipcClientService.IpcContext.IpcConfiguration.AutoReconnectPeers;

        /// <summary>
        /// 被对方连回来时调用，此方法被调用意味着准备已完成
        /// </summary>
        /// <param name="ipcInternalPeerConnectedArgs"></param>
        internal void Update(IpcInternalPeerConnectedArgs ipcInternalPeerConnectedArgs)
        {
            Debug.Assert(ipcInternalPeerConnectedArgs.PeerName == PeerName);

            var serverStreamMessageReader = ipcInternalPeerConnectedArgs.ServerStreamMessageReader;

            serverStreamMessageReader.MessageReceived -= ServerStreamMessageReader_MessageReceived;
            serverStreamMessageReader.MessageReceived += ServerStreamMessageReader_MessageReceived;

            // 连接断开
            serverStreamMessageReader.PeerConnectBroke -= ServerStreamMessageReader_PeerConnectBroke;
            serverStreamMessageReader.PeerConnectBroke += ServerStreamMessageReader_PeerConnectBroke;

            IsConnectedFinished = true;
            IsBroken = false;

            if (WaitForFinishedTaskCompletionSource.TrySetResult(true))
            {
            }
            else
            {
                // Debug.Assert(false, "重复调用");
            }
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        internal async void Reconnect(IpcClientService ipcClientService)
        {
            Debug.Assert(ipcClientService.PeerName == PeerName);

            IpcClientService = ipcClientService;

            // 等待完成更新之后，再进行通知，否则将会在收到事件时，还在准备完成所有逻辑
            await WaitForFinishedTaskCompletionSource.Task;

            PeerReconnected?.Invoke(this, new PeerReconnectedArgs());
        }

        private void ServerStreamMessageReader_PeerConnectBroke(object? sender, PeerConnectionBrokenArgs e)
        {
            OnPeerConnectionBroken(e);
        }

        private void ServerStreamMessageReader_MessageReceived(object? sender, PeerStreamMessageArgs e)
        {
            IpcMessageRequestManager.OnReceiveMessage(e);
            var args = e.ToPeerMessageArgs();

            var messageTracker = new IpcMessageTracker<IpcMessage>(
                IpcContext.PipeName,
                PeerName,
                args.Message,
                "MessageReceived",
                IpcContext.Logger);
            messageTracker.CriticalStep("Receive", e.Ack, messageTracker.Message.Body);
            IpcContext.TaskPool.Run(() => MessageReceived?.Invoke(this, args), IpcContext.Logger);
        }

        private void ResponseManager_OnIpcClientRequestReceived(object? sender, IpcClientRequestArgs e)
        {
            var ipcRequestHandlerProvider = IpcContext.IpcRequestHandlerProvider;
            ipcRequestHandlerProvider.HandleRequest(this, e);
        }

        private void OnPeerConnectionBroken(IPeerConnectionBrokenArgs e)
        {
            IsBroken = true;

            if (AutoReconnectPeers)
            {
                WaitForFinishedTaskCompletionSource = new TaskCompletionSource<bool>();
            }

            IpcClientService.Dispose();

            PeerConnectionBroken?.Invoke(this, e);

            IpcMessageRequestManager.BreakAllRequestTaskByIpcBroken();
        }

        private async Task WaitConnectAsync(IIpcMessageTracker requestTracker)
        {
            if (IsBroken)
            {
                if (IpcContext.IsDisposing || IpcContext.IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(PeerProxy),
                        $"当前服务已被释放，服务名: LocalPeerName={IpcContext.PipeName}; MessageTag={requestTracker.Tag}");
                }

                if (AutoReconnectPeers)
                {
#if DEBUG
                    requestTracker.Debug("[Reconnect] Waiting FinishedTaskCompletion");
#endif

                    // 如果完全，且断开，且需要自动连接
                    // 这…… 下面实现了简单的自旋，理论上是无伤的
                    while (WaitForFinishedTaskCompletionSource.Task.IsCompleted && IsBroken)
                    {
                        // 如果只执行到设置 IsBroken=true 还没有创建新 WaitForFinishedTaskCompletionSource 对象
                        // 那么此时 IsCompleted=true 且 IsBroken=true 满足
                        // 进入等待

                        // 如果特别快，进入 IsBroken=true 之后立刻创建 WaitForFinishedTaskCompletionSource 对象
                        // 自然此时的 IsCompleted=false 进入下面的等待

                        // 如果特别特别快，断开后立刻连接
                        // 那么 IsCompleted=true 满足，但是 IsBroken=false 了
                        // 进入下面的等待
                        await Task.Yield();
                    }
                    await WaitForFinishedTaskCompletionSource.Task;
#if DEBUG
                    requestTracker.Debug("[Reconnect] Finish Waiting FinishedTaskCompletion");
#endif
                }
                else
                {
                    throw new IpcPeerConnectionBrokenException();
                }
            }
        }

        #endregion

        /// <summary>
        /// 释放当前的对象，必须由框架内调用
        /// </summary>
        internal void DisposePeer()
        {
            if (!IpcContext.IsDisposing)
            {
                throw new InvalidOperationException($"仅允许在 IpcProvider 的 Dispose 进行调用");
            }

            IsBroken = true;
            // 不管此状态
            //IsConnectedFinished = true;
            _ipcClientService.Dispose();
        }
    }
}
