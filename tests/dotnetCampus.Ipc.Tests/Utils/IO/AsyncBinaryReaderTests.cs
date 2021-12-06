using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using dotnetCampus.Ipc.Utils.Buffers;
using dotnetCampus.Ipc.Utils.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Extensions.Contracts;

namespace dotnetCampus.Ipc.Utils.IO.Tests
{
    [TestClass()]
    public class AsyncBinaryReaderTests
    {
        [ContractTestCase]
        public void AsyncBinaryReaderTest()
        {
            "给定的共享数组远远超过所需长度，可以成功读取正确的数值".Test(async () =>
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
                Assert.AreEqual(n1, r1);
                var r2 = await asyncBinaryReader.ReadReadUInt64Async();
                Assert.AreEqual(n2, r2);
                var r3 = await asyncBinaryReader.ReadUInt32Async();
                Assert.AreEqual(n3, r3);
            });
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
}
