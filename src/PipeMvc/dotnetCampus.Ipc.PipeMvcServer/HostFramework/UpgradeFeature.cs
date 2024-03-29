﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Copy From: https://github.com/dotnet/aspnetcore

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;

namespace dotnetCampus.Ipc.PipeMvcServer.HostFramework
{
    internal class UpgradeFeature : IHttpUpgradeFeature
    {
        public bool IsUpgradableRequest => false;

        // TestHost provides an IHttpWebSocketFeature so it wont call UpgradeAsync()
        public Task<Stream> UpgradeAsync()
        {
            throw new NotSupportedException();
        }
    }
}
