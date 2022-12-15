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
    /// <param name="canContinue">是否能继续连接的判断委托</param>
    /// <param name="stepTimeout">每一次连接的等待超时时间</param>
    /// <param name="stepSleepTimeGetter">等待超时之后，下一次连接的延迟时间。连接的之间间隔时间。可以根据连接次数，不断延长延迟时间</param>
    public IpcClientPipeConnector(CanContinueDelegate canContinue, TimeSpan? stepTimeout = null,
        GetStepSleepTimeDelegate? stepSleepTimeGetter = null)
    {
        CanContinue = canContinue;
        StepTimeout = stepTimeout ?? DefaultStepTimeout;
        StepSleepTimeGetter = stepSleepTimeGetter ?? DefaultGetStepSleepTime;
    }

    /// <inheritdoc />
    public async Task ConnectNamedPipeAsync(IpcClientPipeConnectionContext ipcClientPipeConnectionContext)
    {
        var namedPipeClientStream = ipcClientPipeConnectionContext.NamedPipeClientStream;

        int stepCount = 0;
        while (true)
        {
            try
            {
                stepCount++;
                // 由于 namedPipeClientStream.ConnectAsync 底层也是使用 Task.Run 执行 Connect 的逻辑
                // 且 ConnectAsync 在 .NET Framework 4.5 不存在
                // 因此这里就使用 Task.Run 执行
                await Task.Run(() => namedPipeClientStream.Connect((int) StepTimeout.TotalMilliseconds))
                    .ConfigureAwait(false);
                return;
            }
            catch (TimeoutException)
            {
                // 连接超时了，继续执行下面的逻辑
                // 如果没有连接超时，连接成功了，那就会进入上面的 return 分支，方法结束
                // 如果抛出其他异常了，那就不接住，继续向上抛出
            }

            if (CanContinue(ipcClientPipeConnectionContext))
            {
                var sleepTime = StepSleepTimeGetter(ipcClientPipeConnectionContext, stepCount);
                await Task.Delay(sleepTime)
                    .ConfigureAwait(false);
            }
            else
            {
                throw new IpcClientPipeConnectionException(ipcClientPipeConnectionContext.PeerName);
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
    public GetStepSleepTimeDelegate StepSleepTimeGetter { get; }

    /// <summary>
    /// 默认连接的之间间隔时间
    /// </summary>
    /// <param name="ipcClientPipeConnectionContext"></param>
    /// <param name="stepCount"></param>
    /// <returns></returns>
    public static TimeSpan DefaultGetStepSleepTime(IpcClientPipeConnectionContext ipcClientPipeConnectionContext,
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
    /// <param name="ipcClientPipeConnectionContext"></param>
    /// <param name="stepCount">第几次连接</param>
    /// <returns></returns>
    public delegate TimeSpan GetStepSleepTimeDelegate(IpcClientPipeConnectionContext ipcClientPipeConnectionContext,
        int stepCount);

    /// <inheritdoc cref="CanContinue"/>
    public delegate bool CanContinueDelegate(IpcClientPipeConnectionContext ipcClientPipeConnectionContext);
}
