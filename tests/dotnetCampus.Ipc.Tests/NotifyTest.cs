using System;
using System.Threading;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MSTest.Extensions.Contracts;

namespace dotnetCampus.Ipc.Tests;

[TestClass]
public class NotifyTest
{
    [ContractTestCase]
    public void Notify()
    {
        "使用 LocalOneByOne 的线程调度，按照顺序调用 NotifyAsync 方法，但是不等待 NotifyAsync 方法执行完成，可以按照顺序接收".Test(async () =>
        {
            var name = "B_PeerNotifyTest_01";

            // 让 a 发送给 b 接收
            var a = new IpcProvider("A_PeerNotifyTest_01");
            var b = new IpcProvider(name, new IpcConfiguration()
            {
                // 使用 LocalOneByOne 的线程调度
                IpcTaskScheduling = IpcTaskScheduling.LocalOneByOne
            });

            a.StartServer();
            b.StartServer();

            int currentCount = 0;
            var taskCompletionSource = new TaskCompletionSource<bool>();
            const int count = byte.MaxValue - 10;

            b.PeerConnected += (sender, args) =>
            {
                var aPeer = args.Peer;
                aPeer.MessageReceived += (_, e) =>
                {
                    // 按照顺序调用 Notify 方法，可以按照顺序接收
                    // 发送的顺序就是首个不断加一，于是判断收到是否连续即可
                    Assert.AreEqual(currentCount, e.Message.Body.Buffer[0]);
                    Interlocked.Increment(ref currentCount);

                    if (currentCount == count)
                    {
                        // 全部执行完成
                        taskCompletionSource.SetResult(true);
                    }
                };
            };

            var peer = await a.GetAndConnectToPeerAsync(name);

            // 按照顺序调用 NotifyAsync 方法，但是不等待 NotifyAsync 方法执行完成
            for (int i = 0; i < count; i++)
            {
                _ = peer.NotifyAsync(new IpcMessage("测试使用 LocalOneByOne 的线程调度，按照顺序调用 Notify 方法，可以按照顺序接收",
                    new byte[] { (byte) i, 0xF1, 0xF2 }));
            }

            // 等待全部执行完成
            await Task.WhenAny(taskCompletionSource.Task, Task.Delay(TimeSpan.FromMinutes(5)));
            Assert.AreEqual(true, taskCompletionSource.Task.IsCompleted);
        });

        "使用 LocalOneByOne 的线程调度，按照顺序调用 Notify 方法，可以按照顺序接收".Test(async () =>
        {
            var name = "B_PeerNotifyTest";

            // 让 a 发送给 b 接收
            var a = new IpcProvider("A_PeerNotifyTest");
            var b = new IpcProvider(name, new IpcConfiguration()
            {
                // 使用 LocalOneByOne 的线程调度
                IpcTaskScheduling = IpcTaskScheduling.LocalOneByOne
            });

            a.StartServer();
            b.StartServer();

            int currentCount = 0;

            b.PeerConnected += (sender, args) =>
            {
                var aPeer = args.Peer;
                aPeer.MessageReceived += (_, e) =>
                {
                    // 按照顺序调用 Notify 方法，可以按照顺序接收
                    // 发送的顺序就是首个不断加一，于是判断收到是否连续即可
                    Assert.AreEqual(currentCount, e.Message.Body.Buffer[0]);
                    Interlocked.Increment(ref currentCount);
                };
            };

            var peer = await a.GetAndConnectToPeerAsync(name);

            // 按照顺序调用 Notify 方法
            for (int i = 0; i < byte.MaxValue - 10; i++)
            {
                await peer.NotifyAsync(new IpcMessage("测试使用 LocalOneByOne 的线程调度，按照顺序调用 Notify 方法，可以按照顺序接收",
                    new byte[] { (byte) i, 0xF1, 0xF2 }));
            }

        });
    }
}
