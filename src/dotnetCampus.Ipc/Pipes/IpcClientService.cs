using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Diagnostics;
using dotnetCampus.Ipc.Exceptions;
using dotnetCampus.Ipc.Internals;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Pipes.PipeConnectors;
using dotnetCampus.Ipc.Utils.Extensions;
using dotnetCampus.Ipc.Utils.Logging;
using dotnetCampus.Threading;

namespace dotnetCampus.Ipc.Pipes
{
    /// <summary>
    /// 管道的客户端，用于发送消息
    /// </summary>
    /// 采用两个半工的管道做到双向通讯，这里的管道客户端用于发送
    public class IpcClientService : IRawMessageWriter, IDisposable, IClientMessageWriter
    {
        /// <summary>
        /// 连接其他端，用来发送
        /// </summary>
        /// <param name="ipcContext"></param>
        /// <param name="peerName">对方</param>
        internal IpcClientService(IpcContext ipcContext, string peerName = IpcContext.DefaultPipeName)
        {
            IpcContext = ipcContext;
            PeerName = peerName;

            DoubleBufferTask = new DoubleBufferTask<Func<Task>>(DoTask);
        }

        private async Task DoTask(List<Func<Task>> list)
        {
            foreach (var func in list)
            {
                try
                {
                    await func().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    IpcContext.Logger.Error($"[{nameof(IpcClientService)}.{nameof(DoTask)}] {e}");
                }
            }
        }

        private TaskCompletionSource<NamedPipeClientStream>? _namedPipeClientStreamTaskCompletionSource;

        private Task<NamedPipeClientStream> NamedPipeClientStreamTask => _namedPipeClientStreamTaskCompletionSource is null
            ? Task.FromResult<NamedPipeClientStream>(null!)
            : _namedPipeClientStreamTaskCompletionSource.Task;

        internal AckManager AckManager => IpcContext.AckManager;

        private IpcConfiguration IpcConfiguration => IpcContext.IpcConfiguration;

        /// <summary>
        /// 上下文
        /// </summary>
        public IpcContext IpcContext { get; }

        private PeerRegisterProvider PeerRegisterProvider => IpcContext.PeerRegisterProvider;

        /// <summary>
        /// 对方
        /// </summary>
        public string PeerName { get; }

        private ILogger Logger => IpcContext.Logger;

        /// <summary>
        /// 启动客户端，启动的时候将会去主动连接服务端，然后向服务端注册自身
        /// </summary>
        /// <param name="shouldRegisterToPeer">是否需要向对方注册</param>
        /// <returns></returns>
        public async Task Start(bool shouldRegisterToPeer = true)
        {
            var namedPipeClientStream = new NamedPipeClientStream(".", PeerName, PipeDirection.Out,
                PipeOptions.None, TokenImpersonationLevel.Impersonation);
            _namedPipeClientStreamTaskCompletionSource = new TaskCompletionSource<NamedPipeClientStream>();

            await ConnectNamedPipeAsync(namedPipeClientStream);

            if (!_namedPipeClientStreamTaskCompletionSource.Task.IsCompleted)
            {
                _namedPipeClientStreamTaskCompletionSource.SetResult(namedPipeClientStream);
            }

            if (shouldRegisterToPeer)
            {
                // 启动之后，向对方注册，此时对方是服务器
                await RegisterToPeer();
            }
        }

        /// <summary>
        /// 连接命名管道
        /// </summary>
        /// <param name="namedPipeClientStream"></param>
        /// <returns></returns>
        /// 独立方法，方便 dnspy 调试
        private async Task ConnectNamedPipeAsync(NamedPipeClientStream namedPipeClientStream)
        {
            var connector = IpcContext.IpcClientPipeConnector;

            if (connector == null)
            {
                await DefaultConnectNamedPipeAsync(namedPipeClientStream);
            }
            else
            {
                await CustomConnectNamedPipeAsync(connector, namedPipeClientStream);
            }
        }

        /// <summary>
        /// 自定义的连接方式
        /// </summary>
        /// <param name="ipcClientPipeConnector"></param>
        /// <param name="namedPipeClientStream"></param>
        /// <returns></returns>
        private async Task CustomConnectNamedPipeAsync(IIpcClientPipeConnector ipcClientPipeConnector,
            NamedPipeClientStream namedPipeClientStream)
        {
            Logger.Trace($"Connecting NamedPipe by {nameof(CustomConnectNamedPipeAsync)}. LocalClient:'{IpcContext.PipeName}';RemoteServer:'{PeerName}'");
            var ipcClientPipeConnectContext = new IpcClientPipeConnectionContext(PeerName, namedPipeClientStream, CancellationToken.None);
            await ipcClientPipeConnector.ConnectNamedPipeAsync(ipcClientPipeConnectContext);
        }

        /// <summary>
        /// 默认的连接方式
        /// </summary>
        /// <param name="namedPipeClientStream"></param>
        /// <returns></returns>
        private async Task DefaultConnectNamedPipeAsync(NamedPipeClientStream namedPipeClientStream)
        {
            var localClient = IpcContext.PipeName;
            var remoteServer = PeerName;

            Logger.Trace($"Connecting NamedPipe by {nameof(DefaultConnectNamedPipeAsync)}. LocalClient:'{localClient}';RemoteServer:'{remoteServer}'");
            // 由于 dotnet 6 和以下版本的 ConnectAsync 的实现，只是通过 Task.Run 方法而已，因此统一采用相同的方法即可

            await Task.Run(ConnectNamedPipe);

            void ConnectNamedPipe()
            {
                namedPipeClientStream.Connect();

                // 强行捕获变量，方便调试是在等待哪个连接
                Logger.Trace($"Connected NamedPipe by {nameof(DefaultConnectNamedPipeAsync)}. LocalClient:'{localClient}';RemoteServer:'{remoteServer}'");
            }
        }

        private async Task RegisterToPeer()
        {
            Logger.Trace($"[{nameof(IpcClientService)}] StartRegisterToPeer PipeName={IpcContext.PipeName}");

            // 注册自己
            var peerRegisterMessage = PeerRegisterProvider.BuildPeerRegisterMessage(IpcContext.PipeName);
            var peerRegisterMessageTracker = new IpcMessageTracker<IpcBufferMessageContext>(
                IpcContext.PipeName, PeerName, peerRegisterMessage,
                $"PeerRegisterMessage PipeName={IpcContext.PipeName}", IpcContext.Logger);
            await WriteMessageAsync(peerRegisterMessageTracker);
        }

        /// <summary>
        /// 停止客户端
        /// </summary>
        public void Stop()
        {
            // 告诉服务器端不连接
        }

        /// <summary>
        /// 向服务端发送消息
        /// </summary>
        /// <remarks>
        /// 框架层使用的
        /// </remarks>
        internal async Task WriteMessageAsync(IpcMessageTracker<IpcBufferMessageContext> tracker)
        {
            VerifyNotDisposed();

            try
            {
                await DoubleBufferTask.AddTaskAsync(WriteMessageAsyncInner).ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                // 这里的 InvalidOperationException 对应 DoubleBufferTask.AddTask 里抛出的异常。
                // 在逻辑上确实是使用错误，抛出 InvalidOperationException 是合适的；
                // 但因为 IPC 的断开发生在任何时刻，根本无法提前规避，所以实际上这里指的是 IPC 远端异常。
                throw new IpcRemoteException($"因为已无法连接对方，所以 IPC 消息发送失败。Tag={tracker.Tag}", ex);
                // @lindexi，这里违背了异常处理原则里的“不应捕获使用异常”的原则，所以 DoubleBufferTask 的设计需要修改，加一个 TryAddTaskAsync 以应对并发场景。
            }

            async Task WriteMessageAsyncInner()
            {
                if (IsDisposed)
                {
                    return;
                }

                var stream = await NamedPipeClientStreamTask.ConfigureAwait(false);

                // 追踪、校验消息。
                var ack = AckManager.GetAck();
                tracker.Debug("IPC start writing...");
                tracker.CriticalStep("SendCore", ack, tracker.Message.IpcBufferMessageList);

                // 发送消息。
                await IpcMessageConverter.WriteAsync
                (
                    stream,
                    IpcConfiguration.MessageHeader,
                    ack,
                    tracker.Message,
                    IpcConfiguration.SharedArrayPool
                ).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);

                // 追踪消息。
                tracker.Debug("IPC finish writing.");
            }
        }

        /// <summary>
        /// 向服务端发送消息
        /// </summary>
        /// <param name="tracker"></param>
        /// <returns></returns>
        /// <remarks>
        /// 业务层使用的
        /// </remarks>
        internal async Task WriteMessageAsync(IpcMessageTracker<IpcMessageBody> tracker)
        {
            VerifyNotDisposed();

            await DoubleBufferTask.AddTaskAsync(WriteMessageAsyncInner);

            async Task WriteMessageAsyncInner()
            {
                if (IsDisposed)
                {
                    return;
                }

                var stream = await NamedPipeClientStreamTask.ConfigureAwait(false);

                // 追踪、校验消息。
                var ack = AckManager.GetAck();
                tracker.Debug("IPC start writing...");
                tracker.CriticalStep("SendCore", ack, tracker.Message);

                // 发送消息。
                await IpcMessageConverter.WriteAsync
                (
                    stream,
                    IpcConfiguration.MessageHeader,
                    AckManager.GetAck(),
                    // 表示这是业务层的消息
                    IpcMessageCommandType.Business,
                    tracker.Message.Buffer,
                    tracker.Message.Start,
                    tracker.Message.Length,
                    IpcConfiguration.SharedArrayPool,
                    tracker.Tag
                );

                await stream.FlushAsync().ConfigureAwait(false);

                // 追踪消息。
                tracker.Debug("IPC finish writing.");
            }
        }

        /// <summary>
        /// 向服务端发送消息
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="tag">这一次写入的是什么内容，用于调试</param>
        /// <returns></returns>
        /// <remarks>
        /// 业务层使用的
        /// </remarks>
        public async Task WriteMessageAsync(byte[] buffer, int offset, int count,
            [CallerMemberName] string tag = null!)
        {
            VerifyNotDisposed();

            await DoubleBufferTask.AddTaskAsync(WriteMessageAsyncInner);

            async Task WriteMessageAsyncInner()
            {
                if (IsDisposed)
                {
                    return;
                }

                var currentTag = tag;
                var stream = await NamedPipeClientStreamTask.ConfigureAwait(false);
                await IpcMessageConverter.WriteAsync
                (
                    stream,
                    IpcConfiguration.MessageHeader,
                    AckManager.GetAck(),
                    // 表示这是业务层的消息
                    IpcMessageCommandType.Business,
                    buffer,
                    offset,
                    count,
                    IpcConfiguration.SharedArrayPool,
                    currentTag
                );
                await stream.FlushAsync().ConfigureAwait(false);
            }
        }

        /*
        private async Task QueueWriteAsync(Func<Ack, Task> task, string summary)
        {
            async Task CreateDoubleBufferTaskFunc(Ack ack)
            {
                await DoubleBufferTask.AddTaskAsync(async () => { await task(ack); });
            }

            await AckManager.DoWillReceivedAck(CreateDoubleBufferTaskFunc, PeerName, TimeSpan.FromSeconds(3), maxRetryCount: 10, summary,
                IpcContext.Logger);
        }
        */


        private DoubleBufferTask<Func<Task>> DoubleBufferTask { get; }

        /*
        /// <summary>
        /// 向服务器端发送收到某条消息，或用于回复某条消息已收到
        /// </summary>
        /// <param name="receivedAck"></param>
        /// <returns></returns>
        /// 不需要回复，因为如果消息能发送过去到对方，就是对方收到消息了
        [Obsolete(DebugContext.DoNotUseAck)]
        public async Task SendAckAsync(Ack receivedAck)
        {
            Logger.Debug($"[{nameof(IpcClientService)}][{nameof(SendAckAsync)}] {receivedAck} Start AddTaskAsync");

            var ackMessage = AckManager.BuildAckMessage(receivedAck);

            // 这里不能调用 WriteMessageAsync 方法，因为这些方法都使用了 QueueWriteAsync 方法，在这里面将会不断尝试发送信息，需要收到对方的 ack 才能完成。而作为回复 ack 消息的逻辑，如果还需要等待对方回复 ack 那么将会存在相互等待。本地回复对方的 ack 消息需要等待对方的 ack 消息，而对方的 ack 消息又需要等待本地的回复
            await DoubleBufferTask.AddTaskAsync(async () =>
            {
                await IpcMessageConverter.WriteAsync
                (
                    NamedPipeClientStream,
                    IpcConfiguration.MessageHeader,
                    ack: IpcContext.AckUsedForReply,
                    // 需要使用框架的命令
                    ipcMessageCommandType: IpcMessageCommandType.SendAck,
                    buffer: ackMessage,
                    offset: 0,
                    count: ackMessage.Length,
                    summary: nameof(SendAckAsync),
                    Logger
                );
                await NamedPipeClientStream.FlushAsync();
            });
        }
        */

        /// <inheritdoc />
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            if (NamedPipeClientStreamTask.IsCompleted)
            {
                NamedPipeClientStreamTask.Result.Dispose();
            }

            DoubleBufferTask.Finish();
        }

        private bool IsDisposed { set; get; }

        private void VerifyNotDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(IpcClientService));
            }
        }

        Task IClientMessageWriter.WriteMessageAsync(in IpcBufferMessageContext ipcBufferMessageContext)
        {
            var peerRegisterMessageTracker = new IpcMessageTracker<IpcBufferMessageContext>(
                IpcContext.PipeName, PeerName, ipcBufferMessageContext,
                $"PeerRegisterMessage PipeName={IpcContext.PipeName}", IpcContext.Logger);
            return WriteMessageAsync(peerRegisterMessageTracker);
        }
    }
}
