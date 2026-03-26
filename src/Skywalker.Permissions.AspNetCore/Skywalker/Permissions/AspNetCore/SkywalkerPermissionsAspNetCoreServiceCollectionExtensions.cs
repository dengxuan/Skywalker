// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Permissions.AspNetCore;

/// <summary>
/// Skywalker 权限认证的 DI 扩展
/// </summary>
public static class SkywalkerPermissionsAspNetCoreServiceCollectionExtensions
{
    /// <summary>
    /// 注册权限策略提供者与处理器
    /// </summary>
    public static IServiceCollection AddSkywalkerPermissionAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        return services;
    }
}
