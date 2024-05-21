# dotnetCampus.Ipc

本机内多进程通讯库

| Build | NuGet |
|--|--|
|![](https://github.com/dotnet-campus/dotnetCampus.Ipc/workflows/.NET%20Core/badge.svg)|[![](https://img.shields.io/nuget/v/dotnetCampus.Ipc.svg)](https://www.nuget.org/packages/dotnetCampus.Ipc)|

项目级可用状态

## 特点

- 采用两个半工命名管道
- 采用 P2P 方式，每个端都是服务端也是客户端
- ~~加入消息 Ack 机制，弱化版，因为管道形式只要能写入就是成功 Ack 了~~
- 提供 PeerProxy 机制，利用这个机制可以进行发送和接收某个对方的信息
- 追求稳定，而不追求高性能

## 功能

- [x] 通讯建立
- [x] 消息收到回复机制
- [x] 断线重连功能
- [x] 大量异常处理

- [x] 支持裸数据双向传输方式
- [x] 支持裸数据请求响应模式
- [x] 支持字符串消息协议
- [x] 支持远程对象调用和对象存根传输方式
- [x] 支持 NamedPipeStreamForMvc (NamedPipeMvc) 客户端服务器端 MVC 模式

- [x] .NET Framework 4.5
- [x] .NET Core 3.1

<!-- ## 项目设计

### dotnetCampus.Ipc.Abstractions

提供可替换的抽象实现，只是有接口等

进度：等待设计 API 中，设计的时候需要考虑不同的底层技术需要的支持

应该分为两层的 API 实现，第一层为底层 API 用于给各个底层基础项目所使用。第二层是顶层 API 用于给上层业务开发者使用

### dotnetCampus.Ipc.PipeCore

提供对 dotnetCampus.Ipc.Abstractions 的部分实现

使用管道制作的 IPC 通讯

不直接面向最终开发者，或者说只有很少的类会被开发者使用到，这个项目的 API 不做设计，注重的是提供稳定的管道进程间通讯方式

特点是有很多很底层的 API 开放，以及用起来的时候需要了解管道的知识

进度：基本可用

优先实现管道，是因为管道又快有稳。但是缺点是不支持 Linux 下使用，同时稳定的管道需要设计为两个半工的管道

### dotnetCampus.Ipc

提供给上层业务开发者使用的项目，这个项目有良好的 API 设计

也许只有在初始化的时候，才需要用到少量的 dotnetCampus.Ipc.PipeCore 项目里面管道的知识

这个项目能支持的不应该只有管道一个方式，而是任何基于 dotnetCampus.Ipc.Abstractions 的抽象实现都应该支持

进度：等待 API 设计中，也许会接入 [https://github.com/jacqueskang/IpcServiceFramework](https://github.com/jacqueskang/IpcServiceFramework) 的实现，或者模拟 WCF 或 Remoting 的实现 -->

## 进度

- 基本完成 dotnetCampus.Ipc 的 API 定义和功能实现
- 完成 客户端服务器端模型
- 完成 P2P 式模型
- 完成远程调用的实现
- 完成 最小可用呆魔，支持主动和被动连接，支持通讯发送文本
- 完成断线重连
- 完成性能优化，包括内存优化
- 完成 MVC 模式
- 完成接入预编译提供上层的远程调用封装

## Usage

库中提供了较为底层的通信方案，也提供了高级的封装方案（基于Json数据格式的通信方案），完整文档可参阅：

- [使用 .NET Remoting 模式的对象远程调用的 IPC 通讯方式](https://github.com/dotnet-campus/dotnetCampus.Ipc/blob/master/docs/IpcRemotingObject.md)
- [使用直接路由和 Json 通讯格式的 IPC 通讯方式](https://github.com/dotnet-campus/dotnetCampus.Ipc/blob/master/docs/JsonIpcDirectRouted.md)

### 案例：Json通信（需要2.0.0-alpha版本以上）

#### 步骤一

导入nuget包 **dotnetCampus.Ipc**（需要2.0.0-alpha版本以上），并引入所需要的命名空间；

``` C#
using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.IpcRouteds.DirectRouteds;
using dotnetCampus.Ipc.Pipes;
```

#### 步骤二

创建实际负责IPC通信的代理对象

``` C#

/// <summary>
/// 根据<paramref name="pipeName"/>创建一个 JsonIpcDirectRoutedProvider 对象
/// </summary>
/// <param name="pipeName">不同的IPC对象所使用的管道名称，一个管道名称只能被用于一个IPC对象</param>
/// <returns></returns>
private JsonIpcDirectRoutedProvider CreateJsonIpcDirectRoutedProvider(string pipeName)
{
    // 创建一个 IpcProvider，实际创建管道，进行IPC通信的底层对象
    var ipcProvider = new IpcProvider(pipeName, new IpcConfiguration());

    // 创建一个 JsonIpcDirectRoutedProvider，封装了通信中的Json数据解析、简化方法调用
    var ipcDirectRoutedProvider = new JsonIpcDirectRoutedProvider(ipcProvider);

    return ipcDirectRoutedProvider;
}

```

#### 步骤三

向IPC对象注册接受到指定消息后的处理函数（如果该IPC对象只负责发送消息，则它不需要注册消息处理回调）

``` C#

var ipcDirectRoutedProvider = CreateJsonIpcDirectRoutedProvider("我是接收消息的IPC对象");

//对无参的通知消息注册回调函数
ipcDirectRoutedProvider.AddNotifyHandler("通知消息A", () => {
    Console.WriteLine("我是进程A，我收到了通知消息B，该消息无参数");
});

//对参数类型为ParamType的通知消息注册回调函数
ipcDirectRoutedProvider.AddNotifyHandler<ParamType>("通知消息B", param => {
    Console.WriteLine($"我是进程A，我收到了通知消息B，该消息参数：{param.Message}");
});

//对参数类型为ParamType的请求注册回调函数并返回响应数据（可以异步处理响应、也可以无参）
ipcDirectRoutedProvider.AddRequestHandler("请求消息C", (ParamType argument) =>
{
    //处理请求消息C
    var response = new IpcResponse
    {
        Message = $"我是进程A，我收到了请求消息C，该消息参数：{argument.Message}"
    };

    //返回响应数据
    return response;
});

```

#### 步骤四

启动服务

``` C#

var ipcDirectRoutedProvider = CreateJsonIpcDirectRoutedProvider("我是接收消息的IPC对象");

/**
一些消息注册（如果该IPC对象只负责发送消息，则它不需要注册消息处理回调；接受消息的一方需要注册接收到消息后的处理函数）
……
**/

//启动该服务
ipcDirectRoutedProvider.StartServer();

```

#### 步骤五

发送消息（如果该IPC对象只负责接收和处理消息，则它不需要发送消息）

``` C#

var ipcDirectRoutedProvider = CreateJsonIpcDirectRoutedProvider("我是发送消息的IPC对象");
//启动该服务
ipcDirectRoutedProvider.StartServer();
//根据接收方的管道名，获取需要接受到消息的IPC对象，并发送通知
var ipcReceivingObjectA = await ipcDirectRoutedProvider.GetAndConnectClientAsync("我是接收消息的IPC对象");
await ipcReceivingObjectA.NotifyAsync("通知消息A");
await ipcReceivingObjectA.NotifyAsync("通知消息B", new ParamType { Message = "我发送的通知消息是XXX" });
var response = await ipcReceivingObjectA.GetResponseAsync<IpcResponse>("请求消息C", new ParamType { Message = "我发送的请求消息XXX" });

```

*更多案例详见：* [Demo](https://github.com/dotnet-campus/dotnetCampus.Ipc/tree/master/demo)

## 感谢

- [jacqueskang/IpcServiceFramework](https://github.com/jacqueskang/IpcServiceFramework)
- [https://github.com/dotnet/aspnetcore](https://github.com/dotnet/aspnetcore) for PipeMVC

## 踩过的坑

- [2019-12-1-构造PipeAccessRule时请不要使用字符串指定Identity - huangtengxiao](https://huangtengxiao.gitee.io/post/%E6%9E%84%E9%80%A0PipeAccessRule%E6%97%B6%E8%AF%B7%E4%B8%8D%E8%A6%81%E4%BD%BF%E7%94%A8%E5%AD%97%E7%AC%A6%E4%B8%B2%E6%8C%87%E5%AE%9AIdentity.html)