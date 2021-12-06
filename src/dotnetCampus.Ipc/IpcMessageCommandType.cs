using System;

namespace dotnetCampus.Ipc
{
    /// <summary>
    /// 用于作为命令类型，用于框架的命令和业务的命令
    /// </summary>
    [Flags]
    internal enum IpcMessageCommandType : short
    {
        /// <summary>
        /// 向对方服务器注册
        /// </summary>
        PeerRegister = -1,

        /*
        /// <summary>
        /// 发送回复信息
        /// </summary>
        SendAck = 0B0010,

        /// <summary>
        /// 发送回复信息，同时向对方服务器注册
        /// </summary>
        SendAckAndRegisterToPeer = PeerRegister | SendAck,
        */

        /// <summary>
        /// 业务层的消息
        /// </summary>
        /// 所有大于 0 的都是业务层消息
        Business = 1,

        /// <summary>
        /// 请求消息，业务层消息。
        /// </summary>
        RequestMessage = (1 << 1) | Business,

        /// <summary>
        /// 响应消息，业务层消息。
        /// </summary>
        ResponseMessage = (1 << 2) | Business,

        /// <summary>
        /// 请求的细分类型，IPC 框架无法识别和处理此消息体。
        /// </summary>
        RawRequestMessage = (1 << 3) | RequestMessage,

        /// <summary>
        /// 响应的细分类型，IPC 框架无法识别和处理此消息体。
        /// </summary>
        RawResponseMessage = (1 << 3) | ResponseMessage,

        /// <summary>
        /// 请求的细分类型，IPC 框架知道此消息体是可被处理的字符串。
        /// </summary>
        StringRequestMessage = (1 << 4) | RequestMessage,

        /// <summary>
        /// 响应的细分类型，IPC 框架知道此消息体是可被处理的字符串。
        /// </summary>
        StringResponseMessage = (1 << 4) | ResponseMessage,

        /// <summary>
        /// 请求的细分类型，IPC 框架知道此消息体是可被处理的 .NET 对象。
        /// </summary>
        ObjectRequestMessage = (1 << 5) | RequestMessage,

        /// <summary>
        /// 响应的细分类型，IPC 框架知道此消息体是可被处理的 .NET 对象。
        /// </summary>
        ObjectResponseMessage = (1 << 5) | ResponseMessage,

        /// <summary>
        /// 其他消息。
        /// </summary>
        Unknown = short.MaxValue,
    }
}
