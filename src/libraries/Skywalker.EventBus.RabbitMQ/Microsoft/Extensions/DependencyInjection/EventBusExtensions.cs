// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.EventBus.Abstractions;
using Skywalker.EventBus.RabbitMQ;

namespace Microsoft.Extensions.DependencyInjection;

public static class EventBusExtensions
{
    public static void UseRabbitMQ(this EventBusBuilder builder, Action<RabbitMqEventBusOptions> options)
    {
        builder.Services.Configure(options);
        builder.Services.AddRabbitMQ();
        builder.Services.AddGuidGenerator();
        builder.Services.AddSingleton<IEventBus, RabbitMqDistributedEventBus>();
        builder.Services.AddHostedService<RabbitMqDistributedEventBus>();
    }
}
