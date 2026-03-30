// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.RabbitMQ;
using Skywalker.Extensions.RabbitMQ.Abstractions;


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding RabbitMQ services.
/// </summary>
public static class RabbitMQICollectionExtensions
{
    /// <summary>
    /// Adds RabbitMQ services with configuration from appsettings.json.
    /// Configuration section: Skywalker:RabbitMQ
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();
        if (configuration != null)
        {
            services.AddOptions<SkywalkerRabbitMqOptions>()
                .Bind(configuration.GetSection(SkywalkerRabbitMqOptions.SectionName))
                .ValidateOnStart();
        }
        else
        {
            services.AddOptions<SkywalkerRabbitMqOptions>()
                .ValidateOnStart();
        }

        return services.AddRabbitMQServices();
    }

    /// <summary>
    /// Adds RabbitMQ services with configuration action.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, Action<SkywalkerRabbitMqOptions> configure)
    {
        var configuration = services.GetConfiguration();
        if (configuration != null)
        {
            services.AddOptions<SkywalkerRabbitMqOptions>()
                .Bind(configuration.GetSection(SkywalkerRabbitMqOptions.SectionName))
                .Configure(configure)
                .ValidateOnStart();
        }
        else
        {
            services.AddOptions<SkywalkerRabbitMqOptions>()
                .Configure(configure)
                .ValidateOnStart();
        }

        return services.AddRabbitMQServices();
    }

    private static IServiceCollection AddRabbitMQServices(this IServiceCollection services)
    {
        services.AddThreading();
        services.TryAddSingleton<IRabbitMqSerializer, Utf8JsonRabbitMqSerializer>();
        services.TryAddSingleton<IConnectionPool, ConnectionPool>();
        services.TryAddSingleton<IChannelPool, ChannelPool>();
        services.TryAddSingleton<IRabbitMqMessageConsumerFactory, RabbitMqMessageConsumerFactory>();
        services.TryAddTransient<IRabbitMqMessageConsumer, RabbitMqMessageConsumer>();

        return services;
    }
}
