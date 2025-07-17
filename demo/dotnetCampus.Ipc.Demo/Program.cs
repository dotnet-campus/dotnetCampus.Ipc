using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Pipes.PipeConnectors;
using dotnetCampus.Ipc.Threading;
using dotnetCampus.Ipc.Utils.Logging;

namespace dotnetCampus.Ipc.Demo
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var ipcProvider = new IpcProvider("asdasd",new IpcConfiguration()
            {
                IpcClientPipeConnector = new IpcClientPipeConnector(context => false, stepTimeout:TimeSpan.FromSeconds(3))
            });
            ipcProvider.StartServer();

            // 测试在 Linux 连接不存在的对方，将会在 dotnet 框架内部不断重试制造异常
            // System.Net.Sockets.SocketException (99): Cannot assign requested address /tmp/CoreFxPipe_DoNotExistFoo
            // at System.Net.Sockets.Socket.DoConnect(EndPoint endPointSnapshot, SocketAddress socketAddress)
            try
            {
                var peerProxy = await ipcProvider.GetOrCreatePeerProxyAsync("DoNotExistFoo");

                Console.WriteLine($"Is PeerProxy null? {peerProxy == null}");

                if (peerProxy is null)
                {
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }

            if (args.Length == 0)
            {
                var currentName = ipcProvider.IpcContext.PipeName;
                // 将当前的进程的 Name 传递给另一个进程，用来达成通讯
                var mainModuleFileName = Process.GetCurrentProcess().MainModule.FileName;
                Console.WriteLine($"[{Environment.ProcessId}] 准备启动 {mainModuleFileName} 参数：{currentName}");
                Process.Start(mainModuleFileName, currentName);
            }
            else
            {
                // 这是被启动的进程，主动连接发送消息
                Console.WriteLine($"[{Environment.ProcessId}] 开始连接对方进程");

                var peer = await ipcProvider.GetAndConnectToPeerAsync(args[0]);
                peer.MessageReceived += (sender, messageArgs) =>
                {
                    Console.WriteLine(
                        $"[{Environment.ProcessId}] 收到 {peer.PeerName} 的回复消息：{Encoding.UTF8.GetString(messageArgs.Message.Body.AsSpan())}");
                };
                await peer.NotifyAsync(new IpcMessage("Hello",
                    Encoding.UTF8.GetBytes($"Hello,进程号是 {Environment.ProcessId} 发送过来消息")));
                Console.WriteLine($"[{Environment.ProcessId}] 完成发送消息");
            }

            ipcProvider.PeerConnected += (sender, connectedArgs) =>
            {
                Console.WriteLine($"[{Environment.ProcessId}] 收到 {connectedArgs.Peer.PeerName} 的连接");

                connectedArgs.Peer.MessageReceived += async (o, messageArgs) =>
                {
                    Console.WriteLine($"[{Environment.ProcessId}] 收到 {messageArgs.PeerName} 的消息：{Encoding.UTF8.GetString(messageArgs.Message.Body.AsSpan())}");

                    await Task.Delay(TimeSpan.FromSeconds(1));

                    // 反向发送消息给对方
                    Console.WriteLine($"[{Environment.ProcessId}] 向 {connectedArgs.Peer.PeerName} 回复消息");
                    await connectedArgs.Peer.NotifyAsync(new IpcMessage("回复", Encoding.UTF8.GetBytes($"收到你的消息")));
                    Console.WriteLine($"[{Environment.ProcessId}] 完成向 {connectedArgs.Peer.PeerName} 回复消息");
                };
            };

            Console.WriteLine($"[{Environment.ProcessId}] 等待退出");
            Console.Read();
            Console.WriteLine($"[{Environment.ProcessId}] 进程准备退出");
        }
    }
}
