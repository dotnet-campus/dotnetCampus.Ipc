using System;
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using dotnetCampus.Ipc.Pipes;
using IpcRemotingObjectServerDemo;

var ipcProvider = new IpcProvider("IpcRemotingObjectServerDemo");

ipcProvider.CreateIpcJoint<IFoo>(new Foo());
ipcProvider.PeerConnected += (sender, connectedArgs) =>
{
    Console.WriteLine($"PeerConnected. {connectedArgs.Peer.PeerName}");
};
ipcProvider.StartServer();

Console.Read();
