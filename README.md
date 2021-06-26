# dotnetCampus.Ipc

本机内多进程通讯库

| Build | NuGet |
|--|--|
|![](https://github.com/dotnet-campus/dotnetCampus.Ipc/workflows/.NET%20Core/badge.svg)|[![](https://img.shields.io/nuget/v/dotnetCampus.Ipc.svg)](https://www.nuget.org/packages/dotnetCampus.Ipc)|

开发中……

大概是基础可用

## 特点

- 采用两个半工命名管道
- 采用 P2P 方式，每个端都是服务端也是客户端
- 加入消息 Ack 机制，弱化版，因为管道形式只要能写入就是成功 Ack 了
- 提供 PeerProxy 机制，利用这个机制可以进行发送和接收某个对方的信息
- 追求稳定，而不追求高性能

## 功能

- [x] 通讯建立
- [x] 消息收到回复机制
- [x] 断线重连功能
- [ ] 大量异常处理

- [x] .NET Framework 4.5
- [x] .NET Core 3.1

## 项目设计

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

进度：等待 API 设计中，也许会接入 [https://github.com/jacqueskang/IpcServiceFramework](https://github.com/jacqueskang/IpcServiceFramework) 的实现，或者模拟 WCF 或 Remoting 的实现

## API 特点

### 设计特点

底层每次调用需要传入 `string summary` 用于标识

优势在于解决调试的时候，看到传送的二进制以及调用堆栈的时候，如何对应上具体的业务方的问题

整个库编写过程中注意了给参数加上单位，不会存在基础类型的传入。例如给 ulong 的 Ack 特别定义了结构体等

## 进度

- 基本完成 dotnetCampus.Ipc.PipeCore 部分
- 基本完成 客户端服务器端模型
- 完成 最小可用呆魔，支持主动和被动连接，支持通讯发送文本

- [ ] 顶层调用 API 设计

- [ ] 远程调用的实现
- [ ] 性能优化，包括内存优化
- [ ] 接入预编译提供上层的远程调用封装


## 不支持的列表

也许后续会支持

### 传入实例类

在 dotnet core 的 DispatchProxy 只支持代理

### 使用属性

因为不能使用同步方法，因此属性都不能使用

### 同步的方法

所有方法都需要是异步的

### 返回值不是 Task 或 Task 泛形或框架封装的类型

框架不知道业务层的返回类型，因此无法封装返回值

### 返回值包含两层 Task 如 `Task<Task<int>>` 类型

框架无法封装和序列化传递

### 事件和方法传入委托

无法序列化由另一个进程调用

### 框架主动通知断开

现在的设计不确定是否可以在框架进行通知，依然是在业务端进行通知

### 发送消息自动等待链接

如果链接断开，此时发送将会失败，是否可以设计为发送时，如果对方断开，自动等待对方再次连接上之后，自动发送数据

## 感谢

[jacqueskang/IpcServiceFramework](https://github.com/jacqueskang/IpcServiceFramework)
