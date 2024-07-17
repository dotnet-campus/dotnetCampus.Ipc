using System.Text;

using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Utils.Extensions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dotnetCampus.Ipc.Tests.Utils.Extensions;

[TestClass]
public class PeerMessageArgsExtensionTest
{
    [TestMethod("给定的 IpcMessage 不包含正确 byte 数组的有效负载，获取有效负载失败")]
    public void GetPayload1()
    {
        var header = new byte[] { 0xF1, 0xC1, 0xAA, (byte) 'a' };
        var text = "abc";
        using var memoryStream = new MemoryStream();
        using (var binaryWriter = new BinaryWriter(memoryStream))
        {
            binaryWriter.Write(header);

            // 再写入一些测试数据
            var textByteList = Encoding.UTF8.GetBytes(text);
            binaryWriter.Write(textByteList);
        }

        var ipcMessage = new IpcMessage("test", new IpcMessageBody(memoryStream.ToArray()));

        // 要求的负载和传入的不相同
        var result = ipcMessage.TryGetPayload(new byte[] { 0xF1, 0xC1, 0xAA, (byte) 'b' }, out var subMessage);

        // 可以成功获取到有效负载
        Assert.AreEqual(false, result);
    }

    [TestMethod("给定的 IpcMessage 存在偏移，但包含正确 byte 数组的有效负载，可以成功获取到有效负载")]
    public void GetPayload2()
    {
        var header = new byte[] { 0xF1, 0xC1, 0xAA, (byte) 'a' };
        var text = "abc";
        using var memoryStream = new MemoryStream();
        using (var binaryWriter = new BinaryWriter(memoryStream))
        {
            // 写入一个 byte 垃圾，用于后续测试存在偏移
            binaryWriter.Write((byte) 0);

            binaryWriter.Write(header);

            // 再写入一些测试数据
            var textByteList = Encoding.UTF8.GetBytes(text);
            binaryWriter.Write(textByteList);
        }
        // sizeof(header) + sizeof(text) = 4 + 3 = 7
        var ipcMessage = new IpcMessage("test", new IpcMessageBody(memoryStream.ToArray(), 1, 7));

        var result = ipcMessage.TryGetPayload(header, out var subMessage);

        // 可以成功获取到有效负载
        Assert.AreEqual(true, result, "没有成功获取到有效负载");

        var resultText = Encoding.UTF8.GetString(subMessage.Body.Buffer, subMessage.Body.Start, subMessage.Body.Length);
        Assert.AreEqual(text, resultText);
    }

    [TestMethod("给定 IpcMessage 包含正确 byte 数组的有效负载，可以成功获取到有效负载")]
    public void GetPayload3()
    {
        var header = new byte[] { 0xF1, 0xC1, 0xAA, (byte) 'a' };
        var text = "abc";
        using var memoryStream = new MemoryStream();
        using (var binaryWriter = new BinaryWriter(memoryStream))
        {
            binaryWriter.Write(header);

            // 再写入一些测试数据
            var textByteList = Encoding.UTF8.GetBytes(text);
            binaryWriter.Write(textByteList);
        }

        var ipcMessage = new IpcMessage("test", new IpcMessageBody(memoryStream.ToArray()));

        var result = ipcMessage.TryGetPayload(header, out var subMessage);

        // 可以成功获取到有效负载
        Assert.AreEqual(true, result, "没有成功获取到有效负载");

        var resultText = Encoding.UTF8.GetString(subMessage.Body.Buffer, subMessage.Body.Start, subMessage.Body.Length);
        Assert.AreEqual(text, resultText);
    }

    [TestMethod("给定的 IpcMessage 不包含正确 ulong 的有效负载，获取有效负载失败")]
    public void GetPayload4()
    {
        ulong header = 0x12312;
        var text = "abc";
        using var memoryStream = new MemoryStream();
        using (var binaryWriter = new BinaryWriter(memoryStream))
        {
            binaryWriter.Write(header);

            // 再写入一些测试数据
            var textByteList = Encoding.UTF8.GetBytes(text);
            binaryWriter.Write(textByteList);
        }

        var ipcMessage = new IpcMessage("test", new IpcMessageBody(memoryStream.ToArray()));

        // 要求的负载和传入的不相同
        var result = ipcMessage.TryGetPayload(0xFF, out var subMessage);

        // 可以成功获取到有效负载
        Assert.AreEqual(false, result);
    }

    [TestMethod("给定的 IpcMessage 存在偏移，但包含正确 ulong 的有效负载，可以成功获取到有效负载")]
    public void GetPayload5()
    {
        ulong header = 0x12312;
        var text = "abc";
        using var memoryStream = new MemoryStream();
        using (var binaryWriter = new BinaryWriter(memoryStream))
        {
            // 写入一个 byte 垃圾，用于后续测试存在偏移
            binaryWriter.Write((byte) 0);

            binaryWriter.Write(header);

            // 再写入一些测试数据
            var textByteList = Encoding.UTF8.GetBytes(text);
            binaryWriter.Write(textByteList);
        }
        // sizeof(ulong) + sizeof(text) = 8 + 3 = 11
        var ipcMessage = new IpcMessage("test", new IpcMessageBody(memoryStream.ToArray(), 1, 11));

        var result = ipcMessage.TryGetPayload(header, out var subMessage);

        // 可以成功获取到有效负载
        Assert.AreEqual(true, result, "没有成功获取到有效负载");

        var resultText = Encoding.UTF8.GetString(subMessage.Body.Buffer, subMessage.Body.Start, subMessage.Body.Length);
        Assert.AreEqual(text, resultText);
    }

    [TestMethod("给定 IpcMessage 包含正确 ulong 的有效负载，可以成功获取到有效负载")]
    public void GetPayload6()
    {
        ulong header = 0x12312;
        var text = "abc";
        using var memoryStream = new MemoryStream();
        using (var binaryWriter = new BinaryWriter(memoryStream))
        {
            binaryWriter.Write(header);

            // 再写入一些测试数据
            var textByteList = Encoding.UTF8.GetBytes(text);
            binaryWriter.Write(textByteList);
        }

        var ipcMessage = new IpcMessage("test", new IpcMessageBody(memoryStream.ToArray()));

        var result = ipcMessage.TryGetPayload(header, out var subMessage);

        // 可以成功获取到有效负载
        Assert.AreEqual(true, result, "没有成功获取到有效负载");

        var resultText = Encoding.UTF8.GetString(subMessage.Body.Buffer, subMessage.Body.Start, subMessage.Body.Length);
        Assert.AreEqual(text, resultText);
    }
}
