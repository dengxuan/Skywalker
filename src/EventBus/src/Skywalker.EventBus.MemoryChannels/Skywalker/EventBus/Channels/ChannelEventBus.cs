using Microsoft.Extensions.DependencyInjection;
using Skywalker.EventBus.Abstractions;
using Skywalker.Messaging.Abstractions;
using Skywalker.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.EventBus.MemoryChannels;

public class ChannelEventBus : EventBusBase, IEventBus
{

    private readonly ConcurrentDictionary<string, Type> _eventTypes = new();

    private readonly ConcurrentDictionary<Type, List<IEventHandlerFactory>> _handlerFactories = new();

    private readonly IMessagePublisher _messagePublisher;

    private readonly IMessageSubscriber _messageSubscriber;

    public ChannelEventBus(IServiceScopeFactory serviceScopeFactory, IMessagePublisher messagePublisher, IMessageSubscriber messageSubscriber) : base(serviceScopeFactory)
    {
        _messagePublisher = messagePublisher;
        _messageSubscriber = messageSubscriber;
    }

    private List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
    {
        return _handlerFactories.GetOrAdd(eventType, (type) =>
        {
            var eventName = EventNameAttribute.GetNameOrDefault(type);
            _eventTypes[eventName] = type;
            return new List<IEventHandlerFactory>();
        });
    }

    public async Task HandleAsync(string routingKey, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken)
    {
        var eventType = _eventTypes.GetOrDefault(routingKey);
        if (eventType == null)
        {
            return;
        }
        var eventArgs = await bytes.FromBytesAsync(eventType, cancellationToken);
        await TriggerHandlersAsync(eventType, eventArgs!);
    }

    public override async Task PublishAsync(Type eventType, object eventArgs)
    {
        string routingKey = EventNameAttribute.GetNameOrDefault(eventType);
        byte[] bytes = await eventArgs.ToBytesAsync(eventType);
        await _messagePublisher.PublishAsync(routingKey, bytes);
    }

    public override IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
    {
        var handlerFactories = GetOrCreateHandlerFactories(eventType);
        handlerFactories.Locking(factories =>
        {
            if (!factory.IsInFactories(factories))
            {
                string routingKey = EventNameAttribute.GetNameOrDefault(eventType);
                _messageSubscriber.SubscribeAsync(routingKey, HandleAsync, default);

                factories.Add(factory);
            }
        });

        return new EventHandlerFactoryUnregistrar(this, eventType, factory);
    }

    public override void Unsubscribe<TEvent>(Func<TEvent, Task> action)
    {
        action.NotNull(nameof(action));

        var handlerFactories = GetOrCreateHandlerFactories(typeof(TEvent));
        handlerFactories.Locking(factories =>
        {
            factories.RemoveAll(match =>
            {
                if (match is not SingleInstanceHandlerFactory singleInstanceFactory)
                {
                    return false;
                }

                if (singleInstanceFactory.HandlerInstance is not ActionEventHandler<TEvent> actionHandler)
                {
                    return false;
                }

                return actionHandler.Action == action;
            });
        });
    }

    public override void Unsubscribe(Type eventType, IEventHandler handler)
    {
        var handlerFactories = GetOrCreateHandlerFactories(eventType);
        handlerFactories.Locking(factories =>
        {
            factories.RemoveAll(match => match is SingleInstanceHandlerFactory && (match as SingleInstanceHandlerFactory)?.HandlerInstance == handler);
        });
    }

    public override void Unsubscribe(Type eventType, IEventHandlerFactory factory)
    {
        var handlerFactories = GetOrCreateHandlerFactories(eventType);
        handlerFactories.Locking(factories => factories.Remove(factory));
    }

    public override void UnsubscribeAll(Type eventType)
    {
        var handlerFactories = GetOrCreateHandlerFactories(eventType);
        handlerFactories.Locking(factories => factories.Clear());
    }


    private static bool ShouldTriggerEventForHandler(Type targetEventType, Type handlerEventType)
    {
        //Should trigger same type
        if (handlerEventType == targetEventType)
        {
            return true;
        }

        //TODO: Support inheritance? But it does not support on subscription to RabbitMq!
        //Should trigger for inherited types
        if (handlerEventType.IsAssignableFrom(targetEventType))
        {
            return true;
        }

        return false;
    }

    protected override IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType)
    {
        var handlerFactories = new List<EventTypeWithEventHandlerFactories>();

        var shouldTriggerHandlers = _handlerFactories.Where(predicate => ShouldTriggerEventForHandler(eventType, predicate.Key));

        foreach (var handlerFactory in shouldTriggerHandlers)
        {
            handlerFactories.Add(new EventTypeWithEventHandlerFactories(handlerFactory.Key, handlerFactory.Value));
        }

        return handlerFactories.ToArray();
    }
}
