namespace Skywalker.Domain.Entities.Events;

/// <summary>
/// This type of event can be used to notify just after creation of an Entity.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <param name="Entity">The entity which is created</param>
[Serializable]
public record class EntityCreatedEventData<TEntity>(TEntity Entity) : EntityChangedEventData<TEntity>(Entity) where TEntity : notnull, Entity { }
