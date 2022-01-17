using System;
using System.Threading;
using System.Threading.Tasks;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Extensions.Contracts;

namespace dotnetCampus.Ipc.Tests
{
    [TestClass]
    public class IpcClientServiceTests
    {
        [ContractTestCase]
        public void SendWhenDisposed()
        {
            "连接断开之后持续发送消息，将会收到 ObjectDisposedException 异常".Test(async () =>
            {
                var ipcProviderA = new IpcProvider();
                var ipcProviderB = new IpcProvider();

                ipcProviderA.StartServer();
                ipcProviderB.StartServer();

                var peer = await ipcProviderA.GetAndConnectToPeerAsync(ipcProviderB.IpcContext.PipeName);
                var n = 100;
                var buffer = new byte[1024];
                var autoResetEvent = new AsyncAutoResetEvent(false);
                var sendTask = Task.Run(async () =>
                {
                    for (int i = 0; i < n; i++)
                    {
                        await peer.IpcMessageWriter.WriteMessageAsync(new IpcMessage("Test", buffer));

                        if (i == n / 2)
                        {
                            // 让另一个线程去释放 ipcProviderB 对象
                            autoResetEvent.Set();
                        }

                        await Task.Yield();
                    }
                });

                var disposeTask = Task.Run(async () =>
                {
                    await autoResetEvent.WaitOneAsync();
                    ipcProviderB.Dispose();
                });

                await Assert.ThrowsExceptionAsync<ObjectDisposedException>(async () =>
                {
                    await Task.WhenAll(sendTask, disposeTask);
                });
            });
        }
    }
}
