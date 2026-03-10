// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.EventBus;
using Skywalker.EventBus.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// EventBus 内部扩展方法，供具体实现调用。
/// </summary>
internal static class EventBusIServiceCollectionExtensions
{
    /// <summary>
    /// 添加 EventBus 核心服务（internal，供具体实现调用）。
    /// </summary>
    internal static IServiceCollection AddEventBusCore(this IServiceCollection services, Action<EventBusOptions>? options = null)
    {
        if (options != null)
        {
            services.Configure(options);
        }

        services.TryAddSingleton<IEventHandlerFactory, EventHandlerFactory>();
        services.TryAddSingleton<IEventHandlerInvoker, EventHandlerInvoker>();

        return services;
    }
}
