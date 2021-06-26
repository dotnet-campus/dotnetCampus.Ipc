using System.IO;
using System.Threading.Tasks;

namespace dotnetCampus.Ipc.PipeCore.Utils
{
#if NETFRAMEWORK
    static class StreamExtensions
    {
        public static Task WriteAsync(this Stream stream, byte[] data)
        {
            return stream.WriteAsync(data, 0, data.Length);
        }
    }
#endif
}
