﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Copy From: https://github.com/dotnet/aspnetcore

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace dotnetCampus.Ipc.PipeMvcServer.HostFramework
{
    internal class ResponseTrailersFeature : IHttpResponseTrailersFeature
    {
        public IHeaderDictionary Trailers { get; set; } = new HeaderDictionary();
    }
}
