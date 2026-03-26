// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Permissions.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 权限服务端 DI 扩展
/// </summary>
public static class PermissionServerServiceCollectionExtensions
{
    /// <summary>
    /// 添加权限服务端（使用空实现）
    /// </summary>
    public static IServiceCollection AddPermissionsServer(
        this IServiceCollection services,
        Action<PermissionServerOptions>? configure = null)
    {
        return services.AddPermissionsServer<NullPermissionGrantStore>(configure);
    }

    /// <summary>
    /// 添加权限服务端
    /// </summary>
    /// <typeparam name="TGrantStore">权限授权数据存储实现（用于 HTTP 端点提供数据）</typeparam>
    public static IServiceCollection AddPermissionsServer<TGrantStore>(
        this IServiceCollection services,
        Action<PermissionServerOptions>? configure = null)
        where TGrantStore : class, IPermissionGrantStore
    {
        if (configure != null)
        {
            services.Configure(configure);
        }

        // 注册数据存储（用于提供 HTTP 端点数据）
        services.AddSingleton<IPermissionGrantStore, TGrantStore>();

        return services;
    }
}
