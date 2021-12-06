using System;
using System.IO;
using System.Threading.Tasks;
using dotnetCampus.Ipc.Utils.Buffers;

namespace dotnetCampus.Ipc.Utils.IO
{
    class AsyncBinaryReader
    {
        public AsyncBinaryReader(Stream stream, ISharedArrayPool sharedArrayPool)
        {
            _sharedArrayPool = sharedArrayPool;
            Stream = stream;
        }

        private Stream Stream { get; }
        private readonly ISharedArrayPool _sharedArrayPool;

        public Task<StreamReadResult<ushort>> ReadUInt16Async()
        {
            return ReadAsync(2, byteList => BitConverter.ToUInt16(byteList, 0));
        }

        public Task<StreamReadResult<ulong>> ReadReadUInt64Async()
        {
            return ReadAsync(sizeof(ulong), byteList => BitConverter.ToUInt64(byteList, 0));
        }

        public Task<StreamReadResult<uint>> ReadUInt32Async()
        {
            return ReadAsync(sizeof(uint), byteList => BitConverter.ToUInt32(byteList, 0));
        }

        private async Task<StreamReadResult<T>> ReadAsync<T>(int byteCount, Func<byte[], T> converter)
        {
            var readResult = await InternalReadAsync(byteCount);
            if (readResult.IsEndOfStream)
            {
                return StreamReadResult<T>.EndOfStream;
            }

            var byteList = readResult.Result;

            var result = converter(byteList);
            _sharedArrayPool.Return(byteList);
            return new StreamReadResult<T>(result);
        }

        private async Task<StreamReadResult<byte[]>> InternalReadAsync(int numBytes)
        {
            var byteList = _sharedArrayPool.Rent(numBytes);
            var bytesRead = 0;

            do
            {
                var n = await Stream.ReadAsync(byteList, bytesRead, numBytes - bytesRead).ConfigureAwait(false);
                if (n == 0)
                {
                    return StreamReadResult<byte[]>.EndOfStream;
                }

                bytesRead += n;
            } while (bytesRead < numBytes);

            return new StreamReadResult<byte[]>(byteList);
        }
    }

    readonly struct StreamReadResult<T>
    {
        public StreamReadResult(T result, bool isEndOfStream = false)
        {
            Result = result;

            IsEndOfStream = isEndOfStream;
        }

        public static StreamReadResult<T> EndOfStream =>
            new StreamReadResult<T>(result: default!, isEndOfStream: true);

        public bool IsEndOfStream { get; }

        public T Result { get; }
    }
}
