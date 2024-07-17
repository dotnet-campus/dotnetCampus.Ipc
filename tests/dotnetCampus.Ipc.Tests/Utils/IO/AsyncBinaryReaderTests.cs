using dotnetCampus.Ipc.Utils.Buffers;
using dotnetCampus.Ipc.Utils.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dotnetCampus.Ipc.Tests.Utils.IO;

[TestClass]
public class AsyncBinaryReaderTests
{
    [TestMethod("给定的共享数组远远超过所需长度，可以成功读取正确的数值")]
    public async Task AsyncBinaryReaderTest()
    {
        var memoryStream = new MemoryStream();
        var streamWriter = new BinaryWriter(memoryStream);
        ushort n1 = 15;
        streamWriter.Write(n1);
        ulong n2 = 65521;
        streamWriter.Write(n2);
        uint n3 = 0;
        streamWriter.Write(n3);
        streamWriter.Flush();
        memoryStream.Position = 0;

        var asyncBinaryReader = new AsyncBinaryReader(memoryStream, new FakeSharedArrayPool());
        var r1 = await asyncBinaryReader.ReadUInt16Async();
        Assert.AreEqual(n1, r1.Result);
        var r2 = await asyncBinaryReader.ReadReadUInt64Async();
        Assert.AreEqual(n2, r2.Result);
        var r3 = await asyncBinaryReader.ReadUInt32Async();
        Assert.AreEqual(n3, r3.Result);
    }

    class FakeSharedArrayPool : ISharedArrayPool
    {
        public byte[] Rent(int minLength)
        {
            return new byte[1024];
        }

        public void Return(byte[] array)
        {
        }
    }
}
