using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Exceptions;
using dotnetCampus.Ipc.Internals;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Utils.Extensions;
using dotnetCampus.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MSTest.Extensions.Contracts;

namespace dotnetCampus.Ipc.Tests
{
    [TestClass]
    public class PeerProxyTest
    {
        [ContractTestCase]
        public void SendWithReconnect()
        {
            "断开连接过程中，所有请求响应，都可以在重连之后请求成功".Test(async () =>
            {
                var name = "B_PeerReconnected";
                var aRequest = new byte[] { 0xF1 };
                var cResponse = new byte[] { 0xF1, 0xF2 };

                var a = new IpcProvider("A_PeerReconnected", new IpcConfiguration()
                {
                    AutoReconnectPeers = true,
                });
                // 毕竟一会就要挂了，啥都不需要配置
                var b = new IpcProvider(name);

                a.StartServer();
                b.StartServer();

                var peer = await a.GetAndConnectToPeerAsync(name);

                // 连接成功了，那么就让 b 凉凉
                b.Dispose();

                // 等待后台所有断开成功
                await Task.Delay(TimeSpan.FromSeconds(2));

                // 预期状态是 Peer 是断开的，等待重新连接
                Assert.AreEqual(true, peer.IsBroken);
                Assert.AreEqual(false, peer.WaitForFinishedTaskCompletionSource.Task.IsCompleted);

                // 开始请求响应，预期进入等待，没有任何响应
                var requestTask = peer.GetResponseAsync(new IpcMessage("A发送", aRequest));

                // 重新启动 b 服务，用法是再新建一个 c 用了 b 的 name 从而假装是 b 重启
                var c = new IpcProvider(name, new IpcConfiguration()
                {
                    DefaultIpcRequestHandler = new DelegateIpcRequestHandler(context =>
                        new IpcHandleRequestMessageResult(new IpcMessage("C回复", cResponse)))
                });
                c.StartServer();

                // 预期可以收到
                await requestTask.WaitTimeout(TimeSpan.FromSeconds(3));
                var ipcMessage = await requestTask;
                Assert.AreEqual(true, ipcMessage.Body.AsSpan().SequenceEqual(cResponse));
            });

            "断开连接过程中，发送的所有消息，都可以在重连之后发送".Test(async () =>
            {
                var name = "B_PeerReconnected_F2";
                var aRequest = new byte[] { 0xF1 };

                var a = new IpcProvider("A_PeerReconnected_F2", new IpcConfiguration()
                {
                    AutoReconnectPeers = true,
                });
                // 毕竟一会就要挂了，啥都不需要配置
                var b = new IpcProvider(name);

                a.StartServer();
                b.StartServer();

                var peer = await a.GetAndConnectToPeerAsync(name);

                // 连接成功了，那么就让 b 凉凉
                b.Dispose();

                // 等待后台所有断开成功
                await Task.Delay(TimeSpan.FromSeconds(2));

                // 预期状态是 Peer 是断开的，等待重新连接
                Assert.AreEqual(true, peer.IsBroken);
                Assert.AreEqual(false, peer.WaitForFinishedTaskCompletionSource.Task.IsCompleted);

                // 开始发送消息，此时发送消息的任务都在进入等待
                var notifyTask = peer.NotifyAsync(new IpcMessage("A发送", aRequest));

                // 稍微等一下，此时预期还是没有发送完成
                await Task.WhenAny(notifyTask, Task.Delay(TimeSpan.FromMilliseconds(100)));
                Assert.AreEqual(false, notifyTask.IsCompleted);

                // 重新启动 b 服务，用法是再新建一个 c 用了 b 的 name 从而假装是 b 重启
                var c = new IpcProvider(name);

                // 是否可以收到重新发送消息
                var receiveANotifyTask = new TaskCompletionSource<bool>();
                c.PeerConnected += (s, e) =>
                {
                    // 预期这里是 A 连接过来
                    e.Peer.MessageReceived += (sender, args) =>
                    {
                        if (args.Message.Body.AsSpan().SequenceEqual(aRequest))
                        {
                            receiveANotifyTask.SetResult(true);
                        }
                    };
                };

                var receiveAFromGlobalMessageReceived = new TaskCompletionSource<bool>();
                c.IpcServerService.MessageReceived += (s, e) =>
                {
                    if (e.Message.Body.AsSpan().SequenceEqual(aRequest))
                    {
                        receiveAFromGlobalMessageReceived.SetResult(true);
                    }
                };

                c.StartServer();

                await receiveANotifyTask.Task.WaitTimeout(TimeSpan.FromSeconds(5));
                // 发送成功
                Assert.AreEqual(true, notifyTask.IsCompleted);
                if (receiveANotifyTask.Task.IsCompleted)
                {
                    // 和平，能收到重新连接发过来的消息
                }
                else
                {
                    if (receiveAFromGlobalMessageReceived.Task.IsCompleted)
                    {
                        // 如果被全局收了，那也是预期的，因为连接过来之后，立刻收到消息，此时的 e.Peer.MessageReceived+=xx 的代码还没跑
                    }
                    else
                    {
                        Assert.Fail("没有收到重连的消息");
                    }
                }
            });
        }

        [ContractTestCase]
        public void GetResponseAsync()
        {
            "向对方请求响应，可以拿到对方的回复".Test(async () =>
            {
                var name = "B_PeerReconnected_GetResponseAsync";
                var aRequest = new byte[] { 0xF1 };
                var bResponse = new byte[] { 0xF1, 0xF2 };

                var a = new IpcProvider("A_PeerReconnected_GetResponseAsync");
                var b = new IpcProvider(name, new IpcConfiguration()
                {
                    DefaultIpcRequestHandler = new DelegateIpcRequestHandler(context =>
                        new IpcHandleRequestMessageResult(new IpcMessage("B回复", bResponse)))
                });

                a.StartServer();
                b.StartServer();

                var peer = await a.GetAndConnectToPeerAsync(name);
                var request1 = await peer.GetResponseAsync(new IpcMessage("A发送", aRequest));
                Assert.AreEqual(true, bResponse.AsSpan().SequenceEqual(request1.Body.AsSpan()));

                // 多次发送消息测试一下
                var request2 = await peer.GetResponseAsync(new IpcMessage("A发送", aRequest));
                Assert.AreEqual(true, bResponse.AsSpan().SequenceEqual(request2.Body.AsSpan()));
            });
        }

        [ContractTestCase]
        public void PeerReconnected()
        {
            "连接过程中，对方断掉重连，可以收到重连事件".Test(async () =>
            {
                var name = "B_PeerReconnected_Main";
                var a = new IpcProvider("A_PeerReconnected_Main", new IpcConfiguration()
                {
                    AutoReconnectPeers = true
                });
                var b = new IpcProvider(name);

                a.StartServer();
                b.StartServer();

                var peer = await a.GetAndConnectToPeerAsync(name);
                var peerReconnectedTask = new TaskCompletionSource<bool>();
                peer.PeerReconnected += delegate
                {
                    peerReconnectedTask.SetResult(true);
                };

                // 断开 b 此时只会收到断开消息，不会收到重连消息
                b.Dispose();

                // 等待2秒，预计此时是不会收到重新连接消息，也就是 peerReconnectedTask 任务还没完成
                await Task.Delay(TimeSpan.FromSeconds(2));
                // 判断此时是否收到重连消息
                Assert.AreEqual(false, peerReconnectedTask.Task.IsCompleted, "还没有重启 b 服务，但是已收到重连消息");

                // 重新启动 b 服务，用法是再新建一个 c 用了 b 的 name 从而假装是 b 重启
                var c = new IpcProvider(name);
                c.StartServer();

                // 多线程，需要等待一下，等待连接
                await peerReconnectedTask.Task.WaitTimeout(TimeSpan.FromSeconds(3));

                Assert.AreEqual(true, peerReconnectedTask.Task.IsCompleted);
                Assert.AreEqual(true, peerReconnectedTask.Task.Result);

                Assert.AreEqual(true, peer.IsConnectedFinished);
                Assert.AreEqual(false, peer.IsBroken);
            });
        }

        [ContractTestCase]
        public void PeerConnectionBroken()
        {
            "发送请求响应，对方断掉，请求将会抛出异常".Test(async () =>
            {
                var name = "B_BreakAllRequestTaskByIpcBroken";
                var aRequest = new byte[] { 0xF2 };
                var asyncManualResetEvent = new AsyncManualResetEvent(false);
                var a = new IpcProvider("A_BreakAllRequestTaskByIpcBroken");
                var b = new IpcProvider(name,
                    new IpcConfiguration()
                    {
                        DefaultIpcRequestHandler = new DelegateIpcRequestHandler(context =>
                            Task.Run<IIpcResponseMessage>(async () =>
                            {
                                await asyncManualResetEvent.WaitOneAsync();
                                return (IIpcResponseMessage) null;
                            }))
                    });

                a.StartServer();
                b.StartServer();

                var peer = await a.GetAndConnectToPeerAsync(name);

                // 发送请求，预期这些请求都没有收到回复
                var taskList = new List<Task<IpcMessage>>();
                for (int i = 0; i < 10; i++)
                {
                    Task<IpcMessage> responseTask = peer.GetResponseAsync(new IpcMessage("A发送", aRequest));
                    taskList.Add(responseTask);
                }

                await Task.Yield();

                foreach (var task in taskList)
                {
                    Assert.AreEqual(false, task.IsCompleted);
                }

                // 让消息写入一下
                await Task.Delay(TimeSpan.FromSeconds(2));

                b.Dispose();

                // 等待断开
                await Task.Delay(TimeSpan.FromSeconds(5));
                foreach (var task in taskList)
                {
                    Assert.AreEqual(true, task.IsCompleted);

                    // 这里的异常也许是 连接断开异常， 也许是写入过程中，对方已断开异常
                    Assert.IsNotNull(task.Exception?.InnerExceptions[0] as Exception);
                }

                // 所有请求都炸掉
                Assert.AreEqual(0, peer.IpcMessageRequestManager.WaitingResponseCount);
            });

            "连接过程中，对方断掉，可以收到对方断掉的消息".Test(async () =>
            {
                // 让 a 去连接 b 然后聊聊天
                // 接着将 b 结束，此时 a 的 peer 将会断开连接
                var name = "B_PeerConnectionBroken_PeerConnectionBroken";
                var aRequest = new byte[] { 0xF1 };
                var bResponse = new byte[] { 0xF1, 0xF2 };

                var a = new IpcProvider("A_PeerConnectionBroken_PeerConnectionBroken");
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
            });
        }

        [ContractTestCase]
        public void Dispose()
        {
            "使用释放的服务发送消息，将会提示对象释放".Test(async () =>
            {
                var name = "B_PeerConnectionBroken_Dispose";

                var aRequest = new byte[] { 0xF1 };
                var a = new IpcProvider("A_PeerConnectionBroken_Dispose", new IpcConfiguration()
                {
                    AutoReconnectPeers = true
                });
                var b = new IpcProvider(name);

                a.StartServer();
                b.StartServer();

                var peer = await a.GetAndConnectToPeerAsync(name);

                // 设置为自动重连的服务，释放
                a.Dispose();

                // 等待资源的释放
                await Task.Delay(TimeSpan.FromSeconds(2));

                await Assert.ThrowsExceptionAsync<ObjectDisposedException>(async () =>
                {
                    await peer.NotifyAsync(new IpcMessage("A发送", aRequest));
                });
            });

            "设置为自动重连的服务，释放之后，不会有任何资源进入等待".Test(async () =>
            {
                var name = "B_PeerConnectionBroken_Dispose2";

                var a = new IpcProvider("A_PeerConnectionBroken_Dispose2", new IpcConfiguration()
                {
                    AutoReconnectPeers = true
                });
                var b = new IpcProvider(name);

                a.StartServer();
                b.StartServer();

                var peer = await a.GetAndConnectToPeerAsync(name);

                // 设置为自动重连的服务，释放
                a.Dispose();

                // 等待资源的释放
                await Task.Delay(TimeSpan.FromSeconds(2));
#if DEBUG
                for (int i = 0; i < 1000; i++)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
#endif

                Assert.AreEqual(true, peer.IsBroken);
                Assert.AreEqual(true, peer.WaitForFinishedTaskCompletionSource.Task.IsCompleted);
            });
        }
    }
}
