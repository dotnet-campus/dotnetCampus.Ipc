using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Internals;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.PipeCore;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Utils.Extensions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MSTest.Extensions.Contracts;

namespace dotnetCampus.Ipc.Tests
{
    [TestClass]
    public class ResponseManagerTests
    {
        [ContractTestCase]
        public void SendAndGetResponse()
        {
            "发送消息到另一个 IPC 服务，可以等待收到对方的返回值".Test(async () =>
            {
                var ipcAName = Guid.NewGuid().ToString("N");
                var ipcBName = Guid.NewGuid().ToString("N");
                var requestByteList = new byte[] { 0xFF, 0xFE };
                var responseByteList = new byte[] { 0xF1, 0xF2 };

                using var ipcA = new IpcProvider(ipcAName);
                using var ipcB = new IpcProvider(ipcBName, new IpcConfiguration()
                {
                    DefaultIpcRequestHandler = new DelegateIpcRequestHandler(c =>
                    {
                        Assert.AreEqual(ipcAName, c.Peer.PeerName);
                        c.Handled = true;
                        var span = c.IpcBufferMessage.Body.AsSpan();
                        Assert.AreEqual(true, span.SequenceEqual(requestByteList));

                        return new IpcHandleRequestMessageResult(new IpcMessage("Return",
                            new IpcMessageBody(responseByteList)));
                    })
                });
                ipcA.StartServer();
                ipcB.StartServer();

                var bPeer = await ipcA.GetAndConnectToPeerAsync(ipcBName);
                // 从 A 发送消息给到 B 然后可以收到从 B 返回的消息
                var response =
                    await bPeer.GetResponseAsync(new IpcMessage("发送", new IpcMessageBody(requestByteList)));
                Assert.AreEqual(true, response.Body.AsSpan().SequenceEqual(responseByteList));
            });
        }

        [ContractTestCase]
        public void GetResponseAsync()
        {
            "发送消息之后，能等待收到对应的回复".Test(async () =>
            {
                var ipcMessageRequestManager = new IpcMessageRequestManager(new IpcProvider().IpcContext);
                var requestByteList = new byte[] { 0xFF, 0xFE };
                var request = new IpcMessage("Tests", new IpcMessageBody(requestByteList));
                var ipcClientRequestMessage = ipcMessageRequestManager.CreateRequestMessage(request);
                Assert.AreEqual(false, ipcClientRequestMessage.Task.IsCompleted);

                var requestStream = IpcBufferMessageContextToStream(ipcClientRequestMessage.IpcBufferMessageContext);

                IpcClientRequestArgs ipcClientRequestArgs = null;
                ipcMessageRequestManager.OnIpcClientRequestReceived += (sender, args) =>
                {
                    ipcClientRequestArgs = args;
                };

                Assert.IsNotNull(requestStream);
                ipcMessageRequestManager.OnReceiveMessage(new PeerStreamMessageArgs(new IpcMessageContext(), "Foo", requestStream, ack: 100,
                    IpcMessageCommandType.RequestMessage));

                Assert.IsNotNull(ipcClientRequestArgs);
                var responseByteList = new byte[] { 0xF1, 0xF2 };
                var ipcMessageResponseManager = new IpcMessageResponseManager();
                var responseMessageContext = ipcMessageResponseManager.CreateResponseMessage(
                    ipcClientRequestArgs.MessageId,
                    new IpcMessage("Tests", new IpcMessageBody(responseByteList)));
                var responseStream = IpcBufferMessageContextToStream(responseMessageContext);
                ipcMessageRequestManager.OnReceiveMessage(new PeerStreamMessageArgs(new IpcMessageContext(), "Foo", responseStream, ack: 100,
                    IpcMessageCommandType.ResponseMessage));

                // 在 OnReceiveMessage 收到消息，不是立刻释放 ipcClientRequestMessage 的，需要调度到线程池进行释放
                await ipcClientRequestMessage.Task.WaitTimeout(TimeSpan.FromSeconds(5));
                Assert.AreEqual(true, ipcClientRequestMessage.Task.IsCompleted);
            });
        }

        [ContractTestCase]
        public void WaitingResponseCount()
        {
            "所有发送消息都收到回复后，将清空等待响应的数量".Test(async () =>
            {
                // 请求的顺序是
                // A: 生成请求消息
                // A: 发送请求消息
                // B: 收到请求消息
                // B: 生成回复消息
                // B: 发送回复消息
                // A: 收到回复消息
                // A: 完成请求
                var aIpcMessageRequestManager = new IpcMessageRequestManager(new IpcProvider().IpcContext);
                var requestByteList = new byte[] { 0xFF, 0xFE };
                var request = new IpcMessage("Tests", new IpcMessageBody(requestByteList));

                var ipcClientRequestMessageList = new List<IpcClientRequestMessage>();

                for (int i = 0; i < 10; i++)
                {
                    // 创建请求消息
                    IpcClientRequestMessage ipcClientRequestMessage = aIpcMessageRequestManager.CreateRequestMessage(request);
                    ipcClientRequestMessageList.Add(ipcClientRequestMessage);

                    Assert.AreEqual(i + 1, aIpcMessageRequestManager.WaitingResponseCount);
                }

                // 创建的请求消息还没发送出去，需要进行发送
                // 发送的做法就是往 B 里面调用接收方法
                // 在测试里面不引入 IPC 的发送逻辑，因此 A 的发送就是调用 B 的接收
                var bIpcMessageRequestManager = new IpcMessageRequestManager(new IpcProvider().IpcContext);
                var bIpcMessageResponseManager = new IpcMessageResponseManager();

                // 接收 B 的消息，用的是事件
                var ipcClientRequestArgsList = new List<IpcClientRequestArgs>();
                bIpcMessageRequestManager.OnIpcClientRequestReceived += (sender, args) =>
                {
                    ipcClientRequestArgsList.Add(args);
                };

                // 开始发送消息
                foreach (var ipcClientRequestMessage in ipcClientRequestMessageList)
                {
                    var requestStream = IpcBufferMessageContextToStream(ipcClientRequestMessage.IpcBufferMessageContext);
                    var args = new PeerStreamMessageArgs(new IpcMessageContext(), "Foo", requestStream, ack: 100,
                         IpcMessageCommandType.RequestMessage);

                    bIpcMessageRequestManager.OnReceiveMessage(args);
                }

                // 因为 A 发送了 10 条消息，因此 B 需要接收到 10 条
                Assert.AreEqual(ipcClientRequestMessageList.Count, ipcClientRequestArgsList.Count);

                // 逐条消息回复
                foreach (var ipcClientRequestArgs in ipcClientRequestArgsList)
                {
                    var responseByteList = new byte[] { 0xF1, 0xF2 };
                    var responseMessageContext = bIpcMessageResponseManager.CreateResponseMessage(
                        ipcClientRequestArgs.MessageId,
                        new IpcMessage("Tests", new IpcMessageBody(responseByteList)));
                    var responseStream = IpcBufferMessageContextToStream(responseMessageContext);

                    // 回复就是发送消息给 A 相当于让 A 接收消息
                    aIpcMessageRequestManager.OnReceiveMessage(new PeerStreamMessageArgs(new IpcMessageContext(), "Foo", responseStream, ack: 100,
                        IpcMessageCommandType.ResponseMessage));
                }

                // 此时 A 没有等待回复的消息
                Assert.AreEqual(0, aIpcMessageRequestManager.WaitingResponseCount);
                // 所有发送的消息都收到回复

                foreach (var ipcClientRequestMessage in ipcClientRequestMessageList)
                {
                    // 在 OnReceiveMessage 收到消息，不是立刻释放 ipcClientRequestMessage 的，需要调度到线程池进行释放
                    await ipcClientRequestMessage.Task.WaitTimeout();

                    Assert.AreEqual(true, ipcClientRequestMessage.Task.IsCompleted);
                }
            });
        }

        private static MemoryStream IpcBufferMessageContextToStream(IpcBufferMessageContext ipcBufferMessageContext)
        {
            var stream = new MemoryStream();
            foreach (var ipcBufferMessage in ipcBufferMessageContext.IpcBufferMessageList)
            {
                stream.Write(ipcBufferMessage.Buffer, ipcBufferMessage.Start, ipcBufferMessage.Length);
            }

            stream.Position = 0;
            return stream;
        }

        class FakeClientMessageWriter : IClientMessageWriter
        {
            public Task WriteMessageAsync(byte[] buffer, int offset, int count, string summary = null)
            {
                throw new System.NotImplementedException();
            }

            public Stream Stream { set; get; }

            public Task WriteMessageAsync(in IpcBufferMessageContext ipcBufferMessageContext)
            {
                Stream = IpcBufferMessageContextToStream(ipcBufferMessageContext);

                return Task.CompletedTask;
            }
        }
    }
}
