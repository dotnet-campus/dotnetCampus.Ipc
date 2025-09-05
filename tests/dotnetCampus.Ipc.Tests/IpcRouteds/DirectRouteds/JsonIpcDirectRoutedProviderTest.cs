using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Exceptions;
using dotnetCampus.Ipc.IpcRouteds.DirectRouteds;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Tests.CompilerServices;
using dotnetCampus.Ipc.Threading;
using dotnetCampus.Ipc.Utils.Logging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dotnetCampus.Ipc.Tests.IpcRouteds.DirectRouteds;

[TestClass]
public class JsonIpcDirectRoutedProviderTest
{
    [TestMethod("多个通讯框架共用相同的 IpcProvider 对象，相互之间不受影响")]
    public async Task TestShared()
    {
        var name = "JsonIpcDirectRoutedProviderTest_1";
        var aName = $"IpcObjectTests.IpcTests.{name}.A";
        var bName = $"IpcObjectTests.IpcTests.{name}.B";
        var aProvider = new IpcProvider(aName);
        var bProvider = new IpcProvider(bName);

        var serverProvider = new JsonIpcDirectRoutedProvider(aProvider);
        var clientProvider = new JsonIpcDirectRoutedProvider(bProvider);

        const string routedPath = "Foo1";
        serverProvider.AddRequestHandler(routedPath, (FakeArgument argument) =>
        {
            return new FakeResult("Ok");
        });

        serverProvider.StartServer();
        clientProvider.StartServer();

        // 有上面的 StartServer 其实就可以不需要有下面的启动
        aProvider.StartServer();
        bProvider.StartServer();

        var aJoint = aProvider.CreateIpcJoint<IFakeIpcObject>(new FakeIpcObject());
        var aPeer = await bProvider.GetAndConnectToPeerAsync(aName);
        var bProxy = bProvider.CreateIpcProxy<IFakeIpcObject>(aPeer);

        // 防止获取属性异步转同步卡住
        await Task.Yield();

        // 两个框架发送的内容，都经过相同的一个管道，但是相互不影响
        var result = bProxy.NullableStringProperty;
        Assert.IsNotNull(result);

        // 发送直接路由的请求
        var clientProxy = await clientProvider.GetAndConnectClientAsync(aName);
        var fakeResult = await clientProxy.GetResponseAsync<FakeResult>(routedPath, new FakeArgument("Name", 1));
        Assert.AreEqual("Ok", fakeResult.Name);
    }

    [TestMethod("客户端请求服务端，可以在服务端收到客户端请求的内容")]
    public async Task TestRequest1()
    {
        // 初始化服务端
        var serverName = "JsonIpcDirectRoutedProviderTest_Request_1";
        var serverProvider = new JsonIpcDirectRoutedProvider(serverName,
            new IpcConfiguration() { IpcLoggerProvider = name => new IpcLogger(name) { MinLogLevel = LogLevel.Debug, } }
                .UseTestFrameworkJsonSerializer()
            );
        var argument = new FakeArgument("TestName", 1);

        var responseText = $"OK_{Guid.NewGuid().ToString()}";

        int enterCount = 0;
        serverProvider.AddRequestHandler("Foo1", (FakeArgument arg) =>
        {
            // 没有任何逻辑请求，不能处理
            enterCount++;
            Assert.AreEqual(argument.Name, arg.Name);
            Assert.AreEqual(argument.Count, arg.Count);

            return new FakeResult(responseText);
        });

        serverProvider.AddRequestHandler("Foo2", (FakeArgument arg, JsonIpcDirectRoutedContext context) =>
        {
            // 没有任何逻辑请求，不能处理
            Assert.Fail();
            return new FakeResult("Ok");
        });

        serverProvider.StartServer();

        // 创建客户端
        // 允许管道名无参数，如果只是做客户端使用的话
        JsonIpcDirectRoutedProvider clientProvider = new(ipcConfiguration: new IpcConfiguration()
        {
            IpcLoggerProvider = name => new IpcLogger(name) { MinLogLevel = LogLevel.Debug, }
        }.UseTestFrameworkJsonSerializer());
        // 对于 clientProvider 来说，可选调用 StartServer 方法
        var clientProxy = await clientProvider.GetAndConnectClientAsync(serverName);

        var result = await clientProxy.GetResponseAsync<FakeResult>("Foo1", argument);

        // 可以获取到响应内容
        Assert.AreEqual(responseText, result!.Name);

        // 要求只进入一次
        Assert.AreEqual(1, enterCount);
    }

    [TestMethod("客户端到服务端的请求，可以获取到服务端的响应")]
    public async Task TestRequest2()
    {
        // 初始化服务端
        var serverName = "JsonIpcDirectRoutedProviderTest_Request_2";
        var clientName = "JsonIpcDirectRoutedProviderTest_Request_Client_1";
        var serverProvider = new JsonIpcDirectRoutedProvider(serverName, TestJsonContext.CreateIpcConfiguration());
        var argument = new FakeArgument("TestName", 1);

        var responseText = Guid.NewGuid().ToString();

        int enterCount = 0;
        serverProvider.AddRequestHandler("Foo1", (FakeArgument arg) =>
        {
            // 没有任何逻辑请求，不能处理
            Assert.Fail();
            return new FakeResult("Ok");
        });

        serverProvider.AddRequestHandler("Foo2", (FakeArgument arg, JsonIpcDirectRoutedContext context) =>
        {
            // 可以获取到客户端名
            Assert.AreEqual(clientName, context.PeerName);

            enterCount++;

            return new FakeResult(responseText);
        });

        serverProvider.StartServer();

        // 创建客户端
        // 允许无参数，如果只是做客户端使用的话
        JsonIpcDirectRoutedProvider clientProvider = new(clientName, TestJsonContext.CreateIpcConfiguration());
        // 对于 clientProvider 来说，可选调用 StartServer 方法
        var clientProxy = await clientProvider.GetAndConnectClientAsync(serverName);

        var result = await clientProxy.GetResponseAsync<FakeResult>("Foo2", argument);

        // 可以获取到响应内容
        Assert.AreEqual(responseText, result.Name);

        // 要求只进入一次
        Assert.AreEqual(1, enterCount);
    }

    [TestMethod("允许创建多个服务端实例共用相同的 IpcProvider 对象，但是如果多个服务端对相同的路由进行处理，只有先添加的才能收到消息")]
    public async Task TestRequest3()
    {
        // 初始化服务端
        var serverName = "JsonIpcDirectRoutedProviderTest_Request_3";
        // 这样的创建方式也是对 IPC 连接最高定制的方式
        IpcProvider ipcProvider = new(serverName, TestJsonContext.CreateIpcConfiguration());
        var serverProvider1 = new JsonIpcDirectRoutedProvider(ipcProvider);
        var argument = new FakeArgument("TestName", 1);
        int enterCount = 0;
        const string routedPath = "Foo1";
        var responseText = Guid.NewGuid().ToString();
        TaskCompletionSource taskCompletionSource = new();

        serverProvider1.AddRequestHandler(routedPath, (FakeArgument arg) =>
        {
            enterCount++;
            return new FakeResult(responseText);
        });

        // 再次开启一个服务，共用相同的 IpcProvider 对象
        var serverProvider2 = new JsonIpcDirectRoutedProvider(ipcProvider);

        serverProvider2.AddRequestHandler(routedPath, (FakeArgument arg) =>
        {
            // 第二个服务收不到消息
            Assert.Fail();

            return new FakeResult(responseText);
        });

        // 多服务使用相同的 IpcProvider 对象存在一个问题，那就是如果是原先 IpcProvider 就启动过的，那这里添加处理一定会抛出异常
        // 意味着需要所有的 JsonIpcDirectRoutedProvider 都创建和添加处理，才能一起调用 StartServer 开始
        serverProvider1.StartServer();
        serverProvider2.StartServer();

        // 创建客户端
        // 允许无参数，如果只是做客户端使用的话
        JsonIpcDirectRoutedProvider clientProvider = new(ipcConfiguration: TestJsonContext.CreateIpcConfiguration());
        var clientProxy = await clientProvider.GetAndConnectClientAsync(serverName);

        var result = await clientProxy.GetResponseAsync<FakeResult>("Foo1", argument);

        // 可以获取到响应内容
        Assert.IsNotNull(result);
        Assert.AreEqual(responseText, result.Name);

        // 要求只进入一次
        Assert.AreEqual(1, enterCount);
    }

    [TestMethod("请求不存在的路径，能收到异常")]
    public async Task TestRequest4()
    {
        // 初始化服务端
        var serverName = "JsonIpcDirectRoutedProviderTest_Request_4";

        IpcProvider ipcProvider = new(serverName, TestJsonContext.CreateIpcConfiguration());
        var serverProvider1 = new JsonIpcDirectRoutedProvider(ipcProvider);

        serverProvider1.AddRequestHandler("F1", (FakeArgument arg) =>
        {
            return new FakeResult("F123");
        });
        serverProvider1.StartServer();

        // 创建客户端
        JsonIpcDirectRoutedProvider clientProvider = new(ipcConfiguration: TestJsonContext.CreateIpcConfiguration());
        var clientProxy = await clientProvider.GetAndConnectClientAsync(serverName);
        var argument = new FakeArgument("TestName", 1);

        await Assert.ThrowsExceptionAsync<JsonIpcDirectRoutedCanNotFindRequestHandlerException>(async () =>
        {
            var result = await clientProxy.GetResponseAsync<FakeResult>("不存在", argument);
            _ = result;
        });
    }

    [TestMethod("如果请求的对象出现了异常，可以正确收到请求响应结束和具体的远端异常信息，而不会进入无限等待")]
    public async Task TestException1()
    {
        // 初始化服务端
        var serverName = "JsonIpcDirectRoutedProviderTest_TestException_1";
        var jsonIpcDirectRoutedProvider = new JsonIpcDirectRoutedProvider(serverName, TestJsonContext.CreateIpcConfiguration());
        var path = "Foo";
        jsonIpcDirectRoutedProvider.AddRequestHandler(path, (FakeArgument fakeArgument) =>
        {
            if (!string.IsNullOrEmpty(fakeArgument.Name))
            {
                throw new FooException1("FooExceptionInfo");
            }

            return new FakeResult("xx");
        });
        jsonIpcDirectRoutedProvider.StartServer();

        var t = new JsonIpcDirectRoutedProvider(ipcConfiguration: TestJsonContext.CreateIpcConfiguration());
        var client = await t.GetAndConnectClientAsync(serverName);
        try
        {
            var response = await client.GetResponseAsync<FakeResult>(path, new FakeArgument("xx", 1));
            _ = response;
        }
        catch (JsonIpcDirectRoutedHandleRequestRemoteException e)
        {
            // 不要使用 Assert.ThrowsException 方法，这个方法不适合同时将单元测试作为调试程序，不方便看到具体异常信息内容
            // e.RemoteExceptionType 是 dotnetCampus.Ipc.Tests.IpcRouteds.DirectRouteds.JsonIpcDirectRoutedProviderTest+FooException1
            Assert.IsTrue(e.RemoteExceptionType.Contains(nameof(FooException1)));
            Assert.IsTrue(e.RemoteExceptionMessage == "FooExceptionInfo");

            return;
        }
        // 预期能进入到 catch 分支进行返回
        Assert.Fail("必定能进入到 catch 分支");
    }

    [TestMethod("重复调用 JsonIpcDirectRoutedProvider 添加通知处理相同的消息，将会抛出异常")]
    public void AddNotifyHandler()
    {
        JsonIpcDirectRoutedProvider provider = new();

        var routedPath = "FooPath";

        provider.AddNotifyHandler(routedPath, (FakeArgument arg) =>
        {
        });

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            provider.AddNotifyHandler(routedPath, (FakeArgument arg) =>
            {
            });
        });
    }

    [TestMethod("重复调用 AddRequestHandler 添加请求处理相同的消息，将会抛出异常")]
    public void AddRequestHandler()
    {
        JsonIpcDirectRoutedProvider provider = new();

        var routedPath = "FooPath";

        provider.AddRequestHandler(routedPath, (FakeArgument argument) =>
        {
            return "Ok";
        });

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            provider.AddRequestHandler(routedPath, (FakeArgument argument) =>
            {
                return "Ok";
            });
        });
    }

    [TestMethod("允许创建多个服务端实例共用相同的 IpcProvider 对象，从而每个服务端接收不同的通知")]
    public async Task TestNotify1()
    {
        // 初始化服务端
        var serverName = "JsonIpcDirectRoutedProviderTest_Notify_3";
        // 这样的创建方式也是对 IPC 连接最高定制的方式
        IpcProvider ipcProvider = new(serverName, TestJsonContext.CreateIpcConfiguration());
        var serverProvider1 = new JsonIpcDirectRoutedProvider(ipcProvider);
        var argument = new FakeArgument("TestName", 1);
        int enterCount = 0;
        TaskCompletionSource taskCompletionSource = new();

        serverProvider1.AddNotifyHandler("Foo1", (FakeArgument arg) =>
        {
            Interlocked.Increment(ref enterCount);
            Assert.AreEqual(argument.Name, arg.Name);
            Assert.AreEqual(argument.Count, arg.Count);

            if (enterCount == 2)
            {
                taskCompletionSource.TrySetResult();
            }
        });

        // 再次开启一个服务，共用相同的 IpcProvider 对象
        var serverProvider2 = new JsonIpcDirectRoutedProvider(ipcProvider);

        serverProvider2.AddNotifyHandler("Foo2", (FakeArgument arg) =>
        {
            Interlocked.Increment(ref enterCount);
            Assert.AreEqual(argument.Name, arg.Name);
            Assert.AreEqual(argument.Count, arg.Count);

            if (enterCount == 2)
            {
                taskCompletionSource.TrySetResult();
            }
        });
        serverProvider1.StartServer();
        serverProvider2.StartServer();

        // 创建客户端
        // 允许无参数，如果只是做客户端使用的话
        JsonIpcDirectRoutedProvider clientProvider = new(ipcConfiguration: TestJsonContext.CreateIpcConfiguration());
        var clientProxy = await clientProvider.GetAndConnectClientAsync(serverName);
        await clientProxy.NotifyAsync("Foo1", argument);
        // 预期这条消息是在第二个服务处理的
        await clientProxy.NotifyAsync("Foo2", argument);

        // 等待接收完成，以上的 await 返回仅仅只是发送出去，不代表对方已接收到
        await taskCompletionSource.Task.WaitTimeout(TimeSpan.FromSeconds(5));
        // 两个服务都进入
        Assert.AreEqual(2, enterCount);
    }

    [TestMethod("从客户端通知到服务端，可以在服务端获取到通知的客户端名")]
    public async Task TestNotify2()
    {
        // 初始化服务端
        var serverName = "JsonIpcDirectRoutedProviderTest_Notify_2";
        var clientName = "JsonIpcDirectRoutedProviderTest_Notify_Client_1";
        var serverProvider = new JsonIpcDirectRoutedProvider(serverName, TestJsonContext.CreateIpcConfiguration());
        var argument = new FakeArgument("TestName", 1);

        int enterCount = 0;
        TaskCompletionSource taskCompletionSource = new();
        serverProvider.AddNotifyHandler("Foo1", (FakeArgument arg) =>
        {
            // 没有任何逻辑请求，不能处理
            Assert.Fail();
        });

        serverProvider.AddNotifyHandler<FakeArgument>("Foo2", (arg, context) =>
        {
            // 可以获取到客户端名
            Assert.AreEqual(clientName, context.PeerName);

            enterCount++;

            taskCompletionSource.TrySetResult();
        });

        serverProvider.StartServer();

        // 创建客户端
        // 允许无参数，如果只是做客户端使用的话
        JsonIpcDirectRoutedProvider clientProvider = new(clientName, TestJsonContext.CreateIpcConfiguration());
        // 对于 clientProvider 来说，可选调用 StartServer 方法
        var clientProxy = await clientProvider.GetAndConnectClientAsync(serverName);

        await clientProxy.NotifyAsync("Foo2", argument);

        // 等待接收完成，以上的 await 返回仅仅只是发送出去，不代表对方已接收到
        await taskCompletionSource.Task.WaitTimeout(TimeSpan.FromSeconds(5));
        // 要求只进入一次
        Assert.AreEqual(1, enterCount);
    }

    [TestMethod("客户端的通知可以成功发送到服务端")]
    public async Task TestNotify3()
    {
        // 初始化服务端
        var serverName = "JsonIpcDirectRoutedProviderTest_Notify_1";
        var serverProvider = new JsonIpcDirectRoutedProvider(serverName, TestJsonContext.CreateIpcConfiguration());
        var routedPath = "Foo1";
        var argument = new FakeArgument("TestName", 1);

        int enterCount = 0;
        TaskCompletionSource taskCompletionSource = new();
        serverProvider.AddNotifyHandler(routedPath, (FakeArgument arg) =>
        {
            enterCount++;
            Assert.AreEqual(argument.Name, arg.Name);
            Assert.AreEqual(argument.Count, arg.Count);

            taskCompletionSource.TrySetResult();
        });

        serverProvider.AddNotifyHandler<FakeArgument>("Foo2", (arg, context) =>
        {
            // 没有任何逻辑请求，不能处理
            Assert.Fail();
        });

        serverProvider.StartServer();

        // 创建客户端
        // 允许无参数，如果只是做客户端使用的话
        JsonIpcDirectRoutedProvider clientProvider = new(ipcConfiguration: TestJsonContext.CreateIpcConfiguration());
        clientProvider.StartServer();
        var clientProxy = await clientProvider.GetAndConnectClientAsync(serverName);

        await clientProxy.NotifyAsync(routedPath, argument);

        // 等待接收完成，以上的 await 返回仅仅只是发送出去，不代表对方已接收到
        await taskCompletionSource.Task.WaitTimeout(TimeSpan.FromSeconds(5));
        // 要求只进入一次
        Assert.AreEqual(1, enterCount);
    }

    [TestMethod("配置 LocalOneByOne 即可让服务端收到的通知消息是一条条按照顺序接收的")]
    public async Task TestNotifyLocalOneByOne()
    {
        // 初始化服务端
        var serverName = "JsonIpcDirectRoutedProviderTest_Test_NotifyLocalOneByOne_1";
        var serverProvider = new JsonIpcDirectRoutedProvider(serverName, new IpcConfiguration() { IpcTaskScheduling = IpcTaskScheduling.LocalOneByOne, });

        var count = 0;
        for (int i = 0; i < 10; i++)
        {
            var n = i;
            serverProvider.AddNotifyHandler($"Foo{n}", async () =>
            {
                // 如果是按照顺序进来的，那就是按照数字顺序
                Assert.AreEqual(n, count);
                count++;
                // 模拟异步处理
                await Task.Delay(100);
            });
        }

        serverProvider.StartServer();

        // 创建客户端
        JsonIpcDirectRoutedProvider clientProvider = new();
        clientProvider.StartServer();
        var clientProxy = await clientProvider.GetAndConnectClientAsync(serverName);

        // 发送 10 条通知
        for (int i = 0; i < 10; i++)
        {
            await clientProxy.NotifyAsync($"Foo{i}");
        }

        // 等待接收完成，以上的 await 返回仅仅只是发送出去，不代表对方已接收到
        for (int i = 0; i < 1000 && count != 10; i++)
        {
            await Task.Delay(100);
        }
    }

    /// <summary>
    /// 测试无参版本
    /// </summary>
    [TestMethod("发送无参请求，可以让服务端收到请求")]
    public async Task TestParameterless()
    {
        // 初始化服务端
        var serverName = "JsonIpcDirectRoutedProviderTest_Test_Parameterless_1";
        var serverProvider = new JsonIpcDirectRoutedProvider(serverName, TestJsonContext.CreateIpcConfiguration());
        var routedPath = "Foo1";

        // 注册无参数请求处理
        serverProvider.AddRequestHandler(routedPath, () =>
        {
            return new FakeResult(nameof(TestParameterless));
        });

        serverProvider.StartServer();

        // 创建客户端
        JsonIpcDirectRoutedProvider clientProvider = new(ipcConfiguration: TestJsonContext.CreateIpcConfiguration());
        clientProvider.StartServer();
        var clientProxy = await clientProvider.GetAndConnectClientAsync(serverName);

        // 请求无参数
        var result = await clientProxy.GetResponseAsync<FakeResult>(routedPath);

        // 如果能收到服务端返回值，证明请求成功
        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(TestParameterless), result.Name);
    }

    [TestMethod("发送无参通知，可以让服务端收到通知")]
    public async Task TestParameterless2()
    {
        // 初始化服务端
        var serverName = "JsonIpcDirectRoutedProviderTest_Test_Parameterless_2";
        var serverProvider = new JsonIpcDirectRoutedProvider(serverName);
        var routedPath = "Foo1";

        var taskCompletionSource = new TaskCompletionSource();
        serverProvider.AddNotifyHandler(routedPath, () =>
        {
            taskCompletionSource.SetResult();
        });

        serverProvider.StartServer();

        // 创建客户端
        JsonIpcDirectRoutedProvider clientProvider = new();
        clientProvider.StartServer();
        var clientProxy = await clientProvider.GetAndConnectClientAsync(serverName);
        await clientProxy.NotifyAsync(routedPath);

        // 再等待一下，让服务端处理完成
        await taskCompletionSource.Task.WaitTimeout(TimeSpan.FromSeconds(1));
        // 预期是服务端能够执行完成
        Assert.AreEqual(true, taskCompletionSource.Task.IsCompleted);
    }

    [TestMethod("发送无参请求，服务端订阅有参，依然可以让服务端收到请求")]
    public async Task TestParameterless3()
    {
        // 可能是版本兼容，一个客户端版本软件是旧版本发送时不带参数，后续的服务端新版本写了带参数处理
        // 期望这样的情况服务端依然能够收到请求，达成兼容
        // 初始化服务端
        var serverName = "JsonIpcDirectRoutedProviderTest_Test_Parameterless_3";
        var serverProvider = new JsonIpcDirectRoutedProvider(serverName, TestJsonContext.CreateIpcConfiguration());
        var routedPath = "Foo1";

        // 服务端订阅有参
        serverProvider.AddRequestHandler(routedPath, (FakeArgument arg) =>
        {
            Assert.IsNotNull(arg);

            return new FakeResult(nameof(TestParameterless));
        });

        serverProvider.StartServer();

        // 创建客户端
        JsonIpcDirectRoutedProvider clientProvider = new(ipcConfiguration: TestJsonContext.CreateIpcConfiguration());
        clientProvider.StartServer();
        var clientProxy = await clientProvider.GetAndConnectClientAsync(serverName);

        // 请求无参数
        var result = await clientProxy.GetResponseAsync<FakeResult>(routedPath);

        // 如果能收到服务端返回值，证明请求成功
        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(TestParameterless), result.Name);
    }

    [TestMethod("客户端发送有参请求，服务端订阅无参，依然可以让服务端收到请求")]
    public async Task TestParameterless4()
    {
        var serverName = "JsonIpcDirectRoutedProviderTest_Test_Parameterless_4";
        var serverProvider = new JsonIpcDirectRoutedProvider(serverName, TestJsonContext.CreateIpcConfiguration());
        var routedPath = "Foo1";

        // 服务端订阅无参
        serverProvider.AddRequestHandler(routedPath, () =>
        {
            return new FakeResult(nameof(TestParameterless));
        });

        serverProvider.StartServer();

        // 创建客户端
        JsonIpcDirectRoutedProvider clientProvider = new(ipcConfiguration: TestJsonContext.CreateIpcConfiguration());
        clientProvider.StartServer();
        var clientProxy = await clientProvider.GetAndConnectClientAsync(serverName);

        // 发送有参请求
        var result = await clientProxy.GetResponseAsync<FakeResult>(routedPath, new FakeArgument("foo", 2));

        // 如果能收到服务端返回值，证明请求成功
        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(TestParameterless), result.Name);
    }

    internal record class FakeArgument(string Name, int Count)
    {
    }

    internal record class FakeResult(string Name);

    private class FooException1 : Exception
    {
        public FooException1(string? message) : base(message)
        {
        }
    }
}
