using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.AspNetCore;
using Skywalker.AspNetCore.Endpoints;
using Skywalker.AspNetCore.Permissions;
using Skywalker.AspNetCore.Permissions.Abstractions;
using Skywalker.Extensions.SimpleStateChecking;
using Skywalker.Permissions;
using Skywalker.Permissions.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class PermissionsIServiceCollectionExtensions
{
    internal static IServiceCollection AddAuthorizationServices(this IServiceCollection services, Action<PermissionOptions> options)
    {
        services.Configure(options);
        services.TryAddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
        services.TryAddSingleton<IAuthorizationHandler, PermissionsRequirementHandler>();
        //services.TryAddSingleton<IPermissionDefinitionManager, PermissionDefinitionManager>();
        services.TryAddSingleton<IPermissionValueProviderManager, PermissionValueProviderManager>();
        services.TryAddSingleton(typeof(ISimpleStateCheckerManager<>), typeof(SimpleStateCheckerManager<>));

        services.TryAddTransient<DefaultAuthorizationPolicyProvider>();
        services.TryAddTransient<IPermissionChecker, PermissionChecker>();
        services.TryAddTransient<IAuthorizationService, SkywalkerAuthorizationService>();
        services.TryAddTransient<ISkywalkerAuthorizationService, SkywalkerAuthorizationService>();
        services.TryAddTransient<IAuthorizationPolicyProvider, SkywalkerAuthorizationPolicyProvider>();
        services.TryAddTransient<ISkywalkerAuthorizationPolicyProvider, SkywalkerAuthorizationPolicyProvider>();

        services.AddDefaultEndpoints();
        return services;
    }

    /// <summary>
    /// Adds the default endpoints.
    /// </summary>
    /// <param name="services">The Service Collection.</param>
    /// <returns></returns>
    internal static IServiceCollection AddDefaultEndpoints(this IServiceCollection services)
    {
        services.AddTransient<IEndpointRouter, EndpointRouter>();

        services.AddEndpoint<CheckPermissionEndpoint>(EndpointNames.CheckPermission, ProtocolRoutePaths.ChackPermission.EnsureLeadingSlash());
        services.AddEndpoint<CommitPermissionEndpoint>(EndpointNames.CommitPermission, ProtocolRoutePaths.CommitPermission.EnsureLeadingSlash());

        return services;
    }

    /// <summary>
    /// Adds the endpoint.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="services">The builder.</param>
    /// <param name="name">The name.</param>
    /// <param name="path">The path.</param>
    /// <returns></returns>
    internal static IServiceCollection AddEndpoint<T>(this IServiceCollection services, string name, PathString path)
        where T : class, IEndpointHandler
    {
        services.AddTransient<T>();
        services.AddSingleton(new Endpoint(name, path, typeof(T)));

        return services;
    }

    public static IServiceCollection AddAlwaysAllowPermissions(this IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Singleton<IAuthorizationService, AlwaysAllowAuthorizationService>());
        services.Replace(ServiceDescriptor.Singleton<ISkywalkerAuthorizationService, AlwaysAllowAuthorizationService>());
        services.Replace(ServiceDescriptor.Singleton<IPermissionsAuthorizationService, AlwaysAllowPermissionsAuthorizationService>());
        return services.Replace(ServiceDescriptor.Singleton<IPermissionChecker, AlwaysAllowPermissionChecker>());
    }

    public static IServiceCollection AddPermissions(this IServiceCollection services, Action<PermissionOptions> options)
    {
        return services.AddAuthorizationServices(options);
    }

    public static IServiceCollection AddPermissions(this IServiceCollection services)
    {
        return services.AddAuthorizationServices(options =>
        {
            options.ValueProviders.Add<UserPermissionValueProvider>();
            options.ValueProviders.Add<RolePermissionValueProvider>();
            options.ValueProviders.Add<ClientPermissionValueProvider>();
        });
    }
}
