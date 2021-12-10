using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Diagnostics;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Utils.Buffers;
using dotnetCampus.Ipc.Utils.Extensions;
using dotnetCampus.Ipc.Utils.IO;
using dotnetCampus.Ipc.Utils.Logging;

namespace dotnetCampus.Ipc.Internals
{
    /// <summary>
    /// 消息的封包和解包代码，用于将传入的内容包装为 Ipc 通讯使用的二进制内容，或将 Ipc 通讯使用的二进制内容读取为业务端使用的内容
    /// </summary>
    internal static class IpcMessageConverter
    {
        public static async Task WriteAsync(Stream stream, byte[] messageHeader, Ack ack,
            IpcBufferMessageContext context)
        {
            // 准备变量。
            var commandType = context.IpcMessageCommandType;
            VerifyMessageLength(context.Length);

            // 发送消息头。
            var binaryWriter = await WriteHeaderAsync(stream, messageHeader, ack, commandType).ConfigureAwait(false);

            // 发送消息长度。
            await binaryWriter.WriteAsync(context.Length).ConfigureAwait(false);

            // 发送消息体。
            foreach (var ipcBufferMessage in context.IpcBufferMessageList)
            {
                await stream.WriteAsync(ipcBufferMessage.Buffer, ipcBufferMessage.Start, ipcBufferMessage.Length).ConfigureAwait(false);
            }
        }

        public static async Task WriteAsync(Stream stream, byte[] messageHeader, Ack ack,
            IpcMessageCommandType ipcMessageCommandType, byte[] buffer, int offset, int count, string? tag = null)
        {
            VerifyMessageLength(count, tag);

            var binaryWriter = await WriteHeaderAsync(stream, messageHeader, ack, ipcMessageCommandType);

            await binaryWriter.WriteAsync(count);
            await stream.WriteAsync(buffer, offset, count);
        }

        private static void VerifyMessageLength(int messageLength, string? tag = null)
        {
            if (messageLength > IpcConfiguration.MaxMessageLength)
            {
                throw new ArgumentException($"Message Length too long  MessageLength={messageLength} MaxMessageLength={IpcConfiguration.MaxMessageLength}. {DebugContext.OverMaxMessageLength}")
                {
                    Data =
                    {
                        { "Message Length", messageLength },
                        { "Tag", tag ?? string.Empty }
                    }
                };
            }
        }

        public static async Task<AsyncBinaryWriter> WriteHeaderAsync(Stream stream, byte[] messageHeader, Ack ack,
            IpcMessageCommandType ipcMessageCommandType)
        {
            /*
             * UInt16 Message Header Length 消息头的长度
             * byte[] Message Header        消息头的内容
             * UInt32 Version        当前IPC服务的版本
             * UInt64 Ack            用于给对方确认收到消息使用
             * UInt32 Empty          给以后版本使用的值
             * Int16 Command Type   命令类型，业务端的值将会是 0 而框架层采用其他值
             * UInt32 Content Length 这条消息的内容长度
             * byte[] Content        实际的内容
             */

            // 当前版本默认是 1 版本，这个值用来后续如果有协议上的更改时，兼容旧版本使用
            // - 版本是 0 的版本，每条消息都有回复 ack 的值
            const uint version = 1;

            var asyncBinaryWriter = new AsyncBinaryWriter(stream);
            var messageHeaderLength = (ushort) messageHeader.Length;
            await asyncBinaryWriter.WriteAsync(messageHeaderLength).ConfigureAwait(false);

            await stream.WriteAsync(messageHeader).ConfigureAwait(false);
            // UInt32 Version
            await asyncBinaryWriter.WriteAsync(version).ConfigureAwait(false);
            // UInt64 Ack
            await asyncBinaryWriter.WriteAsync(ack.Value).ConfigureAwait(false);
            // UInt32 Empty
            await asyncBinaryWriter.WriteAsync(uint.MinValue).ConfigureAwait(false);
            // Int16 Command Type   命令类型，业务端的值将会是 0 而框架层采用其他值
            var commandType = (ushort) ipcMessageCommandType;
            await asyncBinaryWriter.WriteAsync(commandType).ConfigureAwait(false);

            return asyncBinaryWriter;
        }

        public static async Task<StreamReadResult<IpcMessageResult>> ReadAsync(Stream stream,
            byte[] messageHeader, ISharedArrayPool sharedArrayPool)
        {
            /*
             * UInt16 Message Header Length 消息头的长度
             * byte[] Message Header        消息头的内容
             * UInt32 Version        当前IPC服务的版本
             * UInt64 Ack            用于给对方确认收到消息使用
             * UInt32 Empty          给以后版本使用的值
             * Int16 Command Type   命令类型，业务端的值将会是 0 而框架层采用其他值
             * UInt32 Content Length 这条消息的内容长度
             * byte[] Content        实际的内容
             */

            var headerResult = await GetHeader(stream, messageHeader, sharedArrayPool);
            if (headerResult.IsEndOfStream)
            {
                return StreamReadResult<IpcMessageResult>.EndOfStream;
            }

            if (!headerResult.Result)
            {
                // 消息不对，忽略
                return new StreamReadResult<IpcMessageResult>(new IpcMessageResult("Message Header no match"));
            }

            var binaryReader = new AsyncBinaryReader(stream, sharedArrayPool);
            // UInt32 Version        当前IPC服务的版本
            var versionResult = await binaryReader.ReadUInt32Async();
            if (versionResult.IsEndOfStream)
            {
                return StreamReadResult<IpcMessageResult>.EndOfStream;
            }
            var version = versionResult.Result;
            Debug.Assert(version == 1);
            if (version == 0)
            {
                // 这是上个版本的，但是不兼容了
                var ipcMessageResult = new IpcMessageResult("收到版本为 0 的旧版本消息，但是不兼容此版本");
                return new StreamReadResult<IpcMessageResult>(ipcMessageResult);
            }

            // UInt64 Ack            用于给对方确认收到消息使用
            var ackResult = await binaryReader.ReadReadUInt64Async();
            if (ackResult.IsEndOfStream)
            {
                return StreamReadResult<IpcMessageResult>.EndOfStream;
            }
            var ack = ackResult.Result;

            // UInt32 Empty          给以后版本使用的值
            var empty = await binaryReader.ReadUInt32Async();
            Debug.Assert(empty.Result == 0);

            // Int16 Command Type   命令类型，业务端的值将会是大于 0 而框架层采用其他值
            var commandTypeResult = await binaryReader.ReadUInt16Async();
            if (commandTypeResult.IsEndOfStream)
            {
                return StreamReadResult<IpcMessageResult>.EndOfStream;
            }
            var commandType = (IpcMessageCommandType) commandTypeResult.Result;

            // UInt32 Content Length 这条消息的内容长度
            var messageLengthResult = await binaryReader.ReadUInt32Async();
            if (messageLengthResult.IsEndOfStream)
            {
                return StreamReadResult<IpcMessageResult>.EndOfStream;
            }

            var messageLength = messageLengthResult.Result;
            if (messageLength > IpcConfiguration.MaxMessageLength)
            {
                // 太长了
                var ipcMessageResult = new IpcMessageResult(
                    $"Message Length too long  MessageLength={messageLength} MaxMessageLength={IpcConfiguration.MaxMessageLength}. {DebugContext.OverMaxMessageLength}");
                return new StreamReadResult<IpcMessageResult>(ipcMessageResult);
            }

            var messageBuffer = sharedArrayPool.Rent((int) messageLength);
            // byte[] Content        实际的内容
            var readCount = await ReadBufferAsync(stream, messageBuffer, (int) messageLength);

            Debug.Assert(readCount == messageLength);

            var ipcMessageContext = new IpcMessageContext(ack, messageBuffer, messageLength, sharedArrayPool);
            return new StreamReadResult<IpcMessageResult>(new IpcMessageResult(success: true, ipcMessageContext, commandType));
        }

        private static async Task<int> ReadBufferAsync(Stream stream, byte[] messageBuffer, int messageLength)
        {
            var readCount = 0;

            do
            {
                var n = await stream.ReadAsync(messageBuffer, readCount, messageLength - readCount);
                readCount += n;
            } while (readCount < messageLength);

            return readCount;
        }

        private static async Task<StreamReadResult<bool>> GetHeader(Stream stream, byte[] messageHeader, ISharedArrayPool sharedArrayPool)
        {
            var binaryReader = new AsyncBinaryReader(stream, sharedArrayPool);
            var messageHeaderLengthResult = await binaryReader.ReadUInt16Async();
            if (messageHeaderLengthResult.IsEndOfStream)
            {
                return StreamReadResult<bool>.EndOfStream;
            }

            var messageHeaderLength = messageHeaderLengthResult.Result;

            //Debug.Assert(messageHeaderLength == messageHeader.Length);
            if (messageHeaderLength != messageHeader.Length)
            {
                // 消息不对，忽略
                return new StreamReadResult<bool>(false);
            }

            var messageHeaderBuffer = sharedArrayPool.Rent(messageHeader.Length);

            try
            {
                var readCount = await ReadBufferAsync(stream, messageHeaderBuffer, messageHeader.Length);
                Debug.Assert(readCount == messageHeader.Length);
                if (ByteListExtensions.Equals(messageHeaderBuffer, messageHeader, readCount))
                {
                    // 读对了
                    return new StreamReadResult<bool>(true);
                }
                else
                {
                    // 发过来的消息是出错的
                    return new StreamReadResult<bool>(false);
                }
            }
            finally
            {
                sharedArrayPool.Return(messageHeaderBuffer);
            }
        }
    }
}
