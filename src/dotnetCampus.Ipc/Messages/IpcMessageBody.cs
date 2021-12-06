using System;
using System.Diagnostics;
using System.IO;

namespace dotnetCampus.Ipc.Messages
{
    /// <summary>
    /// 表示一段 Ipc 消息内容
    /// </summary>
    public readonly struct IpcMessageBody
    {
        /// <summary>
        /// 创建一段 Ipc 消息内容
        /// </summary>
        /// <param name="buffer"></param>
        [DebuggerStepThrough]
        public IpcMessageBody(byte[] buffer)
        {
            Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            Start = 0;
            Length = buffer.Length;
        }

        /// <summary>
        /// 创建一段 Ipc 消息内容
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        [DebuggerStepThrough]
        public IpcMessageBody(byte[] buffer, int start, int length)
        {
            if (start < 0 || start > buffer.Length - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(start), "消息体长度必须大于 0。如果此消息来自发送方，请检查是否发送了消息体长度为 0 的消息。");
            }

            if (length < 0 || start + length > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            Start = start;
            Length = length;
        }

        /// <summary>
        /// 缓存数据
        /// </summary>
        public byte[] Buffer { get; }

        /// <summary>
        /// 缓存数据的起始点
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// 数据长度
        /// </summary>
        public int Length { get; }
    }

    /// <summary>
    /// 给 <see cref="IpcMessageBody"/> 的扩展
    /// </summary>
    public static class IpcMessageBodyExtensions
    {
        /// <summary>
        /// 转换为 <see cref="MemoryStream"/> 对象
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static MemoryStream ToMemoryStream(this IpcMessageBody message) =>
            new MemoryStream(message.Buffer, message.Start, message.Length, false);
    }
}
