using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc
{
    /// <summary>
    /// 提供消息的写入方法
    /// </summary>
    public class IpcMessageWriter : IRawMessageWriter
    {
        /// <summary>
        /// 创建提供消息的写入方法
        /// </summary>
        /// <param name="messageWriter">实际用来写入的方法</param>
        public IpcMessageWriter(IRawMessageWriter messageWriter)
        {
            RawWriter = messageWriter;
        }

        private IRawMessageWriter RawWriter { get; }

        /// <inheritdoc />
        public Task WriteMessageAsync(byte[] buffer, int offset, int count, [CallerMemberName] string tag = "")
        {
            return RawWriter.WriteMessageAsync(buffer, offset, count, tag);
        }

        /// <summary>
        /// 向对方发送消息
        /// </summary>
        /// <param name="message">字符串消息，将会被使用Utf-8编码转换然后发送</param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Task WriteMessageAsync(string message, string? tag = null)
        {
            tag ??= message;
            var messageBuffer = Encoding.UTF8.GetBytes(message);
            return WriteMessageAsync(messageBuffer, 0, messageBuffer.Length, tag);
        }

        /// <summary>
        /// 向对方发送消息
        /// </summary>
        /// <param name="message">字符串消息，将会被使用Utf-8编码转换然后发送</param>
        /// <returns></returns>
        public Task WriteMessageAsync(IpcMessage message)
        {
            return WriteMessageAsync(message.Body.Buffer, message.Body.Start, message.Body.Length, message.Tag);
        }
    }
}
