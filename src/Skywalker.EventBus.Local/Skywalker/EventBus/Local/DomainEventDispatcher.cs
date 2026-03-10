using Skywalker.DependencyInjection;

namespace Skywalker.EventBus.Local;

/// <summary>
/// Default implementation of <see cref="IDomainEventDispatcher"/>.
/// </summary>
public class DomainEventDispatcher : IDomainEventDispatcher, ISingletonDependency
{
    private readonly ILocalEventBus _eventBus;

    public DomainEventDispatcher(ILocalEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    /// <inheritdoc/>
    public async Task DispatchEventsAsync(IEnumerable<IHasDomainEvents> entities, CancellationToken cancellationToken = default)
    {
        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .OrderBy(e => e.OccurredOn)
            .ToList();

        foreach (var entity in entities)
        {
            entity.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            await DispatchEventAsync(domainEvent, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task DispatchEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await _eventBus.PublishAsync(domainEvent.GetType(), domainEvent);
    }
}

