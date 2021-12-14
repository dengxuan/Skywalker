namespace Skywalker.Domain.Entities.Events;

/// <summary>
/// This type of event is used to notify just before deletion of an Entity.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <param name="Entity">The entity which is being deleted</param>
[Serializable]
public record class EntityDeletingEventData<TEntity>(TEntity Entity) : EntityChangingEventData<TEntity>(Entity) where TEntity : notnull, Entity;