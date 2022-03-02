using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.AspNetCore.Permissions;
using Skywalker.Permissions;
using Skywalker.Permissions.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class PermissionsServiceCollectionExtensions
{
    internal static IServiceCollection AddPermissionsCore(this IServiceCollection services, Action<PermissionOptions> options)
    {
        services.Configure(options);
        services.TryAddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
        services.TryAddSingleton<IAuthorizationHandler, PermissionsRequirementHandler>();
        services.TryAddSingleton<IPermissionDefinitionManager, PermissionDefinitionManager>();
        services.TryAddSingleton<IPermissionValueProviderManager, PermissionValueProviderManager>();

        services.TryAddTransient<DefaultAuthorizationPolicyProvider>();
        services.TryAddTransient<IPermissionChecker, PermissionChecker>();
        services.TryAddTransient<ISkywalkerAuthorizationService, SkywalkerAuthorizationService>();
        services.TryAddTransient<ISkywalkerAuthorizationPolicyProvider, SkywalkerAuthorizationPolicyProvider>();
        return services;
    }

    public static IServiceCollection AddAlwaysAllowPermissionsAuthorization(this IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Singleton<IAuthorizationService, AlwaysAllowAuthorizationService>());
        services.Replace(ServiceDescriptor.Singleton<ISkywalkerAuthorizationService, AlwaysAllowAuthorizationService>());
        services.Replace(ServiceDescriptor.Singleton<IPermissionsAuthorizationService, AlwaysAllowPermissionsAuthorizationService>());
        return services.Replace(ServiceDescriptor.Singleton<IPermissionChecker, AlwaysAllowPermissionChecker>());
    }

    public static IServiceCollection AddPermissions(this IServiceCollection services, Action<PermissionOptions> options)
    {
        return services.AddPermissionsCore(options);
    }

    public static IServiceCollection AddPermissions(this IServiceCollection services)
    {
        return services.AddPermissionsCore(options =>
        {
            options.ValueProviders.Add<UserPermissionValueProvider>();
            options.ValueProviders.Add<RolePermissionValueProvider>();
            options.ValueProviders.Add<ClientPermissionValueProvider>();
        });
    }
}
