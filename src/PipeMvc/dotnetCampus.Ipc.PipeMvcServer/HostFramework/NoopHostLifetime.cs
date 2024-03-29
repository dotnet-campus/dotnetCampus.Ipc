﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Copy From: https://github.com/dotnet/aspnetcore

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace dotnetCampus.Ipc.PipeMvcServer.HostFramework
{
    internal class NoopHostLifetime : IHostLifetime
    {
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
