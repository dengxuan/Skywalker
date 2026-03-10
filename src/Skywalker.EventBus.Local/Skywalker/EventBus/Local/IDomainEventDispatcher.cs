namespace Skywalker.EventBus.Local;

/// <summary>
/// Interface for dispatching domain events.
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches all domain events from the given entities.
    /// </summary>
    /// <param name="entities">The entities with domain events.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DispatchEventsAsync(IEnumerable<IHasDomainEvents> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatches a single domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to dispatch.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DispatchEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}

