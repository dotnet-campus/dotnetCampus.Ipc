using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using dotnetCampus.Ipc.Pipes;

using IpcRemotingObjectServerDemo;

var ipcProvider = new IpcProvider("IpcRemotingObjectClientDemo");

ipcProvider.StartServer();

Console.WriteLine($"等待连接 IpcRemotingObjectServerDemo 中，请确保 IpcRemotingObjectServerDemo 已启动");

var peer = await ipcProvider.GetAndConnectToPeerAsync("IpcRemotingObjectServerDemo");

Console.WriteLine($"已与 IpcRemotingObjectServerDemo 建立连接");

var foo = ipcProvider.CreateIpcProxy<IFoo>(peer);

Console.WriteLine(await foo.AddAsync("a", 1));
Console.WriteLine(foo.Add(1, 2));

Console.Read();
