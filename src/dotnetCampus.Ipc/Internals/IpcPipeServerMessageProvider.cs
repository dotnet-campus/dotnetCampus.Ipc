using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Utils.Extensions;

using Microsoft.Win32.SafeHandles;

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
            var namedPipeServerStream = CreateNamedPipeServerStream();

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
            catch (IOException)
            {
                // "管道已结束。"
                // 当前服务关闭，此时异常符合预期
                return;
            }
            catch (ObjectDisposedException)
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
            serverStreamMessageConverter.PeerConnectBroke += (sender, args) =>
                PeerConnectBroke?.Invoke(sender, new IpcPipeServerMessageProviderPeerConnectionBrokenArgs(this, args));

            serverStreamMessageConverter.Run();
        }

        /// <summary>
        /// 创建 NamedPipeServerStream 对象
        /// </summary>
        /// <returns></returns>
        /// 在 Windows 下，支持管理员权限和非管理员权限的进程相互通讯
        private NamedPipeServerStream CreateNamedPipeServerStream()
        {
            NamedPipeServerStream namedPipeServerStream;

#if NETCOREAPP
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // 用来在 Windows 下，混用管理员权限和非管理员权限的管道
                SecurityIdentifier securityIdentifier = new SecurityIdentifier(
                    WellKnownSidType.AuthenticatedUserSid, null);

                PipeSecurity pipeSecurity = new PipeSecurity();
                pipeSecurity.AddAccessRule(new PipeAccessRule(securityIdentifier,
                    PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance,
                    AccessControlType.Allow));

#if NET6_0_OR_GREATER
                // 这个 NamedPipeServerStreamAcl 是在 .NET 5 引入的
                namedPipeServerStream = NamedPipeServerStreamAcl.Create
                (
                    PipeName,
                    // 本框架使用两个半工做双向通讯，因此这里只是接收，不做发送
                    PipeDirection.In,
                    // 旧框架采用默认为 260 个实例链接，这里减少 10 个，没有具体的理由，待测试
                    250,
                    // 默认都采用 byte 方式
                    PipeTransmissionMode.Byte,
                    // 采用异步的方式。如果没有设置，默认是同步方式，即使有 Async 的方法，底层也是走同步
                    PipeOptions.Asynchronous,
                    inBufferSize: 0, // If it is 0, the buffer size is allocated as needed.
                    outBufferSize: 0, // If it is 0, the buffer size is allocated as needed.
                    pipeSecurity
                //, HandleInheritability.None 默认值
                //, PipeAccessRights.ReadWrite 默认值
                );
#else
                // ===== 仅 .NET Core 3.1 使用：通过 P/Invoke 创建带 PipeSecurity 的命名管道 =====
                // .NET Core 3.1 上 NamedPipeServerStream 既不接受 PipeSecurity，
                // 也没有 NamedPipeServerStreamAcl（该 API 仅 .NET 5+ 才在 System.IO.Pipes.AccessControl 中提供）；
                // 而 System.IO.Pipes.AccessControl 包虽然能装上并暴露 PipesAclExtensions.SetAccessControl，
                // 但 .NET Core 3.1 上 NamedPipeServerStream 创建出的句柄不带 WRITE_DAC 权限，
                // 事后再 SetAccessControl 会在 SetSecurityInfo 处抛 UnauthorizedAccessException。
                // 因此通过 P/Invoke 调用 CreateNamedPipe，在创建时直接传入安全描述符。
                namedPipeServerStream = CreateNamedPipeServerStreamWithSecurity(PipeName,
                    // 本框架使用两个半工做双向通讯，因此这里只是接收，不做发送
                    PipeDirection.In,
                    // 旧框架采用默认为 260 个实例链接，这里减少 10 个，没有具体的理由，待测试
                    250,
                    // 默认都采用 byte 方式

                    PipeTransmissionMode.Byte,
                    // 采用异步的方式。如果没有设置，默认是同步方式，即使有 Async 的方法，底层也是走同步
                    PipeOptions.Asynchronous,
                   inBufferSize: 0, // If it is 0, the buffer size is allocated as needed.
                    outBufferSize: 0, // If it is 0, the buffer size is allocated as needed.
                    pipeSecurity);
#endif
                return namedPipeServerStream;
            }

#elif NETFRAMEWORK
            // .Net Framework 就不再判断 Windows 了
            SecurityIdentifier securityIdentifier = new SecurityIdentifier(
                WellKnownSidType.AuthenticatedUserSid, null);

            PipeSecurity pipeSecurity = new PipeSecurity();
            pipeSecurity.AddAccessRule(new PipeAccessRule(securityIdentifier,
                PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance,
                AccessControlType.Allow));

            namedPipeServerStream = new NamedPipeServerStream
            (
                PipeName,
                // 本框架使用两个半工做双向通讯，因此这里只是接收，不做发送
                PipeDirection.In,
                // 旧框架采用默认为 260 个实例链接，这里减少 10 个，没有具体的理由，待测试
                250,
                // 默认都采用 byte 方式
                PipeTransmissionMode.Byte,
                // 采用异步的方式。如果没有设置，默认是同步方式，即使有 Async 的方法，底层也是走同步
                PipeOptions.Asynchronous,
                0,
                0,
                pipeSecurity
            );
            return namedPipeServerStream;
#endif
            // 如果非 Windows 平台，或者非 .NET 6 / .Net Framework 应用，那就不加上权限
            namedPipeServerStream = new NamedPipeServerStream
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

            return namedPipeServerStream;
        }

        /*
        private void ServerStreamMessageConverter_AckRequested(object? sender, Ack e)
        {
            SendAck(e);
        }
        */
        private ServerStreamMessageReader? ServerStreamMessageReader { set; get; }
        public event EventHandler<IpcPipeServerMessageProviderPeerConnectionBrokenArgs>? PeerConnectBroke;

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

#if NETCOREAPP && !NET5_0_OR_GREATER
        // ===== 仅 .NET Core 3.1 使用：通过 P/Invoke 创建带 PipeSecurity 的命名管道 =====
        private const uint PIPE_ACCESS_DUPLEX = 0x00000003;
        private const uint PIPE_ACCESS_INBOUND = 0x00000001;
        private const uint PIPE_ACCESS_OUTBOUND = 0x00000002;
        private const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        private const uint FILE_FLAG_WRITE_THROUGH = 0x80000000;
        private const uint PIPE_TYPE_BYTE = 0x00000000;
        private const uint PIPE_TYPE_MESSAGE = 0x00000004;

        [StructLayout(LayoutKind.Sequential)]
        struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateNamedPipeW")]
        private static extern SafePipeHandle CreateNamedPipe(
            string lpName,
            uint dwOpenMode,
            uint dwPipeMode,
            uint nMaxInstances,
            uint nOutBufferSize,
            uint nInBufferSize,
            uint nDefaultTimeOut,
            ref SECURITY_ATTRIBUTES lpSecurityAttributes);

        /// <summary>
        /// 通过 Win32 CreateNamedPipe 创建带访问控制的命名管道，并包装为 <see cref="NamedPipeServerStream"/>。
        /// 这是 .NET Core 3.1 上唯一能在创建时附带 <see cref="PipeSecurity"/> 的方式。
        /// </summary>
        private static NamedPipeServerStream CreateNamedPipeServerStreamWithSecurity(
            string pipeName, PipeDirection direction, int maxInstances,
            PipeTransmissionMode transmissionMode, PipeOptions options,
            int inBufferSize, int outBufferSize, PipeSecurity pipeSecurity)
        {
            uint openMode;
            switch (direction)
            {
                case PipeDirection.In: openMode = PIPE_ACCESS_INBOUND; break;
                case PipeDirection.Out: openMode = PIPE_ACCESS_OUTBOUND; break;
                default: openMode = PIPE_ACCESS_DUPLEX; break;
            }

            if ((options & PipeOptions.Asynchronous) != 0) openMode |= FILE_FLAG_OVERLAPPED;
            if ((options & PipeOptions.WriteThrough) != 0) openMode |= FILE_FLAG_WRITE_THROUGH;

            uint pipeMode = transmissionMode == PipeTransmissionMode.Message
                ? PIPE_TYPE_MESSAGE
                : PIPE_TYPE_BYTE;

            // 将托管 PipeSecurity 转为原生安全描述符
            var sdBytes = pipeSecurity.GetSecurityDescriptorBinaryForm();
            GCHandle pinned = GCHandle.Alloc(sdBytes, GCHandleType.Pinned);
            try
            {
                var sa = new SECURITY_ATTRIBUTES
                {
                    nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES)),
                    lpSecurityDescriptor = pinned.AddrOfPinnedObject(),
                    bInheritHandle = 0,
                };

                var fullName = @"\\.\pipe\" + pipeName;
                var handle = CreateNamedPipe(fullName, openMode, pipeMode,
                    (uint) maxInstances, (uint) outBufferSize, (uint) inBufferSize, 0, ref sa);

                if (handle.IsInvalid)
                {
                    var error = Marshal.GetLastWin32Error();
                    handle.Dispose();
                    // 与原构造函数行为保持一致：管道已存在 / 被占用时抛 IOException，
                    // 权限不足时抛 UnauthorizedAccessException，外层 catch 会处理。
                    const int ERROR_ACCESS_DENIED = 5;
                    if (error == ERROR_ACCESS_DENIED)
                    {
                        throw new UnauthorizedAccessException();
                    }

                    throw new IOException("CreateNamedPipe 失败，Win32 错误码：" + error);
                }

                var isAsync = (options & PipeOptions.Asynchronous) != 0;
                return new NamedPipeServerStream(direction, isAsync, false, handle);
            }
            finally
            {
                pinned.Free();
            }
        }
#endif
    }
}
