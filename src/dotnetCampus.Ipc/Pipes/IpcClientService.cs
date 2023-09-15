using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Context.LoggingContext;
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

        private readonly TaskCompletionSource<NamedPipeClientStreamResult> _namedPipeClientStreamTaskCompletionSource = new TaskCompletionSource<NamedPipeClientStreamResult>();

        private Task<NamedPipeClientStreamResult> NamedPipeClientStreamTask => _namedPipeClientStreamTaskCompletionSource.Task;

        readonly struct NamedPipeClientStreamResult
        {
            public NamedPipeClientStreamResult(NamedPipeClientStream? namedPipeClientStream)
            {
                NamedPipeClientStream = namedPipeClientStream!;
                Exception = null;
            }

            public NamedPipeClientStreamResult(Exception exception)
            {
                Exception = exception;
                NamedPipeClientStream = null!;
            }

            public bool Success => NamedPipeClientStream is not null;
            public NamedPipeClientStream NamedPipeClientStream { get; }
            public Exception? Exception { get; }
        }

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
        /// <exception cref="IpcClientPipeConnectionException">连接失败时抛出</exception>
        /// <returns></returns>
        public async Task Start(bool shouldRegisterToPeer = true)
        {
            var result = await StartInternalAsync(isReConnect: false, shouldRegisterToPeer);

            if (!result)
            {
                throw new IpcClientPipeConnectionException(PeerName);
            }
        }

        /// <inheritdoc cref="Start"/>
        /// <param name="isReConnect">是否属于重新连接</param>
        /// <param name="shouldRegisterToPeer">是否需要向对方注册</param>
        /// <returns>True:启动成功</returns>
        internal async Task<bool> StartInternalAsync(bool isReConnect, bool shouldRegisterToPeer)
        {
            var namedPipeClientStream = new NamedPipeClientStream(".", PeerName, PipeDirection.Out,
                PipeOptions.None, TokenImpersonationLevel.Impersonation);

            try
            {
                var result = await ConnectNamedPipeAsync(isReConnect, namedPipeClientStream);
                if (!result)
                {
                    _namedPipeClientStreamTaskCompletionSource.TrySetResult(new NamedPipeClientStreamResult(namedPipeClientStream: null));

                    return false;
                }
            }
            catch (Exception e)
            {
                // 理论上不应该存在任何异常的才对，但是由于开放给上层业务端定制。如果存在任何业务端的异常，那就应该设置给 _namedPipeClientStreamTaskCompletionSource 里。否则有一些逻辑将会进入等待，如 Write 系列，等待的 _namedPipeClientStreamTaskCompletionSource 的 Task 将永远不会被释放
                // 包装到 IpcClientPipeConnectionException 里面，方便其他逻辑捕获异常。毕竟要是上层业务端定制的逻辑抛出奇怪类型的异常，那调用 Write 系列的就不好捕获
                _namedPipeClientStreamTaskCompletionSource.TrySetResult(new NamedPipeClientStreamResult(new IpcClientPipeConnectionException(PeerName, e)));

                // 为什么不能调用 SetException 方法？因为以下被注释的代码如果被调用，如果没有任何逻辑等待 _namedPipeClientStreamTaskCompletionSource 的 Task 将会抛出到 TaskScheduler.UnobservedTaskException 里。虽然没有什么事情发生，但是对于某些客户端来说，会让一些伙伴以为存在大坑
                //_namedPipeClientStreamTaskCompletionSource.TrySetException
                throw;
            }

            _namedPipeClientStreamTaskCompletionSource.TrySetResult(new NamedPipeClientStreamResult(namedPipeClientStream));

            if (shouldRegisterToPeer)
            {
                // 启动之后，向对方注册，此时对方是服务器
                await RegisterToPeer();
            }

            return true;
        }

        /// <summary>
        /// 连接命名管道
        /// </summary>
        /// <param name="isReConnect">是否属于重新连接</param>
        /// <param name="namedPipeClientStream"></param>
        /// <returns>True 连接成功</returns>
        /// 独立方法，方便 dnspy 调试
        private async Task<bool> ConnectNamedPipeAsync(bool isReConnect, NamedPipeClientStream namedPipeClientStream)
        {
            var connector = IpcContext.IpcClientPipeConnector;

            if (connector == null)
            {
                await DefaultConnectNamedPipeAsync(namedPipeClientStream);
                return true;
            }
            else
            {
                return await CustomConnectNamedPipeAsync(connector, isReConnect, namedPipeClientStream);
            }
        }

        /// <summary>
        /// 自定义的连接方式
        /// </summary>
        /// <param name="ipcClientPipeConnector"></param>
        /// <param name="namedPipeClientStream"></param>
        /// <param name="isReConnect">是否属于重新连接</param>
        /// <returns></returns>
        private async Task<bool> CustomConnectNamedPipeAsync(IIpcClientPipeConnector ipcClientPipeConnector, bool isReConnect,
            NamedPipeClientStream namedPipeClientStream)
        {
            Logger.Trace($"Connecting NamedPipe by {nameof(CustomConnectNamedPipeAsync)}. LocalClient:'{IpcContext.PipeName}';RemoteServer:'{PeerName}'");
            var ipcClientPipeConnectContext = new IpcClientPipeConnectionContext(PeerName, namedPipeClientStream, CancellationToken.None, isReConnect);
            var result = await ipcClientPipeConnector.ConnectNamedPipeAsync(ipcClientPipeConnectContext);
            Logger.Trace($"Connected NamedPipe by {nameof(CustomConnectNamedPipeAsync)} Success={result.Success} {result.Reason}. LocalClient:'{IpcContext.PipeName}';RemoteServer:'{PeerName}'");

            return result.Success;
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

                var result = await NamedPipeClientStreamTask.ConfigureAwait(false);
                if (result.Success is false)
                {
                    // 理论上框架内不会进入此分支
                    if (Debugger.IsAttached)
                    {
                        // 框架内不应该进入此分支
                        Debugger.Break();
                    }

                    if (result.Exception is not null)
                    {
                        ExceptionDispatchInfo.Capture(result.Exception).Throw();
                    }

                    return;
                }

                var stream = result.NamedPipeClientStream;

                // 追踪、校验消息。
                var ack = AckManager.GetAck();
                tracker.Debug("IPC start writing...");
                tracker.CriticalStep("SendCore", ack, tracker.Message.IpcBufferMessageList);

                IpcContext.LogSendMessage(tracker.Message, PeerName);

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
        internal async Task WriteMessageAsync(IpcMessageTracker<IpcMessage> tracker)
        {
            VerifyNotDisposed();

            await DoubleBufferTask.AddTaskAsync(WriteMessageAsyncInner);

            async Task WriteMessageAsyncInner()
            {
                if (IsDisposed)
                {
                    return;
                }

                var result = await NamedPipeClientStreamTask.ConfigureAwait(false);
                if (result.Success is false)
                {
                    if (result.Exception is not null)
                    {
                        ExceptionDispatchInfo.Capture(result.Exception).Throw();
                    }

                    return;
                }

                var stream = result.NamedPipeClientStream;

                var ipcMessageBody = tracker.Message.Body;

                // 追踪、校验消息。
                var ack = AckManager.GetAck();
                tracker.Debug("IPC start writing...");
                tracker.CriticalStep("SendCore", ack, ipcMessageBody);

                IpcContext.LogSendMessage(in ipcMessageBody, PeerName);

                // 发送消息。
                await IpcMessageConverter.WriteAsync
                (
                    stream,
                    IpcConfiguration.MessageHeader,
                    AckManager.GetAck(),
                    // 表示这是业务层的消息
                    IpcMessageCommandType.Business,
                    ipcMessageBody.Buffer,
                    ipcMessageBody.Start,
                    ipcMessageBody.Length,
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
                var result = await NamedPipeClientStreamTask.ConfigureAwait(false);
                if (result.Success is false)
                {
                    if (result.Exception is not null)
                    {
                        ExceptionDispatchInfo.Capture(result.Exception).Throw();
                    }

                    return;
                }

                IpcContext.LogSendMessage(buffer, offset, count, PeerName);

                var stream = result.NamedPipeClientStream;
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
                var result = NamedPipeClientStreamTask.Result;
                if (result.Success)
                {
                    result.NamedPipeClientStream.Dispose();
                }
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
