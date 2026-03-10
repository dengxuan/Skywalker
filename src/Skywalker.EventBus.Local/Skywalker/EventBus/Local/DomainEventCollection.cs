using System.Collections;

namespace Skywalker.EventBus.Local;

/// <summary>
/// Collection for managing domain events.
/// </summary>
public class DomainEventCollection : IReadOnlyCollection<IDomainEvent>
{
    private readonly List<IDomainEvent> _events = new();

    /// <inheritdoc/>
    public int Count => _events.Count;

    /// <summary>
    /// Adds a domain event to the collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    public void Add(IDomainEvent domainEvent)
    {
        _events.Add(domainEvent);
    }

    /// <summary>
    /// Removes a domain event from the collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    public void Remove(IDomainEvent domainEvent)
    {
        _events.Remove(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from the collection.
    /// </summary>
    public void Clear()
    {
        _events.Clear();
    }

    /// <inheritdoc/>
    public IEnumerator<IDomainEvent> GetEnumerator() => _events.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

