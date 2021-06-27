using System.IO;
using System.Threading.Tasks;
using dotnetCampus.Ipc.Abstractions;
using dotnetCampus.Ipc.Abstractions.Context;
using dotnetCampus.Ipc.PipeCore;
using dotnetCampus.Ipc.PipeCore.Context;
using dotnetCampus.Ipc.PipeCore.IpcPipe;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Extensions.Contracts;

namespace dotnetCampus.Ipc.Tests
{
    [TestClass]
    public class ResponseManagerTests
    {
        [ContractTestCase]
        public void GetResponseAsync()
        {
            "发送消息之后，能等待收到对应的回复".Test(() =>
            {
                var ipcMessageRequestManager = new IpcMessageRequestManager();
                var requestByteList = new byte[] { 0xFF, 0xFE };
                var request = new IpcRequestMessage("Tests", new IpcBufferMessage(requestByteList));
                var ipcClientRequestMessage = ipcMessageRequestManager.CreateRequestMessage(request);
                Assert.AreEqual(false, ipcClientRequestMessage.Task.IsCompleted);

                var requestStream = IpcBufferMessageContextToStream(ipcClientRequestMessage.IpcBufferMessageContext);

                IpcClientRequestArgs ipcClientRequestArgs = null;
                ipcMessageRequestManager.OnIpcClientRequestReceived += (sender, args) =>
                {
                    ipcClientRequestArgs = args;
                };

                Assert.IsNotNull(requestStream);
                ipcMessageRequestManager.OnReceiveMessage(new PeerMessageArgs("Foo", requestStream, ack: 100, IpcMessageCommandType.RequestMessage));

                Assert.IsNotNull(ipcClientRequestArgs);
                var responseByteList = new byte[] { 0xF1, 0xF2 };
                var ipcMessageResponseManager = new IpcMessageResponseManager();
                var responseMessageContext = ipcMessageResponseManager.CreateResponseMessage(ipcClientRequestArgs.MessageId, new IpcRequestMessage("Tests", new IpcBufferMessage(responseByteList)));
                var responseStream = IpcBufferMessageContextToStream(responseMessageContext);
                ipcMessageRequestManager.OnReceiveMessage(new PeerMessageArgs("Foo", responseStream, ack: 100, IpcMessageCommandType.ResponseMessage));

                Assert.AreEqual(true, ipcClientRequestMessage.Task.IsCompleted);
            });
        }

        private static Stream IpcBufferMessageContextToStream(IpcBufferMessageContext ipcBufferMessageContext)
        {
            var stream = new MemoryStream();
            foreach (var ipcBufferMessage in ipcBufferMessageContext.IpcBufferMessageList)
            {
                stream.Write(ipcBufferMessage.Buffer, ipcBufferMessage.Start, ipcBufferMessage.Count);
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
