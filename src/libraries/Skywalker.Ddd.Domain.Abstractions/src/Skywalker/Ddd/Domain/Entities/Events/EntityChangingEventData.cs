namespace Skywalker.Domain.Entities.Events;

/// <summary>
/// Used to pass data for an event when an entity (<see cref="IEntity"/>) is being changed (creating, updating or deleting).
/// See <see cref="EntityCreatingEventData{TEntity}"/>, <see cref="EntityDeletingEventData{TEntity}"/> and <see cref="EntityUpdatingEventData{TEntity}"/> classes.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <param name="Entity">Changing entity in this event</param>
[Serializable]
public record class EntityChangingEventData<TEntity>(TEntity Entity) : EntityEventData<TEntity>(Entity) where TEntity : notnull, Entity { }
