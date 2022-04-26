using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.AspNetCore.Authorization;
using Skywalker.AspNetCore.Authorization.Permissions;
using Skywalker.Authorization.Permissions;
using Skywalker.Authorization.Permissions.Abstractions;
using Skywalker.Extensions.SimpleStateChecking;

namespace Microsoft.Extensions.DependencyInjection;

public static class AuthorizationIServiceCollectionExtensions
{
    internal static IServiceCollection AddAuthorizationServices(this IServiceCollection services, Action<PermissionOptions> options)
    {
        services.Configure(options);
        services.TryAddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
        services.TryAddSingleton<IAuthorizationHandler, PermissionsRequirementHandler>();
        services.TryAddSingleton<IPermissionDefinitionManager, PermissionDefinitionManager>();
        services.TryAddSingleton<IPermissionValueProviderManager, PermissionValueProviderManager>();
        services.TryAddSingleton(typeof(ISimpleStateCheckerManager<>), typeof(SimpleStateCheckerManager<>));

        services.TryAddTransient<DefaultAuthorizationPolicyProvider>();
        services.TryAddTransient<IPermissionChecker, PermissionChecker>();
        services.TryAddTransient<ISkywalkerAuthorizationService, SkywalkerAuthorizationService>();
        services.TryAddTransient<ISkywalkerAuthorizationPolicyProvider, SkywalkerAuthorizationPolicyProvider>();
        return services;
    }

    public static IServiceCollection AddAlwaysAllowAuthorization(this IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Singleton<IAuthorizationService, AlwaysAllowAuthorizationService>());
        services.Replace(ServiceDescriptor.Singleton<ISkywalkerAuthorizationService, AlwaysAllowAuthorizationService>());
        services.Replace(ServiceDescriptor.Singleton<IPermissionsAuthorizationService, AlwaysAllowPermissionsAuthorizationService>());
        return services.Replace(ServiceDescriptor.Singleton<IPermissionChecker, AlwaysAllowPermissionChecker>());
    }

    public static IServiceCollection AddAuthorizationPermissionsEvaluator(this IServiceCollection services, Action<PermissionOptions> options)
    {
        return services.AddAuthorizationServices(options);
    }

    public static IServiceCollection AddAuthorizationPermissionsEvaluator(this IServiceCollection services)
    {
        return services.AddAuthorizationServices(options =>
        {
            options.ValueProviders.Add<UserPermissionValueProvider>();
            options.ValueProviders.Add<RolePermissionValueProvider>();
            options.ValueProviders.Add<ClientPermissionValueProvider>();
        });
    }
}
