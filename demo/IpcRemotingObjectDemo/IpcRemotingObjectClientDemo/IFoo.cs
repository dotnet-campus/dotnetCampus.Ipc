using dotnetCampus.Ipc.CompilerServices.Attributes;

namespace IpcRemotingObjectServerDemo; // Must the same namespace

/// <summary>
/// 可跨进程调用的接口演示。
/// </summary>
[IpcPublic]
public interface IFoo
{
    /// <summary>
    /// 属性演示。支持 get/set 属性、get 只读属性，支持跨进程报告异常。
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// 方法演示。支持参数、返回值，支持跨进程报告异常。
    /// </summary>
    int Add(int a, int b);

    /// <summary>
    /// 异步方法（更推荐）演示。支持参数、返回值，支持跨进程报告异常。
    /// </summary>
    Task<string> AddAsync(string a, int b);
}
