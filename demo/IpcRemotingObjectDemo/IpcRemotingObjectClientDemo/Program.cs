using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using dotnetCampus.Ipc.Pipes;

using IpcRemotingObjectServerDemo;

var ipcProvider = new IpcProvider("IpcRemotingObjectClientDemo");

ipcProvider.StartServer();

var foo = ipcProvider.CreateIpcProxy<IFoo>("IpcRemotingObjectServerDemo", null, null);

var peer = await ipcProvider.GetAndConnectToPeerAsync("IpcRemotingObjectServerDemo");

Console.WriteLine(await foo.AddAsync("a", 1));
Console.WriteLine(foo.Add(1, 2));

Console.Read();
