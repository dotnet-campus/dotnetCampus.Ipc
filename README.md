# dotnetCampus.Ipc

本机内多进程通讯库

| Build | NuGet |
|--|--|
|![](https://github.com/dotnet-campus/dotnetCampus.Ipc/workflows/.NET%20Core/badge.svg)|[![](https://img.shields.io/nuget/v/dotnetCampus.Ipc.svg)](https://www.nuget.org/packages/dotnetCampus.Ipc)|

项目可用状态

打磨中

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

## API 特点

### 设计特点

底层每次调用需要传入 `string summary` 用于标识

优势在于解决调试的时候，看到传送的二进制以及调用堆栈的时候，如何对应上具体的业务方的问题

整个库编写过程中注意了给参数加上单位，不会存在基础类型的传入。例如给 ulong 的 Ack 特别定义了结构体等

## 进度

- 基本完成 dotnetCampus.Ipc 的 API 定义和功能实现
- 完成 客户端服务器端模型
- 完成 P2P 式模型
- 完成远程调用的实现
- 完成 最小可用呆魔，支持主动和被动连接，支持通讯发送文本
- 完成断线重连
- 完成性能优化，包括内存优化
- 完成 MVC 模式

- [ ] 接入预编译提供上层的远程调用封装

## 感谢

- [jacqueskang/IpcServiceFramework](https://github.com/jacqueskang/IpcServiceFramework)
- [https://github.com/dotnet/aspnetcore](https://github.com/dotnet/aspnetcore) for PipeMVC

## 踩过的坑

- [2019-12-1-构造PipeAccessRule时请不要使用字符串指定Identity - huangtengxiao](https://huangtengxiao.gitee.io/post/%E6%9E%84%E9%80%A0PipeAccessRule%E6%97%B6%E8%AF%B7%E4%B8%8D%E8%A6%81%E4%BD%BF%E7%94%A8%E5%AD%97%E7%AC%A6%E4%B8%B2%E6%8C%87%E5%AE%9AIdentity.html)