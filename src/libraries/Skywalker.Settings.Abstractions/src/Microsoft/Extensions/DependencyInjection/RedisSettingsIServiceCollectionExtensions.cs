// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Settings;
using Skywalker.Settings.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class RedisSettingsIServiceCollectionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static ISettingBuilder AddSettingsCore(this IServiceCollection services, Action<SkywalkerSettingOptions> options)
    {
        services.Configure(options);
        services.AddTransient<ConfigurationSettingValueProvider>();
        services.AddTransient<DefaultValueSettingValueProvider>();
        services.AddTransient<GlobalSettingValueProvider>();
        //services.AddTransient<UserSettingValueProvider>();
        services.AddTransient<ISettingEncryptionService, SettingEncryptionService>();
        services.AddTransient<ISettingProvider, SettingProvider>();
        services.AddSingleton<ISettingDefinitionManager, SettingDefinitionManager>();
        services.AddSingleton<ISettingValueProviderManager, SettingValueProviderManager>();
        var builder = new SettingBuilder(services);
        return builder;
    }
}
