using System.Threading.Tasks;
using dotnetCampus.Ipc.CompilerServices.Attributes;

namespace dotnetCampus.Ipc.FakeTests.FakeApis
{
    [IpcPublic]
    public interface IRemoteFakeIpcObject
    {
        [IpcMethod(IgnoresIpcException = false)]
        Task<IRemoteFakeIpcArgumentOrReturn> MethodWithIpcParameterAsync(IRemoteFakeIpcArgumentOrReturn ipcArgument, string changeValue);

        [IpcMethod(IgnoresIpcException = true, Timeout = 2000)]
        Task ShutdownAsync();
    }
}
