using System.Threading;

namespace dotnetCampus.Ipc.Utils.Buffers
{
#if NETCOREAPP
    using System.Buffers;
    /// <summary>
    ///     共享数组内存，底层使用 ArrayPool 实现
    /// </summary>
    public class SharedArrayPool : ISharedArrayPool
    {
        /// <summary>
        ///     创建共享数组
        /// </summary>
        /// <param name="arrayPool"></param>
        public SharedArrayPool(ArrayPool<byte>? arrayPool = null)
        {
            ArrayPool = arrayPool ?? ArrayPool<byte>.Shared;
        }

        /// <summary>
        ///     使用的数组池
        /// </summary>
        public ArrayPool<byte> ArrayPool { get; }

        /// <inheritdoc />
        public byte[] Rent(int minLength)
        {
            return ArrayPool.Rent(minLength);
        }

        /// <inheritdoc />
        public void Return(byte[] array)
        {
            ArrayPool.Return(array);
        }
    }
#else
    /// <summary>
    /// 共享数组内存
    /// </summary>
    /// 低配版
    public class SharedArrayPool : ISharedArrayPool
    {
        /// <inheritdoc />
        public byte[] Rent(int minLength)
        {
            var cacheByteList = _cacheByteList.Value;

            if (cacheByteList != null && cacheByteList.Length >= minLength)
            {
                _cacheByteList.Value = null;
                return cacheByteList;
            }

            return new byte[minLength];
        }

        /// <inheritdoc />
        public void Return(byte[] array)
        {
            var cacheByteList = _cacheByteList.Value;
            if (cacheByteList == null || cacheByteList.Length < array.Length)
            {
                _cacheByteList.Value = cacheByteList;
            }
        }

        private readonly ThreadLocal<byte[]?> _cacheByteList = new ThreadLocal<byte[]?>();
    }
#endif
}
