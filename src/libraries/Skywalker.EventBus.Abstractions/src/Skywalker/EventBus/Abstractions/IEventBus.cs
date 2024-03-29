namespace Skywalker.EventBus.Abstractions;

public interface IEventBus
{
    /// <summary>
    /// Triggers an event.
    /// </summary>
    /// <typeparam name="TEvent">Event type</typeparam>
    /// <param name="eventArgs">Related data for the event</param>
    /// <returns>The task to handle async operation</returns>
    Task PublishAsync<TEvent>(TEvent eventArgs) where TEvent : class;

    /// <summary>
    /// Triggers an event.
    /// </summary>
    /// <param name="eventType">Event type</param>
    /// <param name="eventArgs">Related data for the event</param>
    /// <returns>The task to handle async operation</returns>
    Task PublishAsync(Type eventType, object eventArgs);

    /// <summary>
    /// Registers to an event.
    /// Given action is called for all event occurrences.
    /// </summary>
    /// <param name="action">Action to handle events</param>
    /// <typeparam name="TEvent">Event type</typeparam>
    IDisposable Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class;

    /// <summary>
    /// Registers to an event.
    /// A new instance of <see cref="THandler"/> object is created for every event occurrence.
    /// </summary>
    /// <typeparam name="TEvent">Event type</typeparam>
    /// <typeparam name="THandler">Type of the event handler</typeparam>
    IDisposable Subscribe<TEvent, THandler>() where TEvent : class where THandler : IEventHandler, new();

    /// <summary>
    /// Registers to an event.
    /// Same (given) instance of the handler is used for all event occurrences.
    /// </summary>
    /// <param name="eventType">Event type</param>
    /// <param name="handler">Object to handle the event</param>
    IDisposable Subscribe(Type eventType, IEventHandler handler);

    /// <summary>
    /// Registers to an event.
    /// Given factory is used to create/release handlers
    /// </summary>
    /// <typeparam name="TEvent">Event type</typeparam>
    /// <param name="factory">A factory to create/release handlers</param>
    IDisposable Subscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class;

    /// <summary>
    /// Registers to an event.
    /// </summary>
    /// <param name="eventType">Event type</param>
    /// <param name="factory">A factory to create/release handlers</param>
    IDisposable Subscribe(Type eventType, IEventHandlerFactory factory);

    /// <summary>
    /// Registers to an event. 
    /// Same (given) instance of the handler is used for all event occurrences.
    /// </summary>
    /// <typeparam name="TEvent">Event type</typeparam>
    /// <param name="handler">Object to handle the event</param>
    IDisposable Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class;

    /// <summary>
    /// Unregisters from an event.
    /// </summary>
    /// <typeparam name="TEvent">Event type</typeparam>
    /// <param name="action"></param>
    void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class;

    /// <summary>
    /// Unregisters from an event.
    /// </summary>
    /// <typeparam name="TEvent">Event type</typeparam>
    /// <param name="handler">Handler object that is registered before</param>
    void Unsubscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class;

    /// <summary>
    /// Unregisters from an event.
    /// </summary>
    /// <param name="eventType">Event type</param>
    /// <param name="handler">Handler object that is registered before</param>
    void Unsubscribe(Type eventType, IEventHandler handler);

    /// <summary>
    /// Unregisters from an event.
    /// </summary>
    /// <typeparam name="TEvent">Event type</typeparam>
    /// <param name="factory">Factory object that is registered before</param>
    void Unsubscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class;

    /// <summary>
    /// Unregisters from an event.
    /// </summary>
    /// <param name="eventType">Event type</param>
    /// <param name="factory">Factory object that is registered before</param>
    void Unsubscribe(Type eventType, IEventHandlerFactory factory);

    /// <summary>
    /// Unregisters all event handlers of given event type.
    /// </summary>
    /// <typeparam name="TEvent">Event type</typeparam>
    void UnsubscribeAll<TEvent>() where TEvent : class;

    /// <summary>
    /// Unregisters all event handlers of given event type.
    /// </summary>
    /// <param name="eventType">Event type</param>
    void UnsubscribeAll(Type eventType);
}
