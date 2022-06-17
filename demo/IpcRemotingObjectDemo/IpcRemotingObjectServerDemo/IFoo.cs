using dotnetCampus.Ipc.CompilerServices.Attributes;

namespace IpcRemotingObjectServerDemo;

[IpcPublic(IgnoresIpcException = true, Timeout = 1000)]
interface IFoo
{
    int Add(int a, int b);
}
