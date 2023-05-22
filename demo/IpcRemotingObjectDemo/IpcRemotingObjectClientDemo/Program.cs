using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using dotnetCampus.Ipc.Pipes;

using IpcRemotingObjectServerDemo;

var ipcProvider = new IpcProvider("IpcRemotingObjectClientDemo");

ipcProvider.StartServer();

var foo = ipcProvider.CreateIpcProxyByPeerName<IFoo>("IpcRemotingObjectServerDemo", new IpcProxyConfigs());

var peer = await ipcProvider.GetAndConnectToPeerAsync("IpcRemotingObjectServerDemo");

Console.WriteLine(await foo.AddAsync("a", 1));
Console.WriteLine(foo.Add(1, 2));

Console.Read();
