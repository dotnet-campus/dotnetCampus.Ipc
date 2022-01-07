using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Utils.Extensions;

namespace dotnetCampus.Ipc.Internals
{
    /// <summary>
    /// 提供一个客户端连接
    /// </summary>
    internal class IpcPipeServerMessageProvider : IDisposable
    {
        public IpcPipeServerMessageProvider(IpcContext ipcContext, IpcServerService ipcServerService)
        {
            IpcContext = ipcContext;
            IpcServerService = ipcServerService;
        }

        private NamedPipeServerStream NamedPipeServerStream { set; get; } = null!;

        /// <summary>
        /// 被对方连接
        /// </summary>
        private string PeerName => ServerStreamMessageReader?.PeerName ?? "NoConnected";

        /// <summary>
        /// 自身的名字
        /// </summary>
        public string PipeName => IpcContext.PipeName;

        public IpcContext IpcContext { get; }
        public IpcServerService IpcServerService { get; }


        public async Task Start()
        {
            var namedPipeServerStream = new NamedPipeServerStream
            (
                PipeName,
                // 本框架使用两个半工做双向通讯，因此这里只是接收，不做发送
                PipeDirection.In,
                // 旧框架采用默认为 260 个实例链接，这里减少 10 个，没有具体的理由，待测试
                250,
                // 默认都采用 byte 方式
                PipeTransmissionMode.Byte,
                // 采用异步的方式。如果没有设置，默认是同步方式，即使有 Async 的方法，底层也是走同步
                PipeOptions.Asynchronous
            );
            NamedPipeServerStream = namedPipeServerStream;

            try
            {
#if NETCOREAPP
                await namedPipeServerStream.WaitForConnectionAsync().ConfigureAwait(false);
#else
            await Task.Factory.FromAsync(namedPipeServerStream.BeginWaitForConnection,
                namedPipeServerStream.EndWaitForConnection, null).ConfigureAwait(false);
#endif
            }
            catch (IOException ex)
            {
                // "管道已结束。"
                // 当前服务关闭，此时异常符合预期
                return;
            }
            catch (ObjectDisposedException ex)
            {
                // 当等待客户端连上此服务端期间，被调用了 Dispose 方法后，会抛出此异常。
                // 日志在 Dispose 方法里记。
                return;
            }
            //var streamMessageConverter = new StreamMessageConverter(namedPipeServerStream,
            //    IpcConfiguration.MessageHeader, IpcConfiguration.SharedArrayPool);
            //streamMessageConverter.MessageReceived += OnClientConnectReceived;
            //StreamMessageConverter = streamMessageConverter;
            //streamMessageConverter.Start();

            var serverStreamMessageConverter = new ServerStreamMessageReader(IpcContext, NamedPipeServerStream);
            ServerStreamMessageReader = serverStreamMessageConverter;

            //serverStreamMessageConverter.AckRequested += ServerStreamMessageConverter_AckRequested;
            serverStreamMessageConverter.AckReceived += IpcContext.AckManager.OnAckReceived;
            serverStreamMessageConverter.PeerConnected += IpcServerService.OnPeerConnected;
            serverStreamMessageConverter.MessageReceived += IpcServerService.OnMessageReceived;

            serverStreamMessageConverter.Run();
        }

        /*
        private void ServerStreamMessageConverter_AckRequested(object? sender, Ack e)
        {
            SendAck(e);
        }
        */

        private ServerStreamMessageReader? ServerStreamMessageReader { set; get; }

        /*
        private async void SendAck(Ack receivedAck) => await SendAckAsync(receivedAck);

        private async Task SendAckAsync(Ack receivedAck)
        {
            IpcContext.Logger.Debug($"[{nameof(IpcServerService)}] SendAck {receivedAck} to {PeerName}");
            var ipcProvider = IpcContext.IpcProvider;
            var peerProxy = await ipcProvider.GetOrCreatePeerProxyAsync(PeerName);
            var ipcClient = peerProxy.IpcClientService;
            await ipcClient.SendAckAsync(receivedAck);
        }
        */

        public void Dispose()
        {
            try
            {
                if (ServerStreamMessageReader is null)
                {
                    // 证明此时还没完全连接
                    NamedPipeServerStream.Dispose();
                }
                else
                {
                    // 证明已连接完成，此时不需要释放 NamedPipeServerStream 类
                    // 不在这一层释放 NamedPipeServerStream 类
                    ServerStreamMessageReader.Dispose();
                }
            }
            finally
            {
                // 通过查看 Dispose 的堆栈来检查出异常时到底是谁在 Dispose。
                IpcContext.Logger.Warning(@$"IpcPipeServerMessageProvider.Dispose
{new StackTrace()}");
            }
        }
    }
}
