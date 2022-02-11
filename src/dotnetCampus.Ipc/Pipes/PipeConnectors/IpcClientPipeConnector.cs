using System;
using System.Threading.Tasks;
using dotnetCampus.Ipc.Exceptions;

namespace dotnetCampus.Ipc.Pipes.PipeConnectors;

/// <summary>
/// 默认的配置客户端连接方法
/// </summary>
public class IpcClientPipeConnector : IIpcClientPipeConnector
{
    /// <summary>
    /// 创建默认的配置客户端连接方法
    /// </summary>
    /// <param name="canContinue"></param>
    /// <param name="stepTimeout"></param>
    /// <param name="getStepSleepTime"></param>
    public IpcClientPipeConnector(CanContinueDelegate canContinue, TimeSpan? stepTimeout = null,
        GetStepSleepTimeDelegate? getStepSleepTime = null)
    {
        CanContinue = canContinue;
        StepTimeout = stepTimeout ?? DefaultStepTimeout;
        GetStepSleepTime = getStepSleepTime ?? DefaultGetStepSleepTime;
    }

    /// <inheritdoc />
    public async Task ConnectNamedPipeAsync(IpcClientPipeConnectContext ipcClientPipeConnectContext)
    {
        var namedPipeClientStream = ipcClientPipeConnectContext.NamedPipeClientStream;

        int stepCount = 0;
        while (true)
        {
            try
            {
                stepCount++;
                await Task.Run(() => namedPipeClientStream.Connect((int) StepTimeout.TotalMilliseconds))
                    .ConfigureAwait(false);
                return;
            }
            catch (TimeoutException)
            {
                // 连接超时了，判断能否继续
            }

            if (CanContinue(ipcClientPipeConnectContext))
            {
                var sleepTime = GetStepSleepTime(ipcClientPipeConnectContext, stepCount);
                await Task.Delay(sleepTime);
            }
            else
            {
                throw new IpcClientPipeConnectionException(ipcClientPipeConnectContext.PeerName);
            }
        }
    }

    /// <summary>
    /// 一次连接的超时时间
    /// </summary>
    public TimeSpan StepTimeout { get; }

    /// <summary>
    /// 默认一次连接的超时时间
    /// </summary>
    public static TimeSpan DefaultStepTimeout => TimeSpan.FromSeconds(5);

    /// <summary>
    /// 默认连接的之间间隔时间
    /// </summary>
    public static TimeSpan DefaultStepSleepTime => TimeSpan.FromSeconds(1);

    /// <summary>
    /// 获取连接的之间间隔时间
    /// </summary>
    public GetStepSleepTimeDelegate GetStepSleepTime { get; }

    /// <summary>
    /// 默认连接的之间间隔时间
    /// </summary>
    /// <param name="ipcClientPipeConnectContext"></param>
    /// <param name="stepCount"></param>
    /// <returns></returns>
    public static TimeSpan DefaultGetStepSleepTime(IpcClientPipeConnectContext ipcClientPipeConnectContext,
        int stepCount)
    {
        return DefaultStepSleepTime;
    }

    /// <summary>
    /// 是否还能继续连接
    /// </summary>
    /// <remarks>
    /// 每次连接超时之后，将会调用此方法，判断是否可以继续下一次连接
    /// </remarks>
    public CanContinueDelegate CanContinue { get; }

    /// <summary>
    /// 获取连接的之间间隔时间
    /// </summary>
    /// <param name="ipcClientPipeConnectContext"></param>
    /// <param name="stepCount">第几次连接</param>
    /// <returns></returns>
    public delegate TimeSpan GetStepSleepTimeDelegate(IpcClientPipeConnectContext ipcClientPipeConnectContext,
        int stepCount);

    /// <inheritdoc cref="CanContinue"/>
    public delegate bool CanContinueDelegate(IpcClientPipeConnectContext ipcClientPipeConnectContext);
}
