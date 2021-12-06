using System.IO;

using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Utils.Buffers;

namespace dotnetCampus.Ipc.Utils.IO
{
    internal class ByteListMessageStream : MemoryStream
    {
        public ByteListMessageStream(byte[] buffer, int count, ISharedArrayPool sharedArrayPool) : base(buffer, 0,
            count, false)
        {
            _sharedArrayPool = sharedArrayPool;
            Buffer = buffer;
        }

        public ByteListMessageStream(in IpcMessageContext ipcMessageContext) : this(ipcMessageContext.MessageBuffer,
            (int) ipcMessageContext.MessageLength, ipcMessageContext.SharedArrayPool)
        {
        }

        ~ByteListMessageStream()
        {
            _sharedArrayPool.Return(Buffer);
        }

        private byte[] Buffer { get; }

        private readonly ISharedArrayPool _sharedArrayPool;
    }
}
