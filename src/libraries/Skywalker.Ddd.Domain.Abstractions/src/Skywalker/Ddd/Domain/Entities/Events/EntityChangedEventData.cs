using Skywalker.Ddd.Domain.Entities;

namespace Skywalker.Ddd.Domain.Entities.Events;

/// <summary>
/// Used to pass data for an event when an entity (<see cref="IEntity"/>) is changed (created, updated or deleted).
/// See <see cref="EntityCreatedEventData{TEntity}"/>, <see cref="EntityDeletedEventData{TEntity}"/> and <see cref="EntityUpdatedEventData{TEntity}"/> classes.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <param name="Entity">Changed entity in this event</param>
[Serializable]
public record class EntityChangedEventData<TEntity>(TEntity Entity) : EntityEventData<TEntity>(Entity) where TEntity : notnull, Entity { }
