using System.IO;

namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

static class IpcDirectRoutedMessageWriter
{
    public static void WriteHeader(BinaryWriter writer, ulong businessMessageHeader, string routedPath)
    {
        writer.Write(businessMessageHeader);
        writer.Write(routedPath);
    }
}
