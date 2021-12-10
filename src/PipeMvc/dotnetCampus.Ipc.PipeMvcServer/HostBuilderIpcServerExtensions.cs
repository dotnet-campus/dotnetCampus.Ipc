// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net.Http;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace dotnetCampus.Ipc.PipeMvcServer
{
    /// <summary>
    /// Contains extensions for retrieving properties from <see cref="IHost"/>.
    /// </summary>
    /// Copy from https://github.com/dotnet/aspnetcore/blob/a450cb69b5e4549f5515cdb057a68771f56cefd7/src/Hosting/TestHost/src/HostBuilderTestServerExtensions.cs
    public static class HostBuilderIpcServerExtensions
    {
        /// <summary>
        /// Retrieves the IpcServer from the host services.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IpcServer GetTestServer(this IHost host)
        {
            return (IpcServer)host.Services.GetRequiredService<IServer>();
        }

        /// <summary>
        /// Retrieves the test client from the IpcServer in the host services.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static HttpClient GetTestClient(this IHost host)
        {
            return host.GetTestServer().CreateClient();
        }
    }
}
