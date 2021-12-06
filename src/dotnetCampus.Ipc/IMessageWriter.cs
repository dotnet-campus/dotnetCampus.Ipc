using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace dotnetCampus.Ipc
{
    /// <summary>
    /// 用于表示发送消息
    /// </summary>
    public interface IRawMessageWriter
    {
        /// <summary>
        /// 向服务端发送消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="tag">这一次写入的是什么内容，用于调试</param>
        /// <returns></returns>
        Task WriteMessageAsync(byte[] data, int offset, int length, [CallerMemberName] string tag = "");
    }
}
