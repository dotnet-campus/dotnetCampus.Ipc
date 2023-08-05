using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using dotnetCampus.Ipc.Internals;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Pipes.PipeConnectors;
using dotnetCampus.Ipc.Threading;
using dotnetCampus.Ipc.Threading.Tasks;
using dotnetCampus.Ipc.Utils.Logging;

namespace dotnetCampus.Ipc.Context
{
    /// <summary>
    /// 用于作为 Ipc 库的上下文，包括各个过程需要使用的工具和配置等
    /// </summary>
    public class IpcContext
    {
        /// <summary>
        /// 默认的管道名
        /// </summary>
        public const string DefaultPipeName = "dotnet campus";

        private static readonly IpcTask DefaultIpcTask = new(new IpcThreadPool());

        /// <summary>
        /// 创建上下文
        /// </summary>
        /// <param name="ipcProvider"></param>
        /// <param name="pipeName">管道名，也将被做来作为服务器名或当前服务名</param>
        /// <param name="ipcConfiguration"></param>
        public IpcContext(IpcProvider ipcProvider, string pipeName, IpcConfiguration? ipcConfiguration = null)
        {
            IpcProvider = ipcProvider;
            PipeName = pipeName;

            AckManager = new AckManager();
            IpcRequestHandlerProvider = new IpcRequestHandlerProvider(this);

            IpcConfiguration = ipcConfiguration ?? new IpcConfiguration();
            GeneratedProxyJointIpcContext = new GeneratedProxyJointIpcContext(this);

            IpcClientPipeConnector = IpcConfiguration.IpcClientPipeConnector;

            if (IpcConfiguration.CustomIpcThreadPool is { } customIpcThreadPool)
            {
                TaskPool = new IpcTask(customIpcThreadPool);
            }
            else
            {
                TaskPool = IpcConfiguration.IpcTaskScheduling is IpcTaskScheduling.GlobalConcurrent
                    // 支持并发的 IPC 将共用同一个线程池。
                    ? DefaultIpcTask
                    // 要求在同一线程调度的 IPC 将近似独享一个“线程”。
                    : new IpcTask(new IpcSingleThreadPool());
            }

            Logger = IpcConfiguration.IpcLoggerProvider?.Invoke(pipeName) ?? new IpcLogger(pipeName);
        }

        internal AckManager AckManager { get; }

        internal GeneratedProxyJointIpcContext GeneratedProxyJointIpcContext { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{PipeName}]";
        }

        internal IpcConfiguration IpcConfiguration { get; }

        /// <summary>
        /// 获取客户端的管道连接方法
        /// </summary>
        internal IIpcClientPipeConnector? IpcClientPipeConnector { get; }

        internal IpcProvider IpcProvider { get; }

        internal IpcRequestHandlerProvider IpcRequestHandlerProvider { get; }

        internal IpcMessageResponseManager IpcMessageResponseManager { get; } = new IpcMessageResponseManager();

        /// <summary>
        /// 管道名，本地服务器名
        /// </summary>
        public string PipeName { get; }

        internal PeerRegisterProvider PeerRegisterProvider { get; } = new PeerRegisterProvider();

        internal ILogger Logger { get; }

        /// <summary>
        /// 供 IPC 使用的线程池。
        /// 特点为按顺序触发执行，但如果前一个任务执行超时，下一个任务将转到其他线程中执行。
        /// 适用于：
        ///  1. 期望执行顺序与触发顺序一致；
        ///  2. 大多数为小型任务，但可能会出现一些难以预料到的长时间的任务；
        ///  3. 不阻塞调用线程。
        /// </summary>
        internal IpcTask TaskPool { get; }

        // 当前干掉回应的逻辑
        ///// <summary>
        ///// 规定回应 ack 的值使用的 ack 是最大值
        ///// </summary>
        //internal Ack AckUsedForReply { get; } = new Ack(ulong.MaxValue);

        internal bool IsDisposing { set; get; }
        internal bool IsDisposed { set; get; }
    }
}
