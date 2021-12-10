// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            => builder.UsePipeIpcServer(option => option.IpcPipeName = ipcPipeName);

        /// <summary>
        /// Enables the <see cref="IpcServer" /> service.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/>.</param>
        /// <param name="configureOptions">Configures test server options</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UsePipeIpcServer(this IWebHostBuilder builder, Action<IpcServerOptions>? configureOptions = null)
        {
            return builder.ConfigureServices(services =>
            {
                if (configureOptions is not null)
                {
                    services.Configure(configureOptions);
                }
                services.AddSingleton<IHostLifetime, NoopHostLifetime>();
                services.AddSingleton<IServer, IpcServer>();

                services.AddSingleton<IpcCore>();
            });
        }
    }
}
