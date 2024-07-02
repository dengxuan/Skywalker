// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.RabbitMQ;
using Skywalker.Extensions.RabbitMQ.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class RabbitMQICollectionExtensions
{
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();
        services.Configure<SkywalkerRabbitMqOptions>(configuration.GetSection("RabbitMQ"));
        services.AddThreading();
        services.AddExceptionHandler();

        services.TryAddSingleton<IChannelPool, ChannelPool>();
        services.TryAddSingleton<IConnectionPool, ConnectionPool>();
        services.TryAddSingleton<IRabbitMqMessageConsumerFactory, RabbitMqMessageConsumerFactory>();

        services.TryAddTransient<IRabbitMqSerializer, Utf8JsonRabbitMqSerializer>();
        services.TryAddTransient<IRabbitMqMessageConsumer, RabbitMqMessageConsumer>();
        return services;
    }
}
