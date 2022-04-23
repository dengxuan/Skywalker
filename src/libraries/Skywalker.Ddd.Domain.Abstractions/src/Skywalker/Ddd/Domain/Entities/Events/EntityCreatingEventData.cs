namespace Skywalker.Ddd.Domain.Entities.Events;

/// <summary>
/// This type of event is used to notify just before creation of an Entity.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <param name="Entity">The entity which is being created</param>
[Serializable]
public record class EntityCreatingEventData<TEntity>(TEntity Entity) : EntityChangingEventData<TEntity>(Entity) where TEntity : notnull, Entity;
