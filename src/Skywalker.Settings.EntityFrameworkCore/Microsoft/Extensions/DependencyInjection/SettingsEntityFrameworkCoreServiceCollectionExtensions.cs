// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Settings;
using Skywalker.Settings.Abstractions;
using Skywalker.Settings.EntityFrameworkCore;

[assembly: Skywalker.SkywalkerModule("SettingsEntityFrameworkCore")]

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding Settings EntityFrameworkCore services to the DI container.
/// </summary>
public static class SettingsEntityFrameworkCoreServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Settings EntityFrameworkCore services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddSettingsEntityFrameworkCore(this IServiceCollection services)
    {
        // Register setting value providers (order is configured via SkywalkerSettingOptions.ValueProviders)
        services.AddScoped<UserSettingValueProvider>();
        services.AddScoped<GlobalSettingValueProvider>();

        // Register setting manager for write operations
        services.AddScoped<ISettingManager, SettingManager>();

        return services;
    }

    /// <summary>
    /// Configures the default value provider order: User > Global > Configuration > Default.
    /// Call this method to use the standard provider priority.
    /// </summary>
    /// <param name="options">The setting options.</param>
    /// <returns>The setting options for chaining.</returns>
    public static SkywalkerSettingOptions UseDefaultProviders(this SkywalkerSettingOptions options)
    {
        options.ValueProviders.Clear();
        options.ValueProviders.Add<UserSettingValueProvider>();
        options.ValueProviders.Add<GlobalSettingValueProvider>();
        options.ValueProviders.Add<ConfigurationSettingValueProvider>();
        options.ValueProviders.Add<DefaultValueSettingValueProvider>();
        return options;
    }
}
