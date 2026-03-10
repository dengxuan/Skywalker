using Skywalker.EventBus.Abstractions;

namespace Skywalker.EventBus.Local;

/// <summary>
/// Local (in-process) event bus interface.
/// </summary>
public interface ILocalEventBus : IEventBus
{
    /// <summary>
    /// Subscribes to an event.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <typeparam name="THandler">The handler type.</typeparam>
    void Subscribe<TEvent, THandler>()
        where TEvent : class
        where THandler : IEventHandler<TEvent>;

    /// <summary>
    /// Subscribes to an event.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <param name="handlerType">The handler type.</param>
    void Subscribe(Type eventType, Type handlerType);

    /// <summary>
    /// Unsubscribes from an event.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <typeparam name="THandler">The handler type.</typeparam>
    void Unsubscribe<TEvent, THandler>()
        where TEvent : class
        where THandler : IEventHandler<TEvent>;

    /// <summary>
    /// Unsubscribes from an event.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <param name="handlerType">The handler type.</param>
    void Unsubscribe(Type eventType, Type handlerType);
}

