namespace Skywalker.EventBus.Local;

/// <summary>
/// Marker interface for domain events.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the time when the event occurred.
    /// </summary>
    DateTimeOffset OccurredOn { get; }
}

/// <summary>
/// Base class for domain events.
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
    /// <inheritdoc/>
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

