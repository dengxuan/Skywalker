// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.VirtualFileSystem;


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// VirtualFileSystem 服务扩展方法。
/// </summary>
public static class VirtualFileSystemIServiceCollectionExtensions
{
    public static IServiceCollection AddVirtualFileSystem(this IServiceCollection services)
    {
        services.AddAutoServices();
        return services;
    }
}
