// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using Skywalker.EventBus.Abstractions;

namespace Skywalker.EventBus;

public class EventHandlerInvoker : IEventHandlerInvoker
{
    private readonly ConcurrentDictionary<string, EventHandlerInvokerCacheItem> _cache;

    public EventHandlerInvoker()
    {
        _cache = new ConcurrentDictionary<string, EventHandlerInvokerCacheItem>();
    }

    public async Task InvokeAsync(IEventHandler eventHandler, object eventData, Type eventType)
    {
        var cacheItem = _cache.GetOrAdd($"{eventHandler.GetType().FullName}-{eventType.FullName}", _ =>
        {
            var item = new EventHandlerInvokerCacheItem();

            if (typeof(IEventHandler<>).MakeGenericType(eventType).IsInstanceOfType(eventHandler))
            {
                item.Executor = (IEventHandlerMethodExecutor?)Activator.CreateInstance(typeof(LocalEventHandlerMethodExecutor<>).MakeGenericType(eventType));
            }

            return item;
        });

        if (cacheItem.Executor != null)
        {
            await cacheItem.Executor.ExecutorAsync(eventHandler, eventData);
        }


        if (cacheItem.Executor == null)
        {
            throw new NotSupportedException("The object instance is not an event handler. Object type: " + eventHandler.GetType().AssemblyQualifiedName);
        }
    }
}
