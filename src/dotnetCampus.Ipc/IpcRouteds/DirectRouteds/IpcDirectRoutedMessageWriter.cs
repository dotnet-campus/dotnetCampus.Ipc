using System.IO;

namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

static class IpcDirectRoutedMessageWriter
{
    /// <summary>
    /// 写入消息头
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="businessMessageHeader"></param>
    /// <param name="routedPath"></param>
    public static void WriteHeader(BinaryWriter writer, ulong businessMessageHeader, string routedPath)
    {
        writer.Write(businessMessageHeader);
        writer.Write(routedPath);
    }
}
