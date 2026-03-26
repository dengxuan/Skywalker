// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Skywalker.ObjectMapping;
using Skywalker.ObjectMapping.AutoMapper;

[assembly: Skywalker.SkywalkerModule("AutoMapperObjectMapping")]

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding AutoMapper services.
/// </summary>
public static class AutoMapperServiceCollectionExtensions
{
    /// <summary>
    /// Adds AutoMapper services. Profiles are loaded from assemblies configured via AutoMapperOptions.
    /// This method is called automatically by AddSkywalker().
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddAutoMapperObjectMapping(this IServiceCollection services)
    {
        // 延迟注册 AutoMapper，等待 Options 配置完成
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AutoMapperOptions>>().Value;
            var assemblies = options.ProfileAssemblies.Count > 0
                ? options.ProfileAssemblies.ToArray()
                : AppDomain.CurrentDomain.GetAssemblies();

            var loggerFactory = sp.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
            var config = new global::AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddMaps(assemblies);
            }, loggerFactory);
            return config.CreateMapper();
        });

        services.AddSingleton<IObjectMapper, AutoMapperObjectMapper>();
        return services;
    }

    /// <summary>
    /// Adds AutoMapper services with profiles from the specified assemblies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">The assemblies to scan for profiles.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddAutoMapperObjectMapping(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var config = new global::AutoMapper.MapperConfiguration(cfg =>
        {
            cfg.AddMaps(assemblies);
        }, NullLoggerFactory.Instance);
        services.AddSingleton(config.CreateMapper());
        services.AddSingleton<IObjectMapper, AutoMapperObjectMapper>();
        return services;
    }

    /// <summary>
    /// Adds AutoMapper services with profiles from the assembly containing the specified type.
    /// </summary>
    /// <typeparam name="T">A type from the assembly to scan.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddAutoMapperObjectMapping<T>(this IServiceCollection services)
    {
        return services.AddAutoMapperObjectMapping(typeof(T).Assembly);
    }

    /// <summary>
    /// Adds AutoMapper services with a configuration action.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configAction">The configuration action.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddAutoMapperObjectMapping(
        this IServiceCollection services,
        Action<global::AutoMapper.IMapperConfigurationExpression> configAction)
    {
        var config = new global::AutoMapper.MapperConfiguration(configAction, NullLoggerFactory.Instance);
        services.AddSingleton(config.CreateMapper());
        services.AddSingleton<IObjectMapper, AutoMapperObjectMapper>();
        return services;
    }
}

