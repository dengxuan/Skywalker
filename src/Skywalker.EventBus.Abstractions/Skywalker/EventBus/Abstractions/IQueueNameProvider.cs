namespace Skywalker.EventBus.Abstractions;

public interface IQueueNameProvider
{
    string? GetName(Type eventType);
}
