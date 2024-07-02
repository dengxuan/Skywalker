// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.EventBus;
using Skywalker.EventBus.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class EventBusIServiceCollectionExtensions
{
    public static IServiceCollection AddEventBus(this IServiceCollection services, Action<EventBusBuilder> builder)
    {
        var eventBusBuilder = new EventBusBuilder(services);
        services.AddSingleton(eventBusBuilder);
        builder(eventBusBuilder);
        services.TryAddSingleton<IEventHandlerInvoker,EventHandlerInvoker>();
        return services;
    }
}
