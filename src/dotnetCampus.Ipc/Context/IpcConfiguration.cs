using System;
using System.Collections.Generic;

using dotnetCampus.Ipc.Internals;
using dotnetCampus.Ipc.Pipes.PipeConnectors;
using dotnetCampus.Ipc.Serialization;
using dotnetCampus.Ipc.Threading;
using dotnetCampus.Ipc.Utils.Buffers;
using dotnetCampus.Ipc.Utils.Logging;

namespace dotnetCampus.Ipc.Context
{
    /// <summary>
    /// 进程间通讯的配置
    /// </summary>
    public class IpcConfiguration
    {
        /// <summary>
        /// 自动重连 Peer 是否开启，如开启，在断开后将会自动尝试去重新连接。设置为 true 时，推荐同时设置 <see cref="IpcClientPipeConnector"/> 属性，以防无限重试。
        /// </summary>
        public bool AutoReconnectPeers { get; set; } = false;

        private readonly List<IIpcRequestHandler> _ipcRequestHandlers = new();

        /// <summary>
        /// 消息内容允许最大的长度。超过这个长度，咋不上天
        /// <para>
        /// 如果真有那么大的内容准备传的，自己开共享内存或写文件等方式传输，然后通过 IPC 告知对方如何获取即可
        /// </para>
        /// </summary>
        public const int MaxMessageLength = ushort.MaxValue * byte.MaxValue;

        /// <summary>
        /// 用于内部使用的数组分配池
        /// </summary>
        public ISharedArrayPool SharedArrayPool { get; set; } = new SharedArrayPool();

        /// <summary>
        /// 决定如何调度 IPC 通知到业务的代码。
        /// </summary>
        public IpcTaskScheduling IpcTaskScheduling { get; set; }

        /// <summary>
        /// 添加自定义的 IPC 线程池。此属性一旦设置，将会让 <see cref="IpcTaskScheduling"/> 被无效掉
        /// </summary>
        public CustomIpcThreadPoolBase? CustomIpcThreadPool { set; get; }

        /// <summary>
        /// 为 IPC 记录日志。
        /// </summary>
        /// <remarks>根据当前的 Ipc 名获取到日志对象，一般写法是<code>IpcLoggerProvider = name => new IpcLogger(name)</code></remarks>
        public Func<string, IpcLogger>? IpcLoggerProvider { get; set; }

        /// <summary>
        /// 处理通讯相关业务的定义
        /// </summary>
        public IIpcRequestHandler DefaultIpcRequestHandler { set; get; } = new EmptyIpcRequestHandler();

        /// <summary>
        /// 每一条消息的头，用于处理消息的黏包和通讯损坏问题
        /// </summary>
        /// 在选用 Pipe 通讯，基本不存在通讯损坏等问题，也就是这个 Header 其实用途不大
        /// 这个 Header 的内容就是 dotnet campus 的 Ascii 数组
        /// dotnet campus 0x64, 0x6F, 0x74, 0x6E, 0x65, 0x74, 0x20, 0x63, 0x61, 0x6D, 0x70, 0x75, 0x73
        /// 大概的消息通讯方式如下，详细请看 <see cref="IpcMessageConverter"/> 的代码
        /*
         * Message:
         * Header
         * Length
         * Content
         */
        public byte[] MessageHeader { set; get; } =
            {0x64, 0x6F, 0x74, 0x6E, 0x65, 0x74, 0x20, 0x63, 0x61, 0x6D, 0x70, 0x75, 0x73};

        /// <summary>
        /// 设置或获取客户端的管道连接方法
        /// </summary>
        public IIpcClientPipeConnector? IpcClientPipeConnector { set; get; }

        /// <summary>
        /// 提供给框架调用，用于注入框架特殊处理的请求处理器。
        /// </summary>
        /// <param name="handlers">框架特殊处理的请求处理器。</param>
        internal void AddFrameworkRequestHandlers(params IIpcRequestHandler[] handlers)
        {
            _ipcRequestHandlers.AddRange(handlers);
        }

        /// <inheritdoc cref="AddFrameworkRequestHandlers"/>
        internal void AddFrameworkRequestHandler(IIpcRequestHandler handler)
        {
            _ipcRequestHandlers.Add(handler);
        }

        /// <summary>
        /// 获取框架和业务的请求处理器。
        /// </summary>
        /// <returns>按顺序返回框架注入的请求处理器、业务默认指定的请求处理器。</returns>
        internal IEnumerable<IIpcRequestHandler> GetIpcRequestHandlers()
        {
            foreach (var handler in _ipcRequestHandlers)
            {
                yield return handler;
            }
            if (DefaultIpcRequestHandler is { } @default)
            {
                yield return @default;
            }
        }

        /// <summary>
        /// 用在 IPC 里面的对象序列化器
        /// </summary>
        public IIpcObjectSerializer IpcObjectSerializer
        {
#if UseNewtonsoftJson
            get => _ipcObjectSerializer ??= DefaultNewtonsoftJsonSerializer;
#else
            get => _ipcObjectSerializer ?? throw new InvalidOperationException("必须先设置 IpcObjectSerializer 属性，才能使用 IPC 功能。");
#endif
            set => _ipcObjectSerializer = value;
        }

        private IIpcObjectSerializer? _ipcObjectSerializer;

#if UseNewtonsoftJson
        /// <summary>
        /// 默认的 Newtonsoft Json 序列化器
        /// </summary>
        public static IpcObjectJsonSerializer DefaultNewtonsoftJsonSerializer
        // 不加上锁了，这里不管线程安全，最多就是多创建几个对象而已，不会影响业务逻辑
            => _defaultNewtonsoftJsonSerializer ??= new IpcObjectJsonSerializer();
        private static IpcObjectJsonSerializer? _defaultNewtonsoftJsonSerializer;
#endif
    }
}
