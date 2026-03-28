// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Skywalker.EventBus;
using Skywalker.EventBus.Abstractions;
using Skywalker.EventBus.RabbitMQ;


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding RabbitMQ event bus services.
/// </summary>
public static class EventBusRabbitMQServiceCollectionExtensions
{
    /// <summary>
    /// Adds the RabbitMQ event bus with configuration from appsettings.json.
    /// Configuration section: Skywalker:EventBus:RabbitMQ
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddEventBusRabbitMQ(this IServiceCollection services)
    {
        // 添加 EventBus 核心服务
        services.AddEventBusCore();

        // 从配置文件绑定选项
        var configuration = services.GetConfiguration();
        if (configuration != null)
        {
            services.AddOptions<RabbitMqEventBusOptions>()
                .Bind(configuration.GetSection(RabbitMqEventBusOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
        else
        {
            services.AddOptions<RabbitMqEventBusOptions>()
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        return services.AddEventBusRabbitMQServices();
    }

    /// <summary>
    /// Adds the RabbitMQ event bus with configuration action.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddEventBusRabbitMQ(this IServiceCollection services, Action<RabbitMqEventBusOptions> configure)
    {
        // 添加 EventBus 核心服务
        services.AddEventBusCore();

        // 先从配置文件绑定，再应用代码配置
        var configuration = services.GetConfiguration();
        if (configuration != null)
        {
            services.AddOptions<RabbitMqEventBusOptions>()
                .Bind(configuration.GetSection(RabbitMqEventBusOptions.SectionName))
                .Configure(configure)
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
        else
        {
            services.AddOptions<RabbitMqEventBusOptions>()
                .Configure(configure)
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        return services.AddEventBusRabbitMQServices();
    }

    private static IServiceCollection AddEventBusRabbitMQServices(this IServiceCollection services)
    {
        services.AddRabbitMQ();
        services.AddGuidGenerator();
        SkywalkerEventBusRabbitMQAutoServiceExtensions.AddAutoServices(services);
        services.AddSingleton<IEventBus>(sp => sp.GetRequiredService<RabbitMqEventBus>());
        services.AddHostedService<RabbitMqSubscriber>();

        return services;
    }
}
