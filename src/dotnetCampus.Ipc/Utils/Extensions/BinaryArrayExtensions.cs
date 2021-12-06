using System;
using System.Linq;

namespace dotnetCampus.Ipc.Utils.Extensions
{
    class BinaryArrayExtensions
    {
        public static byte[] Combine(params byte[][] arrays)
        {
            var ret = new byte[arrays.Sum(x => x.Length)];
            var offset = 0;
            foreach (var data in arrays)
            {
                Buffer.BlockCopy(data, 0, ret, offset, data.Length);
                offset += data.Length;
            }
            return ret;
        }
    }
}
