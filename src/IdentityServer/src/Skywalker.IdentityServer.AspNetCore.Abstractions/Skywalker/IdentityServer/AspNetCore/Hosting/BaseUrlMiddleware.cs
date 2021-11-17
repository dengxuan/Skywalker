// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Skywalker.IdentityServer.AspNetCore.Configuration.DependencyInjection.Options;
using Skywalker.IdentityServer.AspNetCore.Extensions;

#pragma warning disable 1591

namespace Skywalker.IdentityServer.AspNetCore.Hosting
{
    public class BaseUrlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IdentityServerOptions _options;

        public BaseUrlMiddleware(RequestDelegate next, IdentityServerOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;

            context.SetIdentityServerBasePath(request.PathBase.Value.RemoveTrailingSlash());

            await _next(context);
        }
    }
}