﻿using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Internals;
using dotnetCampus.Ipc.Utils.Buffers;
using dotnetCampus.Ipc.Utils.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dotnetCampus.Ipc.Tests
{
    [TestClass]
    public class PeerRegisterProviderTests
    {
        [TestMethod("如果注册消息的内容添加了其他内容，不会读取到不属于注册消息的内容")]
        public void BuildPeerRegisterMessage1()
        {
            // 创建的内容可以序列化
            var peerRegisterProvider = new PeerRegisterProvider();
            var pipeName = "123";
            var bufferMessageContext = peerRegisterProvider.BuildPeerRegisterMessage(pipeName);
            var memoryStream = new MemoryStream(bufferMessageContext.Length);

            foreach (var ipcBufferMessage in bufferMessageContext.IpcBufferMessageList)
            {
                memoryStream.Write(ipcBufferMessage.Buffer, ipcBufferMessage.Start, ipcBufferMessage.Length);
            }

            // 写入其他内容
            var streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write("林德熙是逗比");
            streamWriter.Flush();

            memoryStream.Position = 0;

            var success = peerRegisterProvider.TryParsePeerRegisterMessage(memoryStream, out var peerName);

            Assert.AreEqual(true, success);
            Assert.AreEqual(pipeName, peerName);
        }

        [TestMethod("如果消息不是对方的注册消息，那么将不修改Stream的起始")]
        public void BuildPeerRegisterMessage2()
        {
            var peerRegisterProvider = new PeerRegisterProvider();
            var memoryStream = new MemoryStream();
            for (int i = 0; i < 1000; i++)
            {
                memoryStream.WriteByte(0x00);
            }

            const int position = 10;
            memoryStream.Position = position;
            var isPeerRegisterMessage = peerRegisterProvider.TryParsePeerRegisterMessage(memoryStream, out _);
            Assert.AreEqual(false, isPeerRegisterMessage);
            Assert.AreEqual(position, memoryStream.Position);
        }

        [TestMethod("使用发送端之后，能序列化之前的字符串")]
        public async Task BuildPeerRegisterMessage3()
        {
            var peerRegisterProvider = new PeerRegisterProvider();
            var pipeName = "123";
            var bufferMessageContext = peerRegisterProvider.BuildPeerRegisterMessage(pipeName);
            var memoryStream = new MemoryStream(bufferMessageContext.Length);
            var ipcConfiguration = new IpcConfiguration();

            await IpcMessageConverter.WriteAsync(memoryStream, ipcConfiguration.MessageHeader, ack: 10,
                bufferMessageContext, new SharedArrayPool());

            memoryStream.Position = 0;
            var (success, ipcMessageContext) = (await IpcMessageConverter.ReadAsync(memoryStream,
                ipcConfiguration.MessageHeader, new SharedArrayPool())).Result;

            Assert.AreEqual(true, success);

            var stream = new ByteListMessageStream(ipcMessageContext);
            success = peerRegisterProvider.TryParsePeerRegisterMessage(stream, out var peerName);

            Assert.AreEqual(true, success);

            Assert.AreEqual(pipeName, peerName);
        }

        [TestMethod("创建的注册服务器名内容可以序列化，序列化之后可以反序列化出服务器名")]
        public void BuildPeerRegisterMessage4()
        {
            // 创建的内容可以序列化
            var peerRegisterProvider = new PeerRegisterProvider();
            var pipeName = "123";
            var bufferMessageContext = peerRegisterProvider.BuildPeerRegisterMessage(pipeName);
            var memoryStream = new MemoryStream(bufferMessageContext.Length);

            foreach (var ipcBufferMessage in bufferMessageContext.IpcBufferMessageList)
            {
                memoryStream.Write(ipcBufferMessage.Buffer, ipcBufferMessage.Start, ipcBufferMessage.Length);
            }

            memoryStream.Position = 0;

            var success = peerRegisterProvider.TryParsePeerRegisterMessage(memoryStream, out var peerName);

            Assert.AreEqual(true, success);
            Assert.AreEqual(pipeName, peerName);
        }
    }
}
