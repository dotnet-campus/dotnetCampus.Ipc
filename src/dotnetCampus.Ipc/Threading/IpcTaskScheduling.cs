using System;

namespace dotnetCampus.Ipc.Threading
{
    /// <summary>
    /// 决定 IPC 消息抵达时，以何种调度方式通知给业务代码。
    /// </summary>
    public enum IpcTaskScheduling
    {
        /// <summary>
        /// 所有的 IPC 共享同一个调度线程池。以大体上按顺序（但不保证）的并发方式通知。
        /// </summary>
        GlobalConcurrent = 0,

        /// <summary>
        /// 按顺序依次通知，并且与其他 IPC 实例互不影响。
        /// </summary>
        /// <remarks>
        /// <para>使用前请避免在业务中创造死锁条件：</para>
        /// <list type="bullet">
        /// <item>A 向 B 发送消息，但 B 为了回 A 的这条消息，需要先向 A 请求某种值。</item>
        /// </list>
        /// <para>在这种情况下，按顺序的发送方式将导致死锁。</para>
        /// </remarks>
        LocalOneByOne = 1,
    }
}
