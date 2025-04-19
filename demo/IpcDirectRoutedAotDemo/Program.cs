// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.IpcRouteds.DirectRouteds;
using dotnetCampus.Ipc.Pipes;

var notifyPath = "NotifyFoo";
var requestPath = "RequestFoo";

if (args.Length == 0)
{
    // 首个启动的，当成服务端
    string pipeName = Guid.NewGuid().ToString();
    var ipcProvider = new IpcProvider(pipeName, new IpcConfiguration()
    {

    });
    var ipcDirectRoutedProvider = new JsonIpcDirectRoutedProvider(ipcProvider);
    ipcDirectRoutedProvider.AddNotifyHandler(notifyPath, () =>
    {
        Console.WriteLine($"[{Environment.ProcessId}] Receive Notify. 服务端收到通知");
    });

    ipcDirectRoutedProvider.AddRequestHandler(requestPath, () => "ResponseFoo");
    ipcDirectRoutedProvider.StartServer();
    Console.WriteLine($"[{Environment.ProcessId}] 服务启动");

    // 启动另一个进程
    Process.Start(Environment.ProcessPath!, pipeName);
}
else
{
    var peerName = args[0];
    Console.WriteLine($"[{Environment.ProcessId}] 客户端进程启动");
    var jsonIpcDirectRoutedProvider = new JsonIpcDirectRoutedProvider();
    JsonIpcDirectRoutedClientProxy jsonIpcDirectRoutedClientProxy = await jsonIpcDirectRoutedProvider.GetAndConnectClientAsync(peerName);

    Console.WriteLine($"[{Environment.ProcessId}] 客户端发送通知");
    await jsonIpcDirectRoutedClientProxy.NotifyAsync(notifyPath);
    Console.WriteLine($"[{Environment.ProcessId}] 客户端发送通知完成");

    Console.WriteLine($"[{Environment.ProcessId}] 客户端发送请求");
    var responseValue = await jsonIpcDirectRoutedClientProxy.GetResponseAsync<string>(requestPath);
    Console.WriteLine($"[{Environment.ProcessId}] 客户端完成请求，收到响应内容 {responseValue}");
}

Console.WriteLine($"[{Environment.ProcessId}] 等待退出");
Console.Read();
Console.WriteLine($"[{Environment.ProcessId}] 进程准备退出");
