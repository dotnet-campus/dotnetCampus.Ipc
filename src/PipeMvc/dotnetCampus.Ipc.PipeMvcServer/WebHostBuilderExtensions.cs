// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

using dotnetCampus.Ipc.PipeMvcServer.HostFramework;
using dotnetCampus.Ipc.PipeMvcServer.IpcFramework;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace dotnetCampus.Ipc.PipeMvcServer
{
    /// <summary>
    /// Contains extensions for configuring the <see cref="IWebHostBuilder" /> instance.
    /// </summary>
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Enables the <see cref="IpcServer" /> service.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/>.</param>
        /// <param name="ipcPipeName">设置 Ipc 服务的管道名</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UsePipeIpcServer(this IWebHostBuilder builder, string ipcPipeName)
        {
            return builder.ConfigureServices(services =>
            {
                services.AddSingleton<IHostLifetime, NoopHostLifetime>();
                services.AddSingleton<IServer, IpcServer>();

                services.AddSingleton<IpcCore>(s => new IpcCore(s, ipcPipeName));
            });
        }
    }
}
