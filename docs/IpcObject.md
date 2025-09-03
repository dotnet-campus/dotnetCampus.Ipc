# 使用「远程对象调用」的方式做进程间通信

假设你有一个接口 `IFoo`，有 A、B 两个进程想做进程间通信。通过 IPC 的「远程对象调用」的方式，你可以让 A 进程调用 B 进程的 `IFoo` 接口方法，就像调用本地对象一样。

## 快速入门

先定义好一个接口，这个接口将可被远程调用：

```csharp
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
```

现在，我们有两个进程 A 和 B：

- A 进程是调用端，想调用 B 进程的 `IFoo` 接口方法。
- B 进程是被调用端，提供 `IFoo` 接口的实现。

为了实现这样的跨进程调用，我们需要在 A 进程和 B 进程分别进行一些 IPC 的初始化。

```csharp
// A 进程 Program.cs

// 1. 初始化 IPC
var ipcProvider = new IpcProvider("IpcRemotingObjectClientDemo");
// 2. 启动 IPC（以支持双向通信）
ipcProvider.StartServer();
// 3. 连接进程 B
var peer = await ipcProvider.GetAndConnectToPeerAsync("IpcRemotingObjectServerDemo");

// 获取来自 B 进程的 IFoo 接口的「代理」（Proxy）
var foo = ipcProvider.CreateIpcProxy<IFoo>(peer);

// 现在，开始自由地享受 IFoo 的远程调用吧！就好像它从来都是自己进程内的对象一样
Console.WriteLine(foo.Name);
Console.WriteLine(foo.Add(1, 2));
Console.WriteLine(await foo.AddAsync("a", 1));
Console.Read();
```

```csharp
// B 进程 Program.cs

// 1. 初始化 IPC
var ipcProvider = new IpcProvider("IpcRemotingObjectServerDemo");

// 创建 IFoo 的实际对象，然后为其创建一个「对接」（Joint）
ipcProvider.CreateIpcJoint<IFoo>(new Foo());

// 2. 启动 IPC（以支持双向通信）
//    这里，我们提前创建好了「对接」，再启动 IPC；这样 A 进程连接 B 进程时，就能马上使用 IFoo 了
ipcProvider.StartServer();

Console.Read();
```

```csharp
// B 进程，IFoo 的实际实现
class Foo : IFoo
{
    public string Name { get; set; } = "Foo";

    public int Add(int a, int b)
    {
        Console.WriteLine($"a({a})+b({b})={a + b}");
        return a + b;
    }

    public async Task<string> AddAsync(string a, int b)
    {
        return await Task.Run(() =>
        {
            Console.WriteLine($"a({a})+b({b})={a + b}");
            return a + b;
        });
    }
}
```

以上，就是「远程对象调用」所需要的所有示例代码了。所有代码来自本仓库 <https://github.com/dotnet-campus/dotnetCampus.Ipc/tree/main/demo/IpcRemotingObjectDemo>。

## 进阶用法

