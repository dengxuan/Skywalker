using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.AspNetCore.PermissionsEvaluator;
using Skywalker.AspNetCore.PermissionsEvaluator.Permissions;
using Skywalker.PermissionsEvaluator.Permissions;
using Skywalker.PermissionsEvaluator.Permissions.Abstractions;
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
        services.TryAddTransient<IAuthorizationService, SkywalkerAuthorizationService>();
        services.TryAddTransient<ISkywalkerAuthorizationService, SkywalkerAuthorizationService>();
        services.TryAddTransient<IAuthorizationPolicyProvider, SkywalkerAuthorizationPolicyProvider>();
        services.TryAddTransient<ISkywalkerAuthorizationPolicyProvider, SkywalkerAuthorizationPolicyProvider>();
        return services;
    }

    public static IServiceCollection AddAlwaysAllowPermissionsEvaluator(this IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Singleton<IAuthorizationService, AlwaysAllowAuthorizationService>());
        services.Replace(ServiceDescriptor.Singleton<ISkywalkerAuthorizationService, AlwaysAllowAuthorizationService>());
        services.Replace(ServiceDescriptor.Singleton<IPermissionsAuthorizationService, AlwaysAllowPermissionsAuthorizationService>());
        return services.Replace(ServiceDescriptor.Singleton<IPermissionChecker, AlwaysAllowPermissionChecker>());
    }

    public static IServiceCollection AddPermissionsEvaluator(this IServiceCollection services, Action<PermissionOptions> options)
    {
        return services.AddAuthorizationServices(options);
    }

    public static IServiceCollection AddPermissionsEvaluator(this IServiceCollection services)
    {
        return services.AddAuthorizationServices(options =>
        {
            options.ValueProviders.Add<UserPermissionValueProvider>();
            options.ValueProviders.Add<RolePermissionValueProvider>();
            options.ValueProviders.Add<ClientPermissionValueProvider>();
        });
    }
}
