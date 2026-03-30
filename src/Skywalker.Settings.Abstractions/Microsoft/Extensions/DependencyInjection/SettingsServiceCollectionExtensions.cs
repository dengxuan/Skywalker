// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.ObjectAccessor.Abstractions;
using Skywalker.Settings;
using Skywalker.Settings.Abstractions;


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding Settings services.
/// </summary>
public static class SettingsServiceCollectionExtensions
{
    private static ISettingBuilder AddSettingServices(this IServiceCollection services)
    {
        if (services.Any(service => service.ServiceType == typeof(IObjectAccessor<ISettingBuilder>)))
        {
            return services.GetSingletonInstance<IObjectAccessor<ISettingBuilder>>().Value!;
        }
        var builder = new SettingBuilder(services);
        services.AddObjectAccessor<ISettingBuilder>(builder);

        // Register value providers as Singleton (stateless, resolved by Singleton manager)
        services.AddSingleton<DefaultValueSettingValueProvider>();
        services.AddSingleton<ConfigurationSettingValueProvider>();

        // Core services
        services.AddTransient<ISettingEncryptionService, SettingEncryptionService>();
        services.AddTransient<ISettingProvider, SettingProvider>();
        services.AddSingleton<ISettingDefinitionManager, SettingDefinitionManager>();
        services.AddSingleton<ISettingValueProviderManager, SettingValueProviderManager>();

        return builder;
    }

    /// <summary>
    /// Adds Settings services with custom options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="options">The options action.</param>
    /// <returns>The setting builder.</returns>
    public static ISettingBuilder AddSettings(this IServiceCollection services, Action<SkywalkerSettingOptions> options)
    {
        var builder = services.AddSettingServices();
        services.Configure(options);
        return builder;
    }

    /// <summary>
    /// Adds Settings services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The setting builder.</returns>
    public static ISettingBuilder AddSettings(this IServiceCollection services)
    {
        return services.AddSettingServices();
    }
}
