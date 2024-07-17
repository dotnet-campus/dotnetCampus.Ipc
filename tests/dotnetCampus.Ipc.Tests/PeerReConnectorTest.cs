using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Internals;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Pipes.PipeConnectors;
using dotnetCampus.Ipc.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dotnetCampus.Ipc.Tests
{
    [TestClass]
    public class PeerReConnectorTest
    {
        [TestMethod("设置自动重新连接，但是重新连接器里面永远返回不继续连接。在对方结束之后，重新再开始，可以被重复连接")]
        public async Task IpcClientPipeConnectorTest()
        {
            // 先启动 a 和 b 两个
            // 让 b 主动连接 a 然后聊聊天
            // 接着将 b 结束，此时 a 的 peer 将会断开连接
            // 然后启动 c 让 c 用原本 b 的 name 从而假装是 b 重新再开始
            // 让 c 主动连接 a 然后聊聊天
            // 预期可以让 a 获取到 c 的连接事件

            var aName = "A_PeerConnectorTest_02";
            var bName = "B_PeerConnectorTest_02";
            var aResponse = new byte[] { 0xF1, 0xF3 };
            var bRequest = new byte[] { 0xF1, 0xF2, 0xF3 };
            var cRequest = new byte[] { 0x01, 0x05, 0xF3 };

            var a = new IpcProvider(aName, new IpcConfiguration()
            {
                AutoReconnectPeers = true, // 设置自动重新连接
                // 但是重新连接器里面永远返回不继续连接
                IpcClientPipeConnector =
                    new IpcClientPipeConnector(context => false,
                        // 设置每一步都是快速超时
                        stepTimeout: TimeSpan.FromMilliseconds(100)),
                DefaultIpcRequestHandler = new DelegateIpcRequestHandler(context =>
                    new IpcHandleRequestMessageResult(new IpcMessage("B回复", aResponse))),
                IpcTaskScheduling = IpcTaskScheduling.LocalOneByOne,
            });

            var connectCount = 0;
            var peerBrokenTask = new TaskCompletionSource<bool>();
            a.PeerConnected += (sender, args) =>
            {
                connectCount++;

                args.Peer.PeerConnectionBroken += (o, brokenArgs) =>
                {
                    peerBrokenTask.TrySetResult(true);
                };
            };

            var b = new IpcProvider(bName, new IpcConfiguration()
            {
                AutoReconnectPeers = true, // 设置自动重新连接
                // 但是重新连接器里面永远返回不继续连接
                IpcClientPipeConnector = new IpcClientPipeConnector(context => false),
                IpcTaskScheduling = IpcTaskScheduling.LocalOneByOne,
            });

            a.StartServer();
            b.StartServer();

            // 让 b 主动连接 a 然后聊聊天
            var peer1 = await b.GetOrCreatePeerProxyAsync(aName);
            var request1 = await peer1.GetResponseAsync(new IpcMessage("Test", bRequest));
            Assert.AreEqual(true, aResponse.AsSpan().SequenceEqual(request1.Body.AsSpan()));
            await Task.Yield();

            // 能收到一次连接。这是预期的
            Assert.AreEqual(1, connectCount);

            var peerReconnectedCount = 0;
            peer1.PeerReconnected += (sender, args) =>
            {
                peerReconnectedCount++;
            };

            // 将 b 结束，此时 a 的 peer 将会断开连接
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

            // 等待重新连接失败
            await Task.Delay(TimeSpan.FromSeconds(5));

            // 确定 b 断开，再启动 c 去主动连接
            var c = new IpcProvider(bName, new IpcConfiguration()
            {
                AutoReconnectPeers = true, // 设置自动重新连接
                // 但是重新连接器里面永远返回不继续连接
                IpcClientPipeConnector = new IpcClientPipeConnector(context => false),
                IpcTaskScheduling = IpcTaskScheduling.LocalOneByOne,
            });
            c.StartServer();

            // 启动 c 去主动连接
            var peer2 = await c.GetOrCreatePeerProxyAsync(aName);
            // 发送一条请求获取响应，可以证明连接到符合预期的。同时也等待对方完成连接
            var request2 = await peer2.GetResponseAsync(new IpcMessage("Test", cRequest));
            Assert.AreEqual(true, aResponse.AsSpan().SequenceEqual(request2.Body.AsSpan()));

            // 可以被重复连接
            // 也就是会被连接两次
            // 不存在被重复连接
            Assert.AreEqual(2, connectCount);
            Assert.AreEqual(0, peerReconnectedCount);
        }

        [TestMethod("不自动重新连接，对方结束之后，重新再开始，可以被重复连接")]
        public async Task Connect()
        {
            // 先启动 a 和 b 两个
            // 让 b 主动连接 a 然后聊聊天
            // 接着将 b 结束，此时 a 的 peer 将会断开连接
            // 然后启动 c 让 c 用原本 b 的 name 从而假装是 b 重新再开始
            // 让 c 主动连接 a 然后聊聊天
            // 预期可以让 a 获取到 c 的连接事件

            var aName = "A_PeerConnectorTest_01";
            var bName = "B_PeerConnectorTest_01";
            var aResponse = new byte[] { 0xF1, 0xF3 };
            var bRequest = new byte[] { 0xF1, 0xF2, 0xF3 };
            var cRequest = new byte[] { 0x01, 0x05, 0xF3 };

            var a = new IpcProvider(aName, new IpcConfiguration()
            {
                AutoReconnectPeers = false, // 不自动重新连接
                DefaultIpcRequestHandler = new DelegateIpcRequestHandler(context =>
                    new IpcHandleRequestMessageResult(new IpcMessage("B回复", aResponse))),
                IpcTaskScheduling = IpcTaskScheduling.LocalOneByOne,
            });

            var connectCount = 0;
            var peerBrokenTask = new TaskCompletionSource<bool>();
            a.PeerConnected += (sender, args) =>
            {
                connectCount++;

                args.Peer.PeerConnectionBroken += (o, brokenArgs) =>
                {
                    peerBrokenTask.TrySetResult(true);
                };
            };

            var b = new IpcProvider(bName, new IpcConfiguration()
            {
                AutoReconnectPeers = false, // 不自动重新连接
                IpcTaskScheduling = IpcTaskScheduling.LocalOneByOne,
            });

            a.StartServer();
            b.StartServer();

            // 让 b 主动连接 a 然后聊聊天
            var peer1 = await b.GetOrCreatePeerProxyAsync(aName);
            var request1 = await peer1.GetResponseAsync(new IpcMessage("Test", bRequest));
            Assert.AreEqual(true, aResponse.AsSpan().SequenceEqual(request1.Body.AsSpan()));
            await Task.Yield();

            // 能收到一次连接。这是预期的
            Assert.AreEqual(1, connectCount);

            var peerReconnectedCount = 0;
            peer1.PeerReconnected += (sender, args) =>
            {
                peerReconnectedCount++;
            };

            // 将 b 结束，此时 a 的 peer 将会断开连接
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

            // 确定 b 断开，再启动 c 去主动连接
            var c = new IpcProvider(bName, new IpcConfiguration()
            {
                AutoReconnectPeers = false, // 不自动重新连接
                IpcTaskScheduling = IpcTaskScheduling.LocalOneByOne,
            });
            c.StartServer();

            // 启动 c 去主动连接
            var peer2 = await c.GetOrCreatePeerProxyAsync(aName);
            // 发送一条请求获取响应，可以证明连接到符合预期的。同时也等待对方完成连接
            var request2 = await peer2.GetResponseAsync(new IpcMessage("Test", cRequest));
            Assert.AreEqual(true, aResponse.AsSpan().SequenceEqual(request2.Body.AsSpan()));

            // 可以被重复连接
            // 也就是会被连接两次
            // 不存在被重复连接
            Assert.AreEqual(2, connectCount);
            Assert.AreEqual(0, peerReconnectedCount);
        }

        [TestMethod("连接过程中，对方断掉，可以自动重新连接对方")]
        public async Task Reconnect()
        {
            // 让 a 去连接 b 然后聊聊天
            // 接着将 b 结束，此时 a 的 peer 将会断开连接
            // 然后启动 c 让 c 用原本 b 的 name 从而假装是 b 重启
            // 预期是 a 会重新连接到 "b" 继续聊天
            var name = "B_PeerReConnectorTest";
            var aRequest = new byte[] { 0xF1 };
            var bResponse = new byte[] { 0xF1, 0xF2 };
            var cResponse = new byte[] { 0x01, 0x05 };

            var a = new IpcProvider("A_PeerReConnectorTest", new IpcConfiguration() { AutoReconnectPeers = true });
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
        }
    }
}
