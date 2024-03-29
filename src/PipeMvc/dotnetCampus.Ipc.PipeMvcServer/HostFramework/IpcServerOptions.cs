﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using dotnetCampus.Ipc.PipeMvcServer.IpcFramework;

using Microsoft.AspNetCore.Http;

namespace dotnetCampus.Ipc.PipeMvcServer.HostFramework
{
    /// <summary>
    /// Options for the test server.
    /// </summary>
    public class IpcServerOptions
    {
        /// <summary>
        /// 获取或设置 Ipc 服务的管道名。如为空将默认使用随机的命名
        /// </summary>
        public string? IpcPipeName { set; get; }

        /// <summary>
        /// Gets a value that controls whether synchronous IO is allowed for the <see cref="HttpContext.Request"/> and <see cref="HttpContext.Response"/>. The default value is <see langword="false" />.
        /// </summary>
        public bool AllowSynchronousIO => false;

        /// <summary>
        /// Gets or sets a value that controls if <see cref="ExecutionContext"/> and <see cref="AsyncLocal{T}"/> values are preserved from the client to the server. The default value is <see langword="false" />.
        /// </summary>
        public bool PreserveExecutionContext { get; set; }

        /// <summary>
        /// Gets or sets the base address associated with the HttpClient returned by the test server. Defaults to http://localhost/ 也就是 <see cref="IpcPipeMvcContext.BaseAddressUrl"/> 的值.
        /// </summary>
        public Uri BaseAddress { get; set; } = new Uri(IpcPipeMvcContext.BaseAddressUrl);
    }
}
