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

        // 默认登记内置值提供者（优先级从高到低：Configuration > Default），
        // 否则开箱 ISettingProvider 永远返回 null（#310）。用户 Configure 在其后执行，可增删调序。
        services.Configure<SkywalkerSettingOptions>(options =>
        {
            if (!options.ValueProviders.Contains<ConfigurationSettingValueProvider>())
            {
                options.ValueProviders.Add<ConfigurationSettingValueProvider>();
            }

            if (!options.ValueProviders.Contains<DefaultValueSettingValueProvider>())
            {
                options.ValueProviders.Add<DefaultValueSettingValueProvider>();
            }
        });

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
