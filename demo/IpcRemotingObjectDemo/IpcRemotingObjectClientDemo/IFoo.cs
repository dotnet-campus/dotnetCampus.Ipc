using dotnetCampus.Ipc.CompilerServices.Attributes;

namespace IpcRemotingObjectServerDemo; // Must the same namespace

[IpcPublic]
interface IFoo
{
    int Add(int a, int b);
    Task<string> AddAsync(string a, int b);
}
