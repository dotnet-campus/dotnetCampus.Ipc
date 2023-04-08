using System;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Utils.Extensions;

/// <summary>
/// 对 <see cref="IPeerMessageArgs"/> 的扩展
/// </summary>
public static class PeerMessageArgsExtension
{
    internal static bool TryGetPayload(IPeerMessageArgs args, byte[] requiredHeader, out IpcMessage subMessage)
    {
        var ipcMessage = args.Message;

        return ipcMessage.TryGetPayload(requiredHeader, out subMessage);
    }

    /// <summary>
    /// 尝试根据 <paramref name="requiredHeader"/> 获取有效负载内容。如果当前的 <see cref="args"/> 不包含 <paramref name="requiredHeader"/> 头信息，将返回 false 值。如包含，则将 <see cref="args"/> 去掉 <paramref name="requiredHeader"/> 长度之后作为 <paramref name="subMessage"/> 返回，同时返回 true 值
    /// </summary>
    /// <param name="requiredHeader"></param>
    /// <param name="subMessage"></param>
    /// <returns></returns>
    public static bool TryGetPayload(this IPeerMessageArgs args, ulong requiredHeader, out IpcMessage subMessage)
    {
        var ipcMessage = args.Message;

        return ipcMessage.TryGetPayload(requiredHeader, out subMessage);
    }

    /// <summary>
    /// 尝试根据 <paramref name="requiredHeader"/> 获取有效负载内容。如果当前的 <see cref="ipcMessage"/> 不包含 <paramref name="requiredHeader"/> 头信息，将返回 false 值。如包含，则将 <see cref="ipcMessage"/> 去掉 <paramref name="requiredHeader"/> 长度之后作为 <paramref name="subMessage"/> 返回，同时返回 true 值
    /// </summary>
    /// <param name="requiredHeader"></param>
    /// <param name="subMessage"></param>
    /// <returns></returns>
    public static bool TryGetPayload(in this IpcMessage ipcMessage, byte[] requiredHeader, out IpcMessage subMessage)
    {
        var length = requiredHeader.Length;
        if (ipcMessage.Body.Length >= length)
        {
            // 反正 requiredHeader 没多长，遍历即可
            for (int i = 0; i < length; i++)
            {
                var start = ipcMessage.Body.Start + i;
                if (ipcMessage.Body.Buffer[start] != requiredHeader[i])
                {
                    subMessage = default;
                    return false;
                }
            }

            subMessage = ipcMessage.Skip(length);
            return true;
        }

        subMessage = default;
        return false;
    }

    /// <summary>
    /// 尝试根据 <paramref name="requiredHeader"/> 获取有效负载内容。如果当前的 <see cref="ipcMessage"/> 不包含 <paramref name="requiredHeader"/> 头信息，将返回 false 值。如包含，则将 <see cref="ipcMessage"/> 去掉 <paramref name="requiredHeader"/> 长度之后作为 <paramref name="subMessage"/> 返回，同时返回 true 值
    /// </summary>
    /// <param name="ipcMessage"></param>
    /// <param name="requiredHeader"></param>
    /// <param name="subMessage"></param>
    /// <returns></returns>
    public static bool TryGetPayload(in this IpcMessage ipcMessage, ulong requiredHeader, out IpcMessage subMessage)
    {
        const int length = sizeof(ulong);
        if (ipcMessage.Body.Length >= length)
        {
            var header = BitConverter.ToUInt64(ipcMessage.Body.Buffer, ipcMessage.Body.Start);
            if (header == requiredHeader)
            {
                subMessage = ipcMessage.Skip(length);
                return true;
            }
        }

        subMessage = default;
        return false;
    }
}
