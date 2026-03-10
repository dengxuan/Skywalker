using Skywalker.EventBus.Abstractions;

namespace Skywalker.EventBus;

public abstract class EventBusBase : IEventBus
{
    /// <inheritdoc/>
    public virtual Task PublishAsync<TEvent>(TEvent eventArgs) where TEvent : class
    {
        return PublishAsync(typeof(TEvent), eventArgs);
    }

    /// <inheritdoc/>
    public abstract Task PublishAsync(Type eventType, object eventArgs);
}
