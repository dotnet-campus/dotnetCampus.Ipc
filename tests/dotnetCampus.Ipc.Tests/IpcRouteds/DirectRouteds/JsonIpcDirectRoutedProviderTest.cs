using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using dotnetCampus.Ipc.IpcRouteds.DirectRouteds;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Tests.CompilerServices;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MSTest.Extensions.Contracts;

namespace dotnetCampus.Ipc.Tests.IpcRouteds.DirectRouteds;

[TestClass]
public class JsonIpcDirectRoutedProviderTest
{
    [ContractTestCase]
    public void TestShared()
    {
        "多个通讯框架共用相同的 IpcProvider 对象，相互之间不受影响".Test(async () =>
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
        });
    }

    [ContractTestCase]
    public void TestRequest()
    {
        "允许创建多个服务端实例共用相同的 IpcProvider 对象，但是如果多个服务端对相同的路由进行处理，只有先添加的才能收到消息".Test(async () =>
        {
            // 初始化服务端
            var serverName = "JsonIpcDirectRoutedProviderTest_Notify_3";
            // 这样的创建方式也是对 IPC 连接最高定制的方式
            IpcProvider ipcProvider = new(serverName);
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
            JsonIpcDirectRoutedProvider clientProvider = new();
            var clientProxy = await clientProvider.GetAndConnectClientAsync(serverName);

            var result = await clientProxy.GetResponseAsync<FakeResult>("Foo1", argument);

            // 可以获取到响应内容
            Assert.AreEqual(responseText, result.Name);

            // 要求只进入一次
            Assert.AreEqual(1, enterCount);
        });

        "客户端请求服务端，可以在服务端收到客户端请求的内容".Test(async () =>
        {
            // 初始化服务端
            var serverName = "JsonIpcDirectRoutedProviderTest_Request_1";
            var serverProvider = new JsonIpcDirectRoutedProvider(serverName);
            var argument = new FakeArgument("TestName", 1);

            var responseText = Guid.NewGuid().ToString();

            int enterCount = 0;
            serverProvider.AddRequestHandler("Foo1", (FakeArgument arg) =>
            {
                // 没有任何逻辑请求，不能处理
                enterCount++;
                Assert.AreEqual(argument.Name, arg.Name);
                Assert.AreEqual(argument.Count, arg.Count);

                return new FakeResult("Ok");
            });

            serverProvider.AddRequestHandler("Foo2", (FakeArgument arg, JsonIpcDirectRoutedContext context) =>
            {
                // 没有任何逻辑请求，不能处理
                Assert.Fail();
                return new FakeResult("Ok");
            });

            serverProvider.StartServer();

            // 创建客户端
            // 允许无参数，如果只是做客户端使用的话
            JsonIpcDirectRoutedProvider clientProvider = new();
            // 对于 clientProvider 来说，可选调用 StartServer 方法
            var clientProxy = await clientProvider.GetAndConnectClientAsync(serverName);

            var result = await clientProxy.GetResponseAsync<FakeResult>("Foo1", argument);

            // 可以获取到响应内容
            Assert.AreEqual("Ok", result.Name);

            // 要求只进入一次
            Assert.AreEqual(1, enterCount);
        });

        "客户端到服务端的请求，可以获取到服务端的响应".Test(async () =>
        {
            // 初始化服务端
            var serverName = "JsonIpcDirectRoutedProviderTest_Request_2";
            var clientName = "JsonIpcDirectRoutedProviderTest_Request_Client_1";
            var serverProvider = new JsonIpcDirectRoutedProvider(serverName);
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
            JsonIpcDirectRoutedProvider clientProvider = new(clientName);
            // 对于 clientProvider 来说，可选调用 StartServer 方法
            var clientProxy = await clientProvider.GetAndConnectClientAsync(serverName);

            var result = await clientProxy.GetResponseAsync<FakeResult>("Foo2", argument);

            // 可以获取到响应内容
            Assert.AreEqual(responseText, result.Name);

            // 要求只进入一次
            Assert.AreEqual(1, enterCount);
        });
    }

    [ContractTestCase]
    public void AddNotifyHandler()
    {
        "重复调用 JsonIpcDirectRoutedProvider 添加通知处理相同的消息，将会抛出异常".Test(() =>
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
        });
    }

    [ContractTestCase]
    public void AddRequestHandler()
    {
        "重复调用 AddRequestHandler 添加请求处理相同的消息，将会抛出异常".Test(() =>
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
        });
    }

    [ContractTestCase]
    public void TestNotify()
    {
        "允许创建多个服务端实例共用相同的 IpcProvider 对象，从而每个服务端接收不同的通知".Test(async () =>
        {
            // 初始化服务端
            var serverName = "JsonIpcDirectRoutedProviderTest_Notify_3";
            // 这样的创建方式也是对 IPC 连接最高定制的方式
            IpcProvider ipcProvider = new(serverName);
            var serverProvider1 = new JsonIpcDirectRoutedProvider(ipcProvider);
            var argument = new FakeArgument("TestName", 1);
            int enterCount = 0;
            TaskCompletionSource taskCompletionSource = new();

            serverProvider1.AddNotifyHandler("Foo1", (FakeArgument arg) =>
            {
                enterCount++;
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
                enterCount++;
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
            JsonIpcDirectRoutedProvider clientProvider = new();
            var clientProxy = await clientProvider.GetAndConnectClientAsync(serverName);
            await clientProxy.NotifyAsync("Foo1", argument);
            // 预期这条消息是在第二个服务处理的
            await clientProxy.NotifyAsync("Foo2", argument);

            // 等待接收完成，以上的 await 返回仅仅只是发送出去，不代表对方已接收到
            await taskCompletionSource.Task.WaitTimeout(TimeSpan.FromSeconds(5));
            // 两个服务都进入
            Assert.AreEqual(2, enterCount);
        });

        "从客户端通知到服务端，可以在服务端获取到通知的客户端名".Test(async () =>
        {
            // 初始化服务端
            var serverName = "JsonIpcDirectRoutedProviderTest_Notify_2";
            var clientName = "JsonIpcDirectRoutedProviderTest_Notify_Client_1";
            var serverProvider = new JsonIpcDirectRoutedProvider(serverName);
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
            JsonIpcDirectRoutedProvider clientProvider = new(clientName);
            // 对于 clientProvider 来说，可选调用 StartServer 方法
            var clientProxy = await clientProvider.GetAndConnectClientAsync(serverName);

            await clientProxy.NotifyAsync("Foo2", argument);

            // 等待接收完成，以上的 await 返回仅仅只是发送出去，不代表对方已接收到
            await taskCompletionSource.Task.WaitTimeout(TimeSpan.FromSeconds(5));
            // 要求只进入一次
            Assert.AreEqual(1, enterCount);
        });

        "客户端的通知可以成功发送到服务端".Test(async () =>
        {
            // 初始化服务端
            var serverName = "JsonIpcDirectRoutedProviderTest_Notify_1";
            var serverProvider = new JsonIpcDirectRoutedProvider(serverName);
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
            JsonIpcDirectRoutedProvider clientProvider = new();
            clientProvider.StartServer();
            // todo 命名：这里叫 ClientProxy 正确还是 ServerProxy 正确
            var clientProxy = await clientProvider.GetAndConnectClientAsync(serverName);

            await clientProxy.NotifyAsync(routedPath, argument);

            // 等待接收完成，以上的 await 返回仅仅只是发送出去，不代表对方已接收到
            await taskCompletionSource.Task.WaitTimeout(TimeSpan.FromSeconds(5));
            // 要求只进入一次
            Assert.AreEqual(1, enterCount);
        });
    }

    record class FakeArgument(string Name, int Count)
    {
    }

    record class FakeResult(string Name);
}
