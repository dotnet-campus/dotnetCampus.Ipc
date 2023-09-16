using System.IO;

using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Utils.Buffers;

namespace dotnetCampus.Ipc.Utils.IO
{
    /// <summary>
    /// 数组列表的消息，特别定义类型，方便内存分析
    /// </summary>
    internal class ByteListMessageStream : MemoryStream
    {
        public ByteListMessageStream(byte[] buffer, int count) : base(buffer, 0,
            count, writable: false,
            // 设置 publiclyVisible 属性，防止 GetBuffer 抛出 UnauthorizedAccessException 异常
            publiclyVisible: true)
        {
        }

        public ByteListMessageStream(in IpcMessageContext ipcMessageContext) : this(ipcMessageContext.MessageBuffer,
            (int) ipcMessageContext.MessageLength)
        {
        }
    }
}
