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
}
