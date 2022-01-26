using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Exceptions;
using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Pipes
{
    /// <summary>
    /// 请求管理
    /// <para/>
    /// 这是用来期待发送一条消息之后，在对方的业务层能有回复的消息 <para/>
    /// 这是服务器-客户端模型 <para/>
    /// 客户端向服务器发起请求，服务器端业务层处理之后返回响应信息 <para/>
    /// 通过调用 <see cref="CreateRequestMessage"/> 方法创建出请求消息 <para/>
    /// 然后将此消息的 <see cref="IpcClientRequestMessage.IpcBufferMessageContext"/> 通过现有 <see cref="PeerProxy"/> 发送到服务器端。同时客户端可以使用 <see cref="IpcClientRequestMessage.Task"/> 进行等待 <para/>
    /// 服务器端接收到 <see cref="IpcClientRequestMessage"/> 的内容，将会在 <see cref="OnIpcClientRequestReceived"/> 事件触发，这个事件将会带上 <see cref="IpcClientRequestArgs"/> 参数 <para/>
    /// 在服务器端处理完成之后，底层的方法是通过调用 <see cref="IpcMessageResponseManager.CreateResponseMessage"/> 方法创建响应消息，通过 <see cref="PeerProxy"/> 发送给客户端 <para/>
    /// 客户端收到了服务器端的响应信息，将会释放 <see cref="IpcClientRequestMessage.Task"/> 任务，客户端从 <see cref="IpcClientRequestMessage.Task"/> 可以拿到服务器端的返回值
    /// </summary>
    class IpcMessageRequestManager : IpcMessageManagerBase
    {
        /// <summary>
        /// 创建请求管理
        /// </summary>
        /// <param name="ipcContext"></param>
        public IpcMessageRequestManager(IpcContext ipcContext)
        {
            IpcContext = ipcContext;
        }

        internal IpcContext IpcContext { get; }

        /// <summary>
        /// 等待响应的数量
        /// </summary>
        public int WaitingResponseCount
        {
            get
            {
                lock (Locker)
                {
                    return TaskList.Count;
                }
            }
        }

        public IpcClientRequestMessage CreateRequestMessage(IpcMessage request)
        {
            ulong currentMessageId;
            var task = new TaskCompletionSource<IpcMessageBody>();
            lock (Locker)
            {
                currentMessageId = CurrentMessageId;
                // 在超过 ulong.MaxValue 之后，将会是 0 这个值
                CurrentMessageId++;

                TaskList[currentMessageId] = task;
            }

            var requestMessage = CreateRequestMessageInner(request, currentMessageId);
            return new IpcClientRequestMessage(requestMessage, task.Task, new IpcClientRequestMessageId(currentMessageId));
        }

        private Dictionary<ulong, TaskCompletionSource<IpcMessageBody>> TaskList { get; } =
            new Dictionary<ulong, TaskCompletionSource<IpcMessageBody>>();

        /// <summary>
        /// 收到消息，包括收到别人的请求消息，和收到别人的响应消息
        /// </summary>
        /// <param name="args"></param>
        public void OnReceiveMessage(PeerStreamMessageArgs args)
        {
            HandleResponse(args);
            if (args.Handle)
            {
                return;
            }

            HandleRequest(args);
        }

        /// <summary>
        /// 断开连接之后的炸掉所有的请求任务
        /// </summary>
        /// 重发？其实会丢失上下文，因此不合适
        /// 如某个业务需要连续发送三条请求消息才能实现
        /// 但是前面两条消息成功，在发送第三条请求时，对方进程退出
        /// 如果让第三条请求重新发送，将会让新的重连的对方收到一条非预期的消息
        ///
        /// 然而如果不重发的话，也许如上面例子的三条消息差距很大，前两条消息发送之后
        /// 对方进程挂了，等待一会，才发送第三条消息
        /// 这也是坑
        ///
        /// 然而此时也触发了断开的事件，也许此时的业务端可以处理
        public void BreakAllRequestTaskByIpcBroken()
        {
            List<TaskCompletionSource<IpcMessageBody>> taskList;
            lock (Locker)
            {
                taskList = TaskList.Select(temp => temp.Value).ToList();
                TaskList.Clear();
            }

            foreach (var taskCompletionSource in taskList)
            {
                var ipcPeerConnectionBrokenException = new IpcPeerConnectionBrokenException();
                if (taskCompletionSource.TrySetException(ipcPeerConnectionBrokenException))
                {

                }
                else
                {
                    // 难道在断开的时候，刚好收到消息了？
                }
            }
        }

        private void HandleRequest(PeerStreamMessageArgs args)
        {
            if (!args.MessageCommandType.HasFlag(IpcMessageCommandType.RequestMessage))
            {
                // 如果没有请求标识，那么返回。
                return;
            }

            var message = args.MessageStream;
            if (message.Length < RequestMessageHeader.Length + sizeof(ulong))
            {
                return;
            }

            if (CheckRequestHeader(message))
            {
                // 标记在这一级消费
                args.SetHandle(message: nameof(HandleRequest));

                var binaryReader = new BinaryReader(message);
                var messageId = binaryReader.ReadUInt64();
                var requestMessageLength = binaryReader.ReadInt32();

                var currentPosition = message.Position;
                try
                {
                    var requestMessageByteList = binaryReader.ReadBytes(requestMessageLength);
                    var ipcClientRequestArgs =
                        new IpcClientRequestArgs(new IpcClientRequestMessageId(messageId),
                            new IpcMessageBody(requestMessageByteList),
                            args.MessageCommandType);
                    OnIpcClientRequestReceived?.Invoke(this, ipcClientRequestArgs);
                }
                finally
                {
                    message.Position = currentPosition;
                }
            }
        }

        public event EventHandler<IpcClientRequestArgs>? OnIpcClientRequestReceived;

        private void HandleResponse(PeerStreamMessageArgs args)
        {
            if (!args.MessageCommandType.HasFlag(IpcMessageCommandType.ResponseMessage))
            {
                // 如果没有命令标识，那么返回
                return;
            }

            var message = args.MessageStream;

            if (message.Length < ResponseMessageHeader.Length + sizeof(ulong))
            {
                return;
            }

            if (CheckResponseHeader(message))
            {
                // 标记在这一级消费
                args.SetHandle(message: nameof(HandleResponse));

                var binaryReader = new BinaryReader(message);
                var messageId = binaryReader.ReadUInt64();
                TaskCompletionSource<IpcMessageBody>? task = null;
                lock (Locker)
                {
                    // 下面这句代码在 .NET 45 不存在，因此不能使用。换成两次调用，反正性能差不多
                    //TaskList.Remove(messageId, out task);
                    if (TaskList.TryGetValue(messageId, out task))
                    {
                        TaskList.Remove(messageId);
                    }
                    else
                    {
                        return;
                    }
                }

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (task == null)
                {
                    return;
                }

                var responseMessageLength = binaryReader.ReadInt32();

                var currentPosition = message.Position;
                try
                {
                    var responseMessageByteList = binaryReader.ReadBytes(responseMessageLength);

                    // 为什么需要放在线程池执行？原因是在使用 GetResponse 请求时，收到对方的请求，进入以下代码。此时的代码运行在 DispatchMessage 消息循环线程里面
                    // 通过 CLR 的逻辑可以了解到，在调用 TaskCompletionSource 的 SetResult 方法
                    // 将会让原本 await TaskCompletionSource.Task 的代码继续执行，在调用 SetResult 方法的线程上继续执行
                    // 也就是 GetResponse 返回时的执行代码线程就是 DispatchMessage 消息循环线程
                    // 但是 GetResponse 返回的代码执行是业务代码，如果在业务上锁住了，
                    // 例如 https://github.com/dotnet-campus/dotnetCampus.Ipc/pull/67/commits/74f7a25446f2107f3556ac97659a89b37f82f885 的 "IPC 代理生成：IPC 参数和异步 IPC 返回值" 单元测试，在 GetResponse 之后进行异步转同步等待下一条消息
                    // 此时消息循环线程进入等待，需要等待下一条消息才能继续。但是下一条消息需要等待消息循环线程收到下一条消息。于是相互等待
                    // 让业务代码在消息循环线程上运行，是不符合框架设计的。因此让业务代码调度到线程池执行
                    IpcContext.TaskPool.Run(() => task.SetResult(new IpcMessageBody(responseMessageByteList)));
                }
                finally
                {
                    message.Position = currentPosition;
                }
            }
        }

        private object Locker => TaskList;

        private ulong CurrentMessageId { set; get; }

        public override string ToString()
        {
            return $"[{nameof(IpcMessageRequestManager)}] PeerName={IpcContext.PipeName} CurrentMessageId={CurrentMessageId} WaitingResponseCount={WaitingResponseCount}";
        }
    }
}
