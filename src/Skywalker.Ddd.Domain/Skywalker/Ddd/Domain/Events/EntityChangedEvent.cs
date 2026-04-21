// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.EventBus.Local;

namespace Skywalker.Ddd.Domain.Events;

/// <summary>
/// Event raised when an entity is changed.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public class EntityChangedEvent<TEntity> : DomainEventBase
{
    /// <summary>
    /// Gets the entity that was changed.
    /// </summary>
    public TEntity Entity { get; }

    /// <summary>
    /// Gets the type of change.
    /// </summary>
    public EntityChangeType ChangeType { get; }

    public EntityChangedEvent(TEntity entity, EntityChangeType changeType)
    {
        Entity = entity;
        ChangeType = changeType;
    }
}

/// <summary>
/// Event raised when an entity is created.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public class EntityCreatedEvent<TEntity> : EntityChangedEvent<TEntity>
{
    public EntityCreatedEvent(TEntity entity) : base(entity, EntityChangeType.Created)
    {
    }
}

/// <summary>
/// Event raised when an entity is updated.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public class EntityUpdatedEvent<TEntity> : EntityChangedEvent<TEntity>
{
    public EntityUpdatedEvent(TEntity entity) : base(entity, EntityChangeType.Updated)
    {
    }
}

/// <summary>
/// Event raised when an entity is deleted.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public class EntityDeletedEvent<TEntity> : EntityChangedEvent<TEntity>
{
    public EntityDeletedEvent(TEntity entity) : base(entity, EntityChangeType.Deleted)
    {
    }
}
