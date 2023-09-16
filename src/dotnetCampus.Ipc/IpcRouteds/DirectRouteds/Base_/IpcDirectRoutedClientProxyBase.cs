using System.IO;
using System.Text;
using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

/// <summary>
/// 直接路由的客户端基类
/// </summary>
public abstract class IpcDirectRoutedClientProxyBase
{
    /// <summary>
    /// 业务头
    /// </summary>
    protected abstract ulong BusinessHeader { get; }

    /// <summary>
    /// 写入消息头，包括路由地址和业务头
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="routedPath"></param>
    protected void WriteHeader(MemoryStream stream, string routedPath)
    {
        using var binaryWriter = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
        IpcDirectRoutedMessageWriter.WriteHeader(binaryWriter, BusinessHeader, routedPath);
    }

    /// <summary>
    /// 从 MemoryStream 转换为 <see cref="IpcMessage"/> 对象。将会取出 MemoryStream 的 Buffer 封装为 <see cref="IpcMessage"/> 对象
    /// </summary>
    /// <param name="stream">要求是自己可控创建的 MemoryStream 对象，不能传入从池里获取的对象，且在 ToIpcMessage 之后再没有修改</param>
    /// <param name="tag"></param>
    /// <returns></returns>
    protected IpcMessage ToIpcMessage(MemoryStream stream, string tag = "")
    {
        var buffer = stream.GetBuffer();
        var length = (int) stream.Position;

        return new IpcMessage(tag, new IpcMessageBody(buffer, start: 0, length));
    }
}
