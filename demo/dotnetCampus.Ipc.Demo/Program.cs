using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Threading;
using dotnetCampus.Ipc.Utils.Logging;

namespace dotnetCampus.Ipc.Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // 为了测试 NET 45 和 NET Core 3.1 版本，才这么写的
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            var ipcProvider = new IpcProvider();
            ipcProvider.StartServer();

            ipcProvider.PeerConnected += (sender, connectedArgs) =>
            {
                Console.WriteLine($"[{GetCurrentProcessId()}] 收到 {connectedArgs.Peer.PeerName} 的连接");

                connectedArgs.Peer.MessageReceived += async (o, messageArgs) =>
                {
                    Console.WriteLine($"[{GetCurrentProcessId()}] 收到 {messageArgs.PeerName} 的消息：{GetMessageText(messageArgs.Message.Body)}");

                    await Task.Delay(TimeSpan.FromSeconds(1));

                    // 反向发送消息给对方
                    Console.WriteLine($"[{GetCurrentProcessId()}] 向 {connectedArgs.Peer.PeerName} 回复消息");
                    await connectedArgs.Peer.NotifyAsync(new IpcMessage("回复", Encoding.UTF8.GetBytes($"收到你的消息")));
                    Console.WriteLine($"[{GetCurrentProcessId()}] 完成向 {connectedArgs.Peer.PeerName} 回复消息");
                };
            };

            if (args.Length == 0)
            {
                var currentName = ipcProvider.IpcContext.PipeName;
                // 将当前的进程的 Name 传递给另一个进程，用来达成通讯
                var currentProcess = Process.GetCurrentProcess();
                var mainModule = currentProcess.MainModule;
                if (mainModule == null)
                {
                    throw new InvalidOperationException("无法获取当前进程的主模块路径。");
                }

                var mainModuleFileName = mainModule.FileName;
                Console.WriteLine($"[{GetCurrentProcessId()}] 准备启动 {mainModuleFileName} 参数：{currentName}");

                Process.Start(mainModuleFileName, currentName);
            }
            else
            {
                // 这是被启动的进程，主动连接发送消息
                Console.WriteLine($"[{GetCurrentProcessId()}] 开始连接对方进程");

                var peer = await ipcProvider.GetAndConnectToPeerAsync(args[0]);
                peer.MessageReceived += (sender, messageArgs) =>
                {
                    Console.WriteLine(
                        $"[{GetCurrentProcessId()}] 收到 {peer.PeerName} 的回复消息：{GetMessageText(messageArgs.Message.Body)}");
                };
                await peer.NotifyAsync(new IpcMessage("Hello",
                    Encoding.UTF8.GetBytes($"Hello,进程号是 {GetCurrentProcessId()} 发送过来消息")));
                Console.WriteLine($"[{GetCurrentProcessId()}] 完成发送消息");
            }

            Console.WriteLine($"[{GetCurrentProcessId()}] 等待退出");
            Console.Read();
            Console.WriteLine($"[{GetCurrentProcessId()}] 进程准备退出");
        }

        private static int GetCurrentProcessId()
        {
            return Process.GetCurrentProcess().Id;
        }

        private static string GetMessageText(IpcMessageBody body)
        {
            return Encoding.UTF8.GetString(body.Buffer, body.Start, body.Length);
        }
    }
}
