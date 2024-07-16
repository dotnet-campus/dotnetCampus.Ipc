using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Internals;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Utils.Buffers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dotnetCampus.Ipc.Tests
{
    [TestClass]
    public class IpcMessageConverterTest
    {
        [TestMethod("读取消息头不对的数据，可以返回读取失败")]
        public async Task IpcMessageConverterWriteAsync1()
        {
            using var memoryStream = new MemoryStream();

            ulong ack = 10;
            var buffer = new byte[] { 0x12, 0x12, 0x00 };
            var messageHeader = new byte[] { 0x00, 0x00 };
            var breakMessageHeader = new byte[] { 0x00, 0x01 };

            var ipcBufferMessageContext = new IpcBufferMessageContext("test", IpcMessageCommandType.Unknown, new IpcMessageBody(buffer));

            await IpcMessageConverter.WriteAsync(memoryStream, messageHeader, ack, ipcBufferMessageContext, new SharedArrayPool());

            memoryStream.Position = 0;
            var ipcMessageResult = (await IpcMessageConverter.ReadAsync(memoryStream,
                breakMessageHeader, new SharedArrayPool())).Result;
            var success = ipcMessageResult.Success;
            var ipcMessageCommandType = ipcMessageResult.IpcMessageCommandType;

            Assert.AreEqual(false, success);
            Assert.AreEqual(IpcMessageCommandType.Unknown, ipcMessageCommandType);
        }

        [TestMethod("读取消息头长度不对的数据，可以返回读取失败")]
        public async Task IpcMessageConverterWriteAsync2()
        {
            using var memoryStream = new MemoryStream();

            var ipcConfiguration = new IpcConfiguration();
            ulong ack = 10;
            var buffer = new byte[] { 0x12, 0x12, 0x00 };
            var messageHeader = new byte[] { 0x00, 0x00 };

            var ipcBufferMessageContext = new IpcBufferMessageContext("test", IpcMessageCommandType.Unknown, new IpcMessageBody(buffer));

            await IpcMessageConverter.WriteAsync(memoryStream, messageHeader, ack, ipcBufferMessageContext, new SharedArrayPool());

            memoryStream.Position = 0;
            var ipcMessageResult = (await IpcMessageConverter.ReadAsync(memoryStream,
                ipcConfiguration.MessageHeader, new SharedArrayPool())).Result;
            var success = ipcMessageResult.Success;
            var ipcMessageCommandType = ipcMessageResult.IpcMessageCommandType;

            Assert.AreEqual(false, success);
            Assert.AreEqual(IpcMessageCommandType.Unknown, ipcMessageCommandType);
        }

        [TestMethod("写入的数据和读取的相同，可以读取到写入的数据")]
        public async Task IpcMessageConverterWriteAsync3()
        {

            // 写入的数据和读取的相同
            using var memoryStream = new MemoryStream();

            var ipcConfiguration = new IpcConfiguration();
            ulong ack = 10;
            var buffer = new byte[] { 0x12, 0x12, 0x00 };
            var ipcBufferMessageContext = new IpcBufferMessageContext("test", IpcMessageCommandType.Unknown, new IpcMessageBody(buffer));
            await IpcMessageConverter.WriteAsync(memoryStream, ipcConfiguration.MessageHeader, ack, ipcBufferMessageContext, new SharedArrayPool());

            memoryStream.Position = 0;
            var (success, ipcMessageContext) = (await IpcMessageConverter.ReadAsync(memoryStream,
                ipcConfiguration.MessageHeader, new SharedArrayPool())).Result;

            Assert.AreEqual(true, success);
            Assert.AreEqual(ack, ipcMessageContext.Ack.Value);
        }
    }
}
