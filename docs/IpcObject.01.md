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

### `IpcPublic`

上述 IPC 初始化的部分不变，实现不变的情况下，接口 `IFoo` 上的 `[IpcPublic]` 接口还有更多玩法。

```csharp
// IgnoresIpcException = true
//  - A 进程调用 IFoo 的「代理」时，忽略所有的 IPC 异常（即对方进程断开、IFoo 接口出现方法签名的变更等）
//  - 但 A 进程仍然能收到 B 进程 IFoo 实现类的业务异常（如 ArgumentNullException）
// Timeout = 1000
//  - A 进程调用 IFoo 的「代理」时，最多等待 1000 毫秒
//  - 超出时间没有返回，则方法会立即返回；如果方法带有返回值，则会返回返回类型的默认值
[IpcPublic(IgnoresIpcException = true, Timeout = 1000)]
```

这些是对 `IFoo` 这个接口的全局设置。当然，还可以对它内部的每个成员单独设置更多属性：

```csharp
// DefaultReturn = "Error"
//  - A 进程调用 IFoo「代理」的此方法时，如果真发生了 IPC 异常，则会返回指定的默认值
//  - 在这里，是返回 "Error"，而不是 string 的默认值 null（这可以避免破坏可空性）
[IpcMethod(DefaultReturn = "Error", IgnoresIpcException = true, Timeout = 2000)]
Task<string> AddAsync(string a, int b);
```

特别的，对于 void 返回值的方法，还有一个属性 `WaitsVoid`：

```csharp
// WaitsVoid = true
//  - 默认情况下，IPC 不会等待 void 方法返回（因为库作者 @walterlv 认为如果你想等待，改用异步方法更好）
//  - 这意味着你甚至还收不到 B 进程此方法实现的异常
//  - 但如果你确实希望这个 void 像一个本地 void 一样等待，可以设置此属性
[IpcMethod(WaitsVoid = true)]
void Add(int a, int b);
```

哦，对了，属性上也有属性可以设置哦：

```csharp
// IsReadonly = true
//  - 神奇吧，一个 get/set 属性设成只读有什么作用？
//  - 这意味着 A 进程通过「代理」获取此属性的值时，会假设此属性不会再变了，于是缓存起来，只拿这一次；以后都使用这次的缓存
[IpcProperty(IsReadonly = true)]
string Name { get; set; }
```

你可能注意到我们还有一个 `IpcEvent` 可以标在事件上，不过很遗憾地告诉你，目前还没实现事件。所以你会看到我们写了个分析器告诉你不要这么做。

### `IpcShape`

好了，现在更麻烦的事来了。假设你有三个进程 A、B、C：

- A 仍然是调用端
- B 仍然是被调用端
- 新增了一个 C，跟 A 一样是调用端，但希望用不同的方式调用 `IFoo` 这个接口怎么办？

我们前面介绍了 `[IpcPublic]` 特性，它可以用来标记接口及其成员，以详细定制各个成员的 IPC 行为。但是，它一旦在接口上标记了，就意味着所有进程对这个接口的调用都会遵循这个标记的规则。

有没有什么方法，能够允许我 A 和 C 进程使用不同的规则来调用 `IFoo` 接口呢？

答案就是 `[IpcShape]` 特性：

- 你可以在 C 进程里额外定义一个 `IFoo` 接口的空实现
- 然后逐一设置这个空实现中你希望与 `IFoo` 接口中所定义的不同的调用规则

```csharp
[IpcShape(typeof(IFoo))]
internal class IpcFooShape : IFoo
{
    // IFoo 接口上没有设置此属性，所以 A 进程是默认方式访问这个属性的
    // 但是 C 进程通过 IpcShape，在不影响 A 进程访问规则的情况下，定制了 C 进程下的访问规则
    [IpcProperty(IsReadonly = true)]
    public string Name { get; set; } = null!;

    public int Add(int a, int b) => throw null!;
    public async Task<string> AddAsync(string a, int b) => throw null!;
}
```

那么 C 进程的初始化需要有所变化：

```diff
    var ipcProvider = new IpcProvider("IpcRemotingObjectClientDemo");
    ipcProvider.StartServer();
    var peer = await ipcProvider.GetAndConnectToPeerAsync("IpcRemotingObjectServerDemo");

    // 获取来自 B 进程的 IFoo 接口的「代理」（Proxy）
--  var foo = ipcProvider.CreateIpcProxy<IFoo>(peer);
++  // 不过这次，我们使用了 IpcFooShape 这个「形状」（Shape）作为「代理」（Proxy）
++  var foo = ipcProvider.CreateIpcProxy<IpcFooShape>(peer);

    Console.WriteLine(foo.Name);
    Console.WriteLine(foo.Add(1, 2));
    Console.WriteLine(await foo.AddAsync("a", 1));
    Console.Read();
```

## 高级用法

DotNetCampus.Ipc「远程对象调用」支持你嵌套 IPC 对象，这意味着你可以实现更加复杂的 IPC 需求。

```csharp
/// <summary>
/// 嵌套 IPC 类型演示。
/// </summary>
[IpcPublic]
public interface IBar
{
    /// <summary>
    /// 这是一个超复杂的方法，参数和返回值都是 IPC 对象。
    /// </summary>
    Task<IBaz> AddAsync(IQux qux, IQuux quux);
}
```

想想看，这里的每个类型，谁是调用方，谁是被调用方（实现方）？

假设 A 进程试图调用 B 进程的 `IBar` 接口：

| 类型    | A 进程           | B 进程           | 描述                                         |
| ------- | ---------------- | ---------------- | -------------------------------------------- |
| `IBar`  | 代理             | 实现（需要对接） | A 进程调用 `IBar` 的代理以访问 B 进程        |
| `IBaz`  | 代理             | 对接（无需对接） | A 进程拿到了 B 进程的 `IBaz` 的代理          |
| `IQux`  | 实现（无需对接） | 代理             | A 进程有一个 IQux 的实现，会传给 B 进程去用  |
| `IQuux` | 实现（无需对接） | 代理             | A 进程有一个 IQuux 的实现，会传给 B 进程去用 |

这里的 `IBaz` `IQux` `IQuux` 都需要标记 `[IpcPublic]` 或 `[IpcShape]`（当然，我们的分析器也会提示你需要标的）。但好在你不需要额外编写对接代码；我的意思是 IPC 的初始化代码里，你只需要处理 `IBar` 这一个接口就够了，剩下的 DotNetCampus.Ipc 会帮你完成。

## 异常处理

本 IPC 库有两种类型的异常：

- `IpcLocalException`: 表示异常发生在本地进程中
- `IpcRemoteException`: 表示异常发生在远端进程中，或者发生在 IPC 通信过程中

你可能在初始化等过程中收到各种异常，不过「远程对象调用」中只会收到以下这些：

- 本地异常 `IpcLocalException`
    - 「远程对象调用」几乎不会发生本地异常
- 代理异常，如果 IPC 接口的实现方法内抛出了以下这几种异常，则会在调用方也代理出相同类型的异常（这个列表详见 [这里](../src/dotnetCampus.Ipc/CompilerServices/GeneratedProxies/Models/GeneratedProxyExceptionModel.cs)，你也可以提 PR 修改这个列表）
    - `ArgumentException`
    - `ArgumentNullException`
    - `BadImageFormatException`
    - `InvalidCastException`
    - `InvalidOperationException`
    - `NotImplementedException`
    - `NotSupportedException`
    - `NullReferenceException`
- 远端异常 `IpcRemoteException`
    - `IpcInvokingException`: 如果 IPC 接口的实现方法内抛出了上述异常之外的其他异常，则会包装成此异常
    - `IpcInvokingTimeoutException`: 远程对象调用超时（如前面所说，设置 `Timeout` 属性后可以支持超时）

所以，大多数情况下，你只需要像一个本地对象一样去处理异常即可。

不过，如果你想更加可靠一些处理异常，我们正计划做「自动代理」功能，以便能更好地用通用的方式来处理「远程对象调用」中发生的远端**非业务性**异常。功能计划中，文件夹已经建好了，请耐心等待。

## 性能和 AOT 兼容性

1. DotNetCampus.Ipc 库使用源生成器生成「代理」（Proxy）和「对接」（Joint）的代码，旨在提升性能和确保 AOT 兼容性。
    - 目前还仍有少量代码在使用反射，不过我们计划很快将其完全消除
2. DotNetCampus.Ipc 已移除 Newtonsoft.Json 库，完全使用 System.Text.Json 并配合源生成器来做跨进程对象的传输，旨在大幅减少 AOT 之后的大小。

## 最佳实践

为了更好地发挥 IPC「远程对象调用」的代码编写直观性优势，同时又避免不太喜欢的行为，库作者 @walterlv 推荐：

1. IPC 接口中尽量全部使用异步方法
    - 除非你希望这个对象用起来更加像一个本地对象一样，拥有属性、同步的方法
2. 如果一定要用同步方法，也请避免使用 void 返回值
    - 如果真用了 void 返回值，请认真考虑一下要不要设置 `WaitsVoid` 属性

好了，就这些。其他你随便。
