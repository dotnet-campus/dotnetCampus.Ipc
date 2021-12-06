using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Internals;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Utils.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Extensions.Contracts;

namespace dotnetCampus.Ipc.Tests
{
    [TestClass]
    public class PeerReConnectorTest
    {
        [ContractTestCase]
        public void Reconnect()
        {
            "连接过程中，对方断掉，可以自动重新连接对方".Test(async () =>
            {
                // 让 a 去连接 b 然后聊聊天
                // 接着将 b 结束，此时 a 的 peer 将会断开连接
                // 然后启动 c 让 c 用原本 b 的 name 从而假装是 b 重启
                // 预期是 a 会重新连接到 "b" 继续聊天
                var name = "B_PeerReConnectorTest";
                var aRequest = new byte[] { 0xF1 };
                var bResponse = new byte[] { 0xF1, 0xF2 };
                var cResponse = new byte[] { 0x01, 0x05 };

                var a = new IpcProvider("A_PeerReConnectorTest", new IpcConfiguration()
                {
                    AutoReconnectPeers = true
                });
                var b = new IpcProvider(name,
                    new IpcConfiguration()
                    {
                        DefaultIpcRequestHandler = new DelegateIpcRequestHandler(context =>
                            new IpcHandleRequestMessageResult(new IpcMessage("B回复", bResponse)))
                    });

                a.StartServer();
                b.StartServer();

                var peer = await a.GetAndConnectToPeerAsync(name);
                var request1 = await peer.GetResponseAsync(new IpcMessage("A发送", aRequest));
                Assert.AreEqual(true, bResponse.AsSpan().SequenceEqual(request1.Body.AsSpan()));
                await Task.Yield();

                var peerBrokenTask = new TaskCompletionSource<bool>();
                peer.PeerConnectionBroken += delegate { peerBrokenTask.TrySetResult(true); };

                b.Dispose();

                // 预期 b 结束时，能收到 PeerConnectionBroken 事件
                await Task.WhenAny(peerBrokenTask.Task, Task.Delay(TimeSpan.FromSeconds(2)));

                if (!peerBrokenTask.Task.IsCompleted)
                {
#if DEBUG
                    // 进入断点，也许上面的时间太短
                    await Task.WhenAny(peerBrokenTask.Task, Task.Delay(TimeSpan.FromMinutes(5)));
#endif
                }

                // 判断是否能收到对方断开的消息
                Assert.AreEqual(true, peerBrokenTask.Task.IsCompleted);
                Assert.AreEqual(true, peerBrokenTask.Task.Result);

                Assert.AreEqual(true, peer.IsBroken);
                Assert.AreEqual(false, peer.WaitForFinishedTaskCompletionSource.Task.IsCompleted);

                // 启动 c 用来假装 b 重启，能让 a 自动用原先的 Peer 连接
                var c = new IpcProvider(name,
                    new IpcConfiguration()
                    {
                        DefaultIpcRequestHandler = new DelegateIpcRequestHandler(context =>
                            new IpcHandleRequestMessageResult(new IpcMessage("C回复", cResponse)))
                    });
                c.StartServer();

                // 等待 a 重新连接
                await Task.WhenAny(peer.WaitForFinishedTaskCompletionSource.Task, Task.Delay(TimeSpan.FromSeconds(2)));
                if (!peer.WaitForFinishedTaskCompletionSource.Task.IsCompleted)
                {
#if DEBUG
                    // 进入断点，也许上面的时间太短
                    await Task.WhenAny(peer.WaitForFinishedTaskCompletionSource.Task, Task.Delay(TimeSpan.FromMinutes(2)));
#endif
                }

                var request2 = await peer.GetResponseAsync(new IpcMessage("A发送", aRequest));
                Assert.AreEqual(true, cResponse.AsSpan().SequenceEqual(request2.Body.AsSpan()));
            });
        }
    }


}
