using System;
using System.Collections.Generic;

using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Context
{
    /// <summary>
    /// 包含多条信息的上下文
    /// </summary>
    /// 不开放，因为我不认为上层能用对
    readonly struct IpcBufferMessageContext
    {
        /// <summary>
        /// 创建包含多条信息的上下文
        /// </summary>
        /// <param name="summary">表示写入的是什么内容，用于调试</param>
        /// <param name="ipcMessageCommandType">命令类型，用于分开框架内的消息和业务的</param>
        /// <param name="ipcBufferMessageList"></param>
        public IpcBufferMessageContext(string summary, IpcMessageCommandType ipcMessageCommandType, params IpcMessageBody[] ipcBufferMessageList)
        {
            Tag = summary;
            IpcMessageCommandType = ipcMessageCommandType;
            IpcBufferMessageList = ipcBufferMessageList;
        }

        /// <summary>
        /// 命令类型，用于分开框架内的消息和业务的
        /// </summary>
        public IpcMessageCommandType IpcMessageCommandType { get; }

        public IpcMessageBody[] IpcBufferMessageList { get; }

        /// <summary>
        /// 表示内容是什么用于调试
        /// </summary>
        public string Tag { get; }

        public int Length
        {
            get
            {
                var length = 0;
                foreach (var ipcBufferMessage in IpcBufferMessageList)
                {
                    length += ipcBufferMessage.Length;
                }

                return length;
            }
        }

        /// <summary>
        /// 和其他的合并然后创建新的
        /// </summary>
        /// <param name="ipcMessageCommandType"></param>
        /// <param name="mergeBefore">将加入的内容合并到新的消息前面，为 true 合并到前面，否则合并到后面</param>
        /// <param name="ipcBufferMessageList"></param>
        /// <returns></returns>
        public IpcBufferMessageContext BuildWithCombine(IpcMessageCommandType ipcMessageCommandType, bool mergeBefore, params IpcMessageBody[] ipcBufferMessageList)
            => BuildWithCombine(Tag, ipcMessageCommandType, mergeBefore, ipcBufferMessageList);

        /// <summary>
        /// 和其他的合并然后创建新的
        /// </summary>
        /// <param name="summary">表示写入的是什么内容，用于调试</param>
        /// <param name="ipcMessageCommandType"></param>
        /// <param name="mergeBefore">将加入的内容合并到新的消息前面，为 true 合并到前面，否则合并到后面</param>
        /// <param name="ipcBufferMessageList"></param>
        /// <returns></returns>
        public IpcBufferMessageContext BuildWithCombine(string summary, IpcMessageCommandType ipcMessageCommandType, bool mergeBefore, params IpcMessageBody[] ipcBufferMessageList)
        {
            var newIpcBufferMessageList = new List<IpcMessageBody>(ipcBufferMessageList.Length + IpcBufferMessageList.Length);
            if (mergeBefore)
            {
                newIpcBufferMessageList.AddRange(ipcBufferMessageList);
                newIpcBufferMessageList.AddRange(IpcBufferMessageList);
            }
            else
            {
                newIpcBufferMessageList.AddRange(IpcBufferMessageList);
                newIpcBufferMessageList.AddRange(ipcBufferMessageList);
            }

            return new IpcBufferMessageContext(summary, ipcMessageCommandType, newIpcBufferMessageList.ToArray());
        }
    }
}
