using System;

using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Utils.Extensions;

namespace dotnetCampus.Ipc.Utils.Extensions
{
    static class IpcBufferMessageExtensions
    {
#if NETCOREAPP3_1_OR_GREATER
        public static Span<byte> AsSpan(this in IpcMessageBody message)
        {
            return message.Buffer.AsSpan(message.Start, message.Length);
        }
#endif
    }
}
