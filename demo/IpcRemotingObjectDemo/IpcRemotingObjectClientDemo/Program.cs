// See https://aka.ms/new-console-template for more information

using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using dotnetCampus.Ipc.Pipes;

using IpcRemotingObjectServerDemo;

Task.Run(async () =>
{
    var ipcProvider = new IpcProvider("IpcRemotingObjectClientDemo");

    ipcProvider.StartServer();

    var peer = await ipcProvider.GetAndConnectToPeerAsync("IpcRemotingObjectServerDemo");
    // 由于获取到 Peer 返回时，所在的线程就是 IPC 的读取线程，如果在此线程执行任何等待逻辑，将会导致 IPC 的接收消息端进入等待。如果此等待逻辑是等待一个 IPC 消息的返回，那将由于 IPC 的接收消息端进入等待此等待逻辑，而无法完成，导致了 IPC 不再处理消息
    // 为了调用同步的方法，只能开启一个 Task 来进行调用。否则将会因为同步的方法内部使用了锁，导致 IPC 的接收消息端也在等待锁，而此同步方法的锁需要等待 IPC 的接收消息端接收到对方的返回值才能解锁，可惜 IPC 的接收消息端不再工作，因此方法将不再返回
    await Task.Run(() =>
    {
        var foo = ipcProvider.CreateIpcProxy<IFoo>(peer);
        Console.WriteLine(foo.Add(1, 2));
    });
});

Console.Read();
