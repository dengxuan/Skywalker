// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Security.Claims;
using Skywalker.Security.Clients;
using Skywalker.Security.Users;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Security 服务扩展方法。
/// </summary>
public static class SecurityIServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Security 模块服务到服务集合。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        services.TryAddTransient<ISkywalkerClaimsPrincipalFactory, SkywalkerClaimsPrincipalFactory>();
        services.TryAddSingleton<ICurrentPrincipalAccessor, ThreadCurrentPrincipalAccessor>();
        services.TryAddTransient<ICurrentClient, CurrentClient>();
        services.TryAddTransient<ICurrentUser, CurrentUser>();
        return services;
    }
}
