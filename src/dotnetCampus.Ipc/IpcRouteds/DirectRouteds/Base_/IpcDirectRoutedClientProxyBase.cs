using System.IO;
using System.Text;
using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

public abstract class IpcDirectRoutedClientProxyBase
{
    /// <summary>
    /// 业务头
    /// </summary>
    protected abstract ulong BusinessHeader { get; }

    protected void WriteHeader(MemoryStream stream, string routedPath)
    {
        using var binaryWriter = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
        IpcDirectRoutedMessageWriter.WriteHeader(binaryWriter, BusinessHeader, routedPath);
    }

    protected IpcMessage ToIpcMessage(MemoryStream stream, string tag = "")
    {
        var buffer = stream.GetBuffer();
        var length = (int) stream.Position;

        return new IpcMessage(tag, new IpcMessageBody(buffer, start: 0, length));
    }
}
