// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Builder;
using Skywalker.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class PermissionsIApplicationBuilderExtensions
{
    public static IApplicationBuilder UsePermissions(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<PermissionsMiddleware>();
        return builder;
    }
}
