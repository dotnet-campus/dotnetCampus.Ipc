using System.Threading.Tasks;
using dotnetCampus.Ipc.Tests.CompilerServices.FakeRemote;

namespace dotnetCampus.Ipc.FakeTests.FakeApis
{
    public class RemoteFakeIpcObject : IRemoteFakeIpcObject
    {
        private readonly TaskCompletionSource _source = new TaskCompletionSource();
        private readonly IRemoteFakeIpcArgumentOrReturn _privateReturn = new RemoteIpcReturn("private");

        public async Task<IRemoteFakeIpcArgumentOrReturn> MethodWithIpcParameterAsync(IRemoteFakeIpcArgumentOrReturn ipcArgument, string changeValue)
        {
            // 修改来自参数所在进程的 IPC 对象的值。
            await Task.Run(() =>
            {
                ipcArgument.Value = changeValue;
            });

            // 返回自己进程的值给对方进程。
            return _privateReturn;
        }

        Task IRemoteFakeIpcObject.ShutdownAsync()
        {
            _source.SetResult();
            return Task.CompletedTask;
        }

        internal Task WaitForShutdownAsync()
        {
            return _source.Task;
        }
    }
}
