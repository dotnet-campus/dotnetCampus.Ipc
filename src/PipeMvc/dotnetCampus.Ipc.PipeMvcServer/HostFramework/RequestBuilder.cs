﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Copy From: https://github.com/dotnet/aspnetcore

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace dotnetCampus.Ipc.PipeMvcServer.HostFramework
{
    /// <summary>
    /// Used to construct a HttpRequestMessage object.
    /// </summary>
    internal class RequestBuilder
    {
        private readonly HttpRequestMessage _req;

        /// <summary>
        /// Construct a new HttpRequestMessage with the given path.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="path"></param>
        public RequestBuilder(IpcServer server, string path)
        {
            IpcServer = server ?? throw new ArgumentNullException(nameof(server));
            _req = new HttpRequestMessage(HttpMethod.Get, path);
        }

        /// <summary>
        /// Gets the <see cref="IpcServer"/> instance for which the request is being built.
        /// </summary>
        public IpcServer IpcServer { get; }

        /// <summary>
        /// Configure any HttpRequestMessage properties.
        /// </summary>
        /// <param name="configure"></param>
        /// <returns>This <see cref="RequestBuilder"/> for chaining.</returns>
        public RequestBuilder And(Action<HttpRequestMessage> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            configure(_req);
            return this;
        }

        /// <summary>
        /// Add the given header and value to the request or request content.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns>This <see cref="RequestBuilder"/> for chaining.</returns>
        public RequestBuilder AddHeader(string name, string value)
        {
            if (!_req.Headers.TryAddWithoutValidation(name, value))
            {
                if (_req.Content == null)
                {
                    _req.Content = new StreamContent(Stream.Null);
                }
                if (!_req.Content.Headers.TryAddWithoutValidation(name, value))
                {
                    // TODO: throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidHeaderName, name), "name");
                    throw new ArgumentException("Invalid header name: " + name, nameof(name));
                }
            }
            return this;
        }

        /// <summary>
        /// Set the request method and start processing the request.
        /// </summary>
        /// <param name="method"></param>
        /// <returns>The resulting <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> SendAsync(string method)
        {
            _req.Method = new HttpMethod(method);
            return IpcServer.CreateClient().SendAsync(_req);
        }

        /// <summary>
        /// Set the request method to GET and start processing the request.
        /// </summary>
        /// <returns>The resulting <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> GetAsync()
        {
            _req.Method = HttpMethod.Get;
            return IpcServer.CreateClient().SendAsync(_req);
        }

        /// <summary>
        /// Set the request method to POST and start processing the request.
        /// </summary>
        /// <returns>The resulting <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> PostAsync()
        {
            _req.Method = HttpMethod.Post;
            return IpcServer.CreateClient().SendAsync(_req);
        }
    }
}
