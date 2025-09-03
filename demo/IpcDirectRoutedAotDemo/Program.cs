// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.IpcRouteds.DirectRouteds;
using dotnetCampus.Ipc.Pipes;
using IpcDirectRoutedAotDemo;

var notifyPath = "NotifyFoo";
var notifyPath2 = "NotifyFoo2";
var requestPath = "RequestFoo";
var requestPath2 = "RequestFoo2";

if (args.Length == 0)
{
    // 首个启动的，当成服务端
    string pipeName = Guid.NewGuid().ToString();
    var ipcProvider = new IpcProvider(pipeName, new IpcConfiguration()
    {

    }.UseSystemTextJsonIpcObjectSerializer(SourceGenerationContext.Default));
    var ipcDirectRoutedProvider = new JsonIpcDirectRoutedProvider(ipcProvider);
    ipcDirectRoutedProvider.AddNotifyHandler(notifyPath, () =>
    {
        Console.WriteLine($"[{Environment.ProcessId}] Receive Notify. 服务端收到通知");
    });

    ipcDirectRoutedProvider.AddNotifyHandler(notifyPath2, (NotifyInfo notifyInfo) =>
    {
        Console.WriteLine($"[{Environment.ProcessId}] Receive Notify. 服务端收到通知 NotifyInfo={notifyInfo}");
    });

    ipcDirectRoutedProvider.AddRequestHandler(requestPath, () => "ResponseFoo");

    ipcDirectRoutedProvider.AddRequestHandler(requestPath2, (DemoRequest request) =>
    {
        Console.WriteLine($"[{Environment.ProcessId}] Receive Request. 服务端收到请求 DemoRequest={request}");
        return new DemoResponse() { Result = "返回内容" };
    });

    ipcDirectRoutedProvider.StartServer();
    Console.WriteLine($"[{Environment.ProcessId}] 服务启动");

    // 启动另一个进程
    Process.Start(Environment.ProcessPath!, pipeName);
}
else
{
    var peerName = args[0];
    Console.WriteLine($"[{Environment.ProcessId}] 客户端进程启动");
    var jsonIpcDirectRoutedProvider = new JsonIpcDirectRoutedProvider(ipcConfiguration: new IpcConfiguration()
        .UseSystemTextJsonIpcObjectSerializer(SourceGenerationContext.Default));
    JsonIpcDirectRoutedClientProxy jsonIpcDirectRoutedClientProxy = await jsonIpcDirectRoutedProvider.GetAndConnectClientAsync(peerName);

    Console.WriteLine($"[{Environment.ProcessId}] 客户端发送通知");
    await jsonIpcDirectRoutedClientProxy.NotifyAsync(notifyPath);
    Console.WriteLine($"[{Environment.ProcessId}] 客户端发送通知完成");

    Console.WriteLine($"[{Environment.ProcessId}] 客户端发送通知2");
    await jsonIpcDirectRoutedClientProxy.NotifyAsync(notifyPath2, new NotifyInfo()
    {
        Value = "通知2"
    });
    Console.WriteLine($"[{Environment.ProcessId}] 客户端发送通知2完成");


    Console.WriteLine($"[{Environment.ProcessId}] 客户端发送请求");
    var responseValue = await jsonIpcDirectRoutedClientProxy.GetResponseAsync<string>(requestPath);
    Console.WriteLine($"[{Environment.ProcessId}] 客户端完成请求，收到响应内容 {responseValue}");

    Console.WriteLine($"[{Environment.ProcessId}] 客户端发送请求2");
    var responseValue2 = await jsonIpcDirectRoutedClientProxy.GetResponseAsync<DemoResponse>(requestPath2, new DemoRequest()
    {
        Value = "客户端请求内容"
    });
    Console.WriteLine($"[{Environment.ProcessId}] 客户端完成请求2，收到响应内容 {responseValue2}");
}

Console.WriteLine($"[{Environment.ProcessId}] 等待退出");
Console.Read();
Console.WriteLine($"[{Environment.ProcessId}] 进程准备退出");
