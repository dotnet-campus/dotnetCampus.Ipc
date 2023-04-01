using System;
using System.Collections.Generic;
using System.Text;

namespace dotnetCampus.Ipc.Context;

/// <summary>
/// 已知的消息头
/// </summary>
/// 现在已有三套通讯方法：
/// - RemoteObject
/// - MVC
/// - Raw
/// 其中 Raw 不加头，完全都裸通讯方式
public enum KnownMessageHeaders : ulong
{
    /// <summary>
    /// 发送的消息是 RemoteObject 通讯的消息
    /// </summary>
    RemoteObjectMessageHeader
        // 消息头是 R(e)m(ote)O(b)j(ect) 的 RmOj 几个字符组成的 long 头
        = 0x526D4F6A,

    /// <summary>
    /// 发送的是 Json 的直接路由消息
    /// </summary>
    JsonIpcDirectRoutedMessageHeader
        // JsonDrRt
        = 0x745272446E6F734A,
}
