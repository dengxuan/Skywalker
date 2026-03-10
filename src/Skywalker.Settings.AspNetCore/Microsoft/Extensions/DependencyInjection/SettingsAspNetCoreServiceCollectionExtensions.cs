// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding Settings AspNetCore services.
/// </summary>
public static class SettingsAspNetCoreServiceCollectionExtensions
{
    /// <summary>
    /// Adds Settings AspNetCore services to the specified <see cref="IServiceCollection"/>.
    /// Note: Use app.MapSettingEndpoints() to register the endpoints.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddSkywalkerSettingsAspNetCore(this IServiceCollection services)
    {
        // Minimal API doesn't require additional service registration here.
        // The endpoints are registered via app.MapSettingEndpoints(routePrefix).
        return services;
    }
}
