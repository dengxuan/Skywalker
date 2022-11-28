// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Security.Claims;
using Skywalker.Security.Clients;
using Skywalker.Security.Users;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static  class SecurityIServiceCollectionExtensions
{
    public static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        services.TryAddTransient<ICurrentUser, CurrentUser>();
        services.TryAddTransient<ICurrentClient, CurrentClient>();
        services.TryAddTransient<ISkywalkerClaimsPrincipalFactory, SkywalkerClaimsPrincipalFactory>();

        services.TryAddSingleton<ICurrentPrincipalAccessor,ThreadCurrentPrincipalAccessor>();
        return services;
    }
}
