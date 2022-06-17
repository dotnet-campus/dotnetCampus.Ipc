// See https://aka.ms/new-console-template for more information

using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using dotnetCampus.Ipc.Pipes;

using IpcRemotingObjectServerDemo;

var ipcProvider = new IpcProvider("IpcRemotingObjectClientDemo");

ipcProvider.StartServer();

var peer = await ipcProvider.GetAndConnectToPeerAsync("IpcRemotingObjectServerDemo");

var foo = ipcProvider.CreateIpcProxy<IFoo>(peer);

Console.WriteLine(await foo.AddAsync("a", 1));
Console.WriteLine(foo.Add(1, 2));

Console.Read();
