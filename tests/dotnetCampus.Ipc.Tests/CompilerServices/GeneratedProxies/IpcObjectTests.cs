#nullable enable
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Exceptions;
using dotnetCampus.Ipc.FakeTests.FakeApis;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Tests.CompilerServices.Fake;
using dotnetCampus.Ipc.Tests.CompilerServices.FakeRemote;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dotnetCampus.Ipc.Tests.CompilerServices.GeneratedProxies;

[TestClass]
public class IpcObjectTests
{
    [TestMethod("IPC 代理生成：可空字符串属性")]
    public async Task IpcPropertyTests1()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.NullableStringProperty));

        // 安放。
        var result = proxy.NullableStringProperty;

        // 植物。
        Assert.AreEqual("Title", result);
    }

    [TestMethod("IPC 代理生成：非可空字符串属性")]
    public async Task IpcPropertyTests2()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.NonNullableStringProperty));

        // 安放。
        var result = proxy.NonNullableStringProperty;

        // 植物。
        Assert.AreEqual("Description", result);
    }

    [TestMethod("IPC 代理生成：枚举属性")]
    public async Task IpcPropertyTests3()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.EnumProperty));

        // 安放。
        var result = proxy.EnumProperty;

        // 植物。
        Assert.AreEqual(BindingFlags.Public, result);
    }

    [TestMethod("IPC 代理生成：IPC 只读属性")]
    public async Task IpcPropertyTests4()
    {
        // 准备。
        var instance = new FakeIpcObject();
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.IpcReadonlyProperty), instance);

        // 安放。
        var result1 = proxy.IpcReadonlyProperty;
        instance.SetIpcReadonlyProperty(false);
        var result2 = proxy.IpcReadonlyProperty;

        // 植物。
        Assert.AreEqual(true, result1);
        Assert.AreEqual(true, result2);
    }

#if !NET8_0_OR_GREATER
    [TestMethod("IPC 代理生成：没有原生序列化的属性（以指针属性为例，仅支持 Newtonsoft.Json）")]
    public async Task IpcPropertyTests5()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.IntPtrProperty));

        // 安放。
        var result = proxy.IntPtrProperty;

        // 植物。
        Assert.AreEqual(new IntPtr(1), result);
    }
#endif

    [TestMethod("IPC 代理生成：要等待完成的 void 方法")]
    public async Task IpcMethodsTests1()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.WaitsVoidMethod));

        // 安放。
        proxy.WaitsVoidMethod();
        var result = proxy.EnumProperty;

        // 植物。
        Assert.AreEqual(BindingFlags.Public | BindingFlags.Instance, result);
    }

    [TestMethod("IPC 代理生成：不等待完成的 void 方法")]
    public async Task IpcMethodsTests2()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.NonWaitsVoidMethod));

        // 安放。
        proxy.NonWaitsVoidMethod();
        var result = proxy.EnumProperty;

        // 植物。
        Assert.AreEqual(BindingFlags.Public, result);
    }

    [TestMethod("IPC 代理生成：会 IPC 超时的方法")]
    public async Task IpcMethodsTests3()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.MethodThatHasTimeout));

        // 安放植物。
        await Assert.ThrowsExceptionAsync<IpcInvokingTimeoutException>(async () =>
        {
            await proxy.MethodThatHasTimeout();
        });
    }

    [TestMethod("IPC 代理生成：返回默认值的方法")]
    public async Task IpcMethodsTests4()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.MethodThatHasDefaultReturn));

        // 安放。
        var result = await proxy.MethodThatHasDefaultReturn();

        // 植物。
        Assert.AreEqual("default1", result);
    }

    [TestMethod("IPC 代理生成：返回默认表达式默认值的方法")]
    public async Task IpcMethodsTests5()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.MethodThatHasObjectWithObjectDefaultReturn));

        // 安放。
        var result = await proxy.MethodThatHasObjectWithObjectDefaultReturn();

        // 植物。
        Assert.AreEqual(null, result);
    }

    [TestMethod("IPC 代理生成：返回字符串表达式默认值的方法")]
    public async Task IpcMethodsTests6()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.MethodThatHasObjectWithStringDefaultReturn));

        // 安放。
        var result = await proxy.MethodThatHasObjectWithStringDefaultReturn();

        // 植物。
        Assert.AreEqual("default1", result);
    }

    [TestMethod("IPC 代理生成：返回大写字符串表达式默认值的方法")]
    public async Task IpcMethodsTests7()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.MethodThatHasStringDefaultReturn));

        // 安放。
        var result = await proxy.MethodThatHasStringDefaultReturn();

        // 植物。
        Assert.AreEqual("default1", result);
    }

    [TestMethod("IPC 代理生成：返回自定义表达式默认值的方法")]
    public async Task IpcMethodsTests8()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.MethodThatHasCustomDefaultReturn));

        // 安放。
        var result = await proxy.MethodThatHasCustomDefaultReturn();

        // 植物。
        Assert.AreEqual(new IntPtr(1), result);
    }

    [TestMethod("IPC 代理生成：枚举参数")]
    public async Task IpcParametersAndReturnsTests1()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.MethodWithStructParameters));

        // 安放植物。
        proxy.MethodWithStructParameters(BindingFlags.Public);
    }

    [TestMethod("IPC 代理生成：布尔返回值")]
    public async Task IpcParametersAndReturnsTests2()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.MethodWithStructReturn));

        // 安放。
        var result = proxy.MethodWithStructReturn();

        // 植物。
        Assert.AreEqual(true, result);
    }

    [TestMethod("IPC 代理生成：同数量的参数组成的重载方法组。")]
    public async Task IpcParametersAndReturnsTests3()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.MethodWithSameParameterCountOverloading));

        // 安放。
        var int32Result = proxy.MethodWithSameParameterCountOverloading(1, 2);
        var int64Result = proxy.MethodWithSameParameterCountOverloading(1L, 2L);

        // 植物。
        Assert.AreEqual(3, int32Result);
        Assert.AreEqual(2L, int64Result);
    }

    [TestMethod("IPC 代理生成：异步返回值")]
    public async Task IpcParametersAndReturnsTests4()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.AsyncMethod));

        // 安放植物。
        await proxy.AsyncMethod();
    }

    [TestMethod("IPC 代理生成：多参数和异步结构体返回值。")]
    public async Task IpcParametersAndReturnsTests5()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.AsyncMethodWithStructParametersAndStructReturn));

        // 安放。
        var result = await proxy.AsyncMethodWithStructParametersAndStructReturn(1, 2, 3, 4);

        // 植物。
        Assert.AreEqual(new ValueTuple<double, uint, int, byte>(1, 2, 3, 4), result);
    }

    [TestMethod("IPC 代理生成：复杂参数和异步复杂返回值")]
    public async Task IpcParametersAndReturnsTests6()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.AsyncMethodWithComplexParametersAndComplexReturn));

        // 安放。
        var result = await proxy.AsyncMethodWithComplexParametersAndComplexReturn(new FakeIpcObjectSubModelA(1, 2, 3, 4));

        // 植物。
        Assert.AreEqual((double) 1, result.A);
        Assert.AreEqual((uint) 2, result.B);
        Assert.AreEqual((int) 3, result.C);
        Assert.AreEqual((byte) 4, result.D);
    }

    [TestMethod("IPC 代理生成：字符串参数和异步字符串返回值。")]
    public async Task IpcParametersAndReturnsTests7()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.AsyncMethodWithPrimaryParametersAndPrimaryReturn));

        // 安放。
        var result = await proxy.AsyncMethodWithPrimaryParametersAndPrimaryReturn("Test");

        // 植物。
        Assert.AreEqual("Test", result);
    }

    [TestMethod("IPC 代理生成：IPC 参数和异步 IPC 返回值")]
    public async Task IpcParametersAndReturnsTests8()
    {
        // 准备。
        var proxySideObject = new FakeNestedIpcArgumentOrReturn("test on proxy side");
        var jointSideObject = new FakeNestedIpcArgumentOrReturn("test on joint side");
        var (aProvider, _, peer, proxy) =
            await CreateIpcPairWithProvidersAsync(nameof(FakeIpcObject.AsyncMethodWithIpcPublicObjectParametersAndIpcPublicObjectReturn),
                new FakeIpcObject(jointSideObject));

        // 安放。
        var result = await proxy.AsyncMethodWithIpcPublicObjectParametersAndIpcPublicObjectReturn(proxySideObject, "change on joint side");

        // 植物。
        // 代理端对象的值被对接端修改。
        Assert.AreEqual("change on joint side", proxySideObject.Value);
        // 对接端的值保持原样。
        Assert.AreEqual("test on joint side", jointSideObject.Value);
        Assert.AreEqual("test on joint side", result.Value);

        // 安放。
        result.Value = "test changed from proxy side";

        // 植物。
        // 代理端对象的值保持原样。
        Assert.AreEqual("change on joint side", proxySideObject.Value);
        // 对接端的值被代理端修改。
        Assert.AreEqual("test changed from proxy side", jointSideObject.Value);
        Assert.AreEqual("test changed from proxy side", result.Value);
    }

    [Ignore("似乎在 CI 上这个单元测试会导致卡死？？？")]
    [TestMethod("IPC 代理生成：不同程序集中的同名 IPC 参数和异步 IPC 返回值")]
    public async Task DifferentAssembliesIpcParametersAndReturnsTests()
    {
        // 准备。
        var remoteExecutablePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            "..", "..",
            @"dotnetCampus.Ipc.FakeTests",
            Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyConfigurationAttribute>()!.Configuration.ToLowerInvariant(),
            "dotnetCampus.Ipc.FakeTests.dll");

        if (!File.Exists(remoteExecutablePath))
        {
            throw new FileNotFoundException($"在执行真正跨进程 IPC 通信时，目标程序集未找到，请确认代码中编写的路径是否已更新到最新路径。路径为：{remoteExecutablePath}");
        }

        var process = Process.Start(new ProcessStartInfo("dotnet")
        {
            UseShellExecute = true,
            ArgumentList =
            {
                remoteExecutablePath,
            },
            WorkingDirectory = Path.GetDirectoryName(remoteExecutablePath),
        })!;
        try
        {
            var ipcPeerName = $"IpcObjectTests.IpcTests.RemoteFakeIpcObject";
            var ipcProvider = new IpcProvider(ipcPeerName + ".Local");
            ipcProvider.StartServer();
            var ipcPeer = await ipcProvider.GetAndConnectToPeerAsync(ipcPeerName);
            var remoteObject = ipcProvider.CreateIpcProxy<IRemoteFakeIpcObject>(ipcPeer);
            var ipcArgument = new RemoteIpcArgument("argument");

            // 安放。
            var ipcReturn = await remoteObject.MethodWithIpcParameterAsync(ipcArgument, "changed ipc argument");

            try
            {
                // 植物。
                // 本地值被远端修改。
                Assert.AreEqual("changed ipc argument", ipcArgument.Value);
                // 远端的值保持默认。
                Assert.AreEqual("private", ipcReturn.Value);
            }
            finally
            {
                // 清理。
                await remoteObject.ShutdownAsync();
            }
        }
        finally
        {
            try
            {
                // 清理。
                process.Kill();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

    [TestMethod("IPC 代理生成：集合（列表）属性")]
    public async Task IpcCollectionTests1()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.ListProperty));

        // 安放。
        var result = proxy.ListProperty;

        // 植物。
        CollectionAssert.AreEqual(new string[] { "List1", "List2" }, result);
    }

    [TestMethod("IPC 代理生成：集合（接口）属性")]
    public async Task IpcCollectionTests2()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.CollectionProperty));

        // 安放。
        var result = proxy.CollectionProperty;

        // 植物。
        CollectionAssert.AreEqual(new string[] { "Collection1", "Collection2" }, (ICollection?) result);
    }

    [TestMethod("IPC 代理生成：集合（数组）属性")]
    public async Task IpcCollectionTests3()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.ArrayProperty));

        // 安放。
        var result = proxy.ArrayProperty;

        // 植物。
        CollectionAssert.AreEqual(new string[] { "Array1", "Array2" }, result);
    }

    [TestMethod("IPC 代理生成：集合（列表）异步方法")]
    public async Task IpcCollectionTests4()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.MethodWithListParametersAndListReturn));

        // 安放。
        var result = await proxy.MethodWithListParametersAndListReturn(
            new List<string> { "a", "b" },
            new List<string> { "c", "d" });

        // 植物。
        CollectionAssert.AreEqual(new string[] { "a", "b", "c", "d" }, result);
    }

    [TestMethod("IPC 代理生成：集合（接口）异步方法")]
    public async Task IpcCollectionTests5()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.MethodWithCollectionParametersAndCollectionReturn));

        // 安放。
        var result = await proxy.MethodWithCollectionParametersAndCollectionReturn(
            new List<string> { "a", "b" },
            new List<string> { "c", "d" });

        // 植物。
        CollectionAssert.AreEqual(new string[] { "a", "b", "c", "d" }, (ICollection) result);
    }

    [TestMethod("IPC 代理生成：集合（数组）异步方法")]
    public async Task IpcCollectionTests6()
    {
        // 准备。
        var (peer, proxy) = await CreateIpcPairAsync(nameof(FakeIpcObject.MethodWithArrayParametersAndArrayReturn));

        // 安放。
        var result = await proxy.MethodWithArrayParametersAndArrayReturn(
            new string[] { "a", "b" },
            new string[] { "c", "d" });

        // 植物。
        CollectionAssert.AreEqual(new string[] { "a", "b", "c", "d" }, result);
    }

    [TestMethod("IPC 代理生成：忽略异常的方法")]
    public async Task IpcMethodExceptionTests1()
    {
        // 准备。
        var name = nameof(FakeIpcObject.MethodThatIgnoresIpcException);
        var aName = $"IpcObjectTests.IpcTests.{name}.A";
        var bName = $"IpcObjectTests.IpcTests.{name}.B";
        var aProvider = new IpcProvider(aName);
        var bProvider = new IpcProvider(bName);
        aProvider.StartServer();
        bProvider.StartServer();
        var aJoint = aProvider.CreateIpcJoint<IFakeIpcObject>(new FakeIpcObject());
        var aPeer = await bProvider.GetAndConnectToPeerAsync(aName);
        var bProxy = bProvider.CreateIpcProxy<IFakeIpcObject>(aPeer);

        // 安放植物。
        // 没有发生异常。
        var task = bProxy.MethodThatIgnoresIpcException();
        await Task.Run(async () =>
        {
            await Task.Delay(20);
            aProvider.Dispose();
        });
        await task;
    }

    [TestMethod("IPC 代理生成：没有忽略异常的方法")]
    public async Task IpcMethodExceptionTests2()
    {
        // 准备。
        var name = nameof(FakeIpcObject.MethodThatThrowsIpcException);
        var aName = $"IpcObjectTests.IpcTests.{name}.A";
        var bName = $"IpcObjectTests.IpcTests.{name}.B";
        var aProvider = new IpcProvider(aName);
        var bProvider = new IpcProvider(bName);
        aProvider.StartServer();
        bProvider.StartServer();
        var aJoint = aProvider.CreateIpcJoint<IFakeIpcObject>(new FakeIpcObject());
        var aPeer = await bProvider.GetAndConnectToPeerAsync(aName);
        var bProxy = bProvider.CreateIpcProxy<IFakeIpcObject>(aPeer);

        // 安放。
        var task = bProxy.MethodThatThrowsIpcException();
        await Task.Run(async () =>
        {
            await Task.Delay(20);
            aProvider.Dispose();
        });

        // 植物。
        try
        {
            await task;
        }
        catch (IpcRemoteException e)
        {
            if (e.InnerException is IpcPeerConnectionBrokenException)
            {
                // 期望的异常是外层是 IpcRemoteException 异常
                // 里层是 IpcPeerConnectionBrokenException 异常
                // 外层的异常将包括发送的消息的调试使用的 Tag 信息
                return;
            }
        }

        Assert.Fail($"期望的异常没有被抛出");
    }

    [TestMethod("IPC 代理生成：成员上没有标记忽略异常，但是类型上标记了，也要忽略异常")]
    public async Task IpcMethodExceptionTests3()
    {
        // 准备。
        var name = $"{nameof(FakeIpcObjectWithTypeAttributes)}.{nameof(FakeIpcObject.MethodThatThrowsIpcException)}";
        var aName = $"IpcObjectTests.IpcTests.{name}.A";
        var bName = $"IpcObjectTests.IpcTests.{name}.B";
        var aProvider = new IpcProvider(aName);
        var bProvider = new IpcProvider(bName);
        aProvider.StartServer();
        bProvider.StartServer();
        var aJoint = aProvider.CreateIpcJoint<IFakeIpcObject>(new FakeIpcObjectWithTypeAttributes());
        var aPeer = await bProvider.GetAndConnectToPeerAsync(aName);
        var bProxy = bProvider.CreateIpcProxy<IFakeIpcObject>(aPeer, new IpcProxyConfigs { IgnoresIpcException = true, });

        // 安放植物。
        // 没有发生异常。
        var task = bProxy.MethodThatThrowsIpcException();
        await Task.Run(async () =>
        {
            await Task.Delay(20);
            aProvider.Dispose();
        });
        await task;
    }

    private async Task<(IPeerProxy peer, IFakeIpcObject proxy)> CreateIpcPairAsync(string name, FakeIpcObject? instance = null)
    {
        var aName = $"IpcObjectTests.IpcTests.{name}.A";
        var bName = $"IpcObjectTests.IpcTests.{name}.B";
        var aProvider = new IpcProvider(aName, TestJsonContext.CreateIpcConfiguration());
        var bProvider = new IpcProvider(bName, TestJsonContext.CreateIpcConfiguration());
        aProvider.StartServer();
        bProvider.StartServer();
        var aJoint = aProvider.CreateIpcJoint<IFakeIpcObject>(instance ?? new FakeIpcObject());
        var aPeer = await bProvider.GetAndConnectToPeerAsync(aName);
        var bProxy = bProvider.CreateIpcProxy<IFakeIpcObject>(aPeer);
        // 这里的延迟是为了暂时缓解死锁 bug @lindexi
        // 这个问题其实是因为 IpcObject 这一套可能存在异步转同步等待的问题
        // 问题原因如下：
        // 从 GetAndConnectToPeerAsync 返回的时，是消息接受端 DispatchMessage 所在线程调用过来的
        // 如果此线程卡住了，那就意味着不再能够接收到消息
        // 那为什么存在锁的问题？因为如果在接下来一句话是走 IpcObject 框架获取远程对象的属性值类似的代码
        // 将会在获取属性时，进入异步转同步等待，需要等待到什么时候才会继续执行？需要等待消息接受端 DispatchMessage 接收到远程对象返回的消息
        // 然而消息接受端 DispatchMessage 所在线程已经进入异步转同步等待，导致无法接收到消息，进而导致 DispatchMessage 所在线程无法释放
        // 那为什么设计上要让 GetAndConnectToPeerAsync 返回的线程是消息接受端 DispatchMessage 所在线程？原因是在主动发起对 Peer 连接时，也许需要进行一些事件加等处理等，如果在另一个线程，那可能出现在获取到 Peer 的同时也接收到 Peer 发送过来的消息。这会存在由于事件加等处理在另一个线程，没有及时执行，导致丢失消息
        await Task.Delay(100);
        return (aPeer, bProxy);
    }

    private async Task<(IIpcProvider aProvider, IIpcProvider bProvider, IPeerProxy peer, IFakeIpcObject proxy)> CreateIpcPairWithProvidersAsync(string name,
        FakeIpcObject? instance = null)
    {
        var aName = $"IpcObjectTests.IpcTests.{name}.A";
        var bName = $"IpcObjectTests.IpcTests.{name}.B";
        var aProvider = new IpcProvider(aName);
        var bProvider = new IpcProvider(bName);
        aProvider.StartServer();
        bProvider.StartServer();
        var aJoint = aProvider.CreateIpcJoint<IFakeIpcObject>(instance ?? new FakeIpcObject());
        var aPeer = await bProvider.GetAndConnectToPeerAsync(aName);
        var bProxy = bProvider.CreateIpcProxy<IFakeIpcObject>(aPeer);
        // 这里的延迟是为了暂时缓解死锁 bug @lindexi
        await Task.Delay(100);
        return (aProvider, bProvider, aPeer, bProxy);
    }
}
