// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.SimpleStateChecking;
using Skywalker.Permissions;
using Skywalker.Permissions.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Skywalker 权限模块服务注册扩展
/// </summary>
public static class SkywalkerPermissionsServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Permissions 模块服务到服务集合。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddPermissions(this IServiceCollection services)
    {
        // 注册 MemoryCache（InMemoryPermissionValidator 依赖）
        services.AddMemoryCache();

        // 注册 SimpleStateCheckerManager<PermissionDefinition>
        services.TryAddSingleton<ISimpleStateCheckerManager<PermissionDefinition>, SimpleStateCheckerManager<PermissionDefinition>>();

        services.TryAddSingleton<IPermissionDefinitionManager, DefaultPermissionDefinitionManager>();
        services.TryAddSingleton<IPermissionValidator, InMemoryPermissionValidator>();
        services.TryAddTransient<IPermissionChecker, PermissionChecker>();
        services.TryAddSingleton<IPermissionValueProviderManager, PermissionValueProviderManager>();
        return services;
    }
}
