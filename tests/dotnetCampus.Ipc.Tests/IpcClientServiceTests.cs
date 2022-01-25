using System;
using System.IO;
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

                try
                {
                    // 等待发送和释放任务完成
                    // 预期是一定会在发送任务抛出异常，异常是 ObjectDisposedException 或 IOException 这两个
                    await Task.WhenAll(sendTask, disposeTask);
                }
                catch (Exception e)
                {
                    // 期望抛出异常
                    // 在调用一次 WriteMessageAsync 之前，被释放，那么抛出 ObjectDisposedException 异常
                    // 在写入过程，被释放，那么写入 PipeStream.WriteCore 抛出 IO 异常
                    Assert.AreEqual(true, e is ObjectDisposedException or IOException);
                    return;
                }

                // 如果没有抛出异常，那么进入此分支
                Assert.Fail("预期抛出的是 ObjectDisposedException 或 IOException 异常");
            });
        }
    }
}
