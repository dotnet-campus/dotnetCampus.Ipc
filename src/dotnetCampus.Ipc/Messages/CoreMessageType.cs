using System;

namespace dotnetCampus.Ipc.Messages
{
    /// <summary>
    /// 在 IPC 框架内部用来标识消息的类型。
    /// 此枚举是内部的，不要求每一套 IPC 实现都完全实现这里的所有类型的消息，因此这里可以提供目前能识别的所有类型消息的完全集合。
    /// </summary>
    [Flags]
    internal enum CoreMessageType
    {
        /// <summary>
        /// 框架内部必须处理的消息或回应。
        /// </summary>
        NotMessageBody = 0,

        /// <summary>
        /// 无特殊标识的消息。IPC 框架无法准确得知此消息的具体内容。
        /// </summary>
        Raw = 1 << 0,

        /// <summary>
        /// 标记为字符串的消息。IPC 框架知道消息体是一个字符串。
        /// </summary>
        String = 1 << 1,

        /// <summary>
        /// 标记为 .NET 对象的消息。IPC 框架知道消息体是用 JSON 序列化过的 .NET 对象。
        /// </summary>
        JsonObject = 1 << 2,
    }
}
