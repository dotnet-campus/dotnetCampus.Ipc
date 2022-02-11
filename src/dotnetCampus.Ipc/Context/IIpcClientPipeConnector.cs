using System;

namespace dotnetCampus.Ipc.Context;

/// <summary>
/// 配置客户端的管道连接
/// </summary>
public interface IIpcClientPipeConnector
{
    /// <summary>
    /// 一次连接的超时时间
    /// </summary>
    public TimeSpan StepTimeout { get; }

    /// <summary>
    /// 获取连接的之间间隔时间
    /// </summary>
    public GetStepSleepTimeDelegate GetStepSleepTime { get; }

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
    /// <param name="stepCount">第几次连接</param>
    /// <returns></returns>
    public delegate TimeSpan GetStepSleepTimeDelegate(int stepCount);

    /// <inheritdoc cref="CanContinue"/>
    public delegate bool CanContinueDelegate();
}
