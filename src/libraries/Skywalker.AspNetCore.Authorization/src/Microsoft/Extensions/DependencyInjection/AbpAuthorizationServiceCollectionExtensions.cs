﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.AspNetCore.Authorization;
using Skywalker.AspNetCore.Authorization.Permissions;

namespace Microsoft.Extensions.DependencyInjection;

public static class AbpAuthorizationServiceCollectionExtensions
{
    public static IServiceCollection AddAlwaysAllowAuthorization(this IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Singleton<IAuthorizationService, AlwaysAllowAuthorizationService>());
        services.Replace(ServiceDescriptor.Singleton<ISkywalkerAuthorizationService, AlwaysAllowAuthorizationService>());
        services.Replace(ServiceDescriptor.Singleton<IMethodInvocationAuthorizationService, AlwaysAllowMethodInvocationAuthorizationService>());
        return services.Replace(ServiceDescriptor.Singleton<IPermissionChecker, AlwaysAllowPermissionChecker>());
    }
}
