﻿using System.Threading.Tasks;

using dotnetCampus.Ipc.Internals;
using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Context
{
    /// <summary>
    /// 回复 Ack 的任务，用于在 <see cref="AckManager"/> 收到回复的时候设置任务完成
    /// </summary>
    // todo 此类型可以准备删除
    class AckTask
    {
        /// <summary>
        /// 创建回复 Ack 的任务
        /// </summary>
        /// <param name="peerName"></param>
        /// <param name="ack"></param>
        /// <param name="task"></param>
        /// <param name="tag">用于调试的信息</param>
        public AckTask(string peerName, in Ack ack, TaskCompletionSource<bool> task, string tag)
        {
            PeerName = peerName;
            Ack = ack;
            Task = task;
            Summary = tag;
        }

        public TaskCompletionSource<bool> Task { get; }

        public string Summary { get; }

        public Ack Ack { get; }

        public string PeerName { get; }
    }
}
