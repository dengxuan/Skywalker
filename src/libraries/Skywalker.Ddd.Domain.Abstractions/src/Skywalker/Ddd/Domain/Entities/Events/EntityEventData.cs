using Skywalker.EventBus.Abstractions;

namespace Skywalker.Domain.Entities.Events;

/// <summary>
/// Used to pass data for an event that is related to with an <see cref="IEntity"/> object.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <param name="Entity">Related entity with this event</param>
[Serializable]
public record class EntityEventData<TEntity>(TEntity Entity) : IEventDataWithInheritableGenericArgument where TEntity : notnull, Entity
{
    public virtual object[] GetConstructorArgs()
    {
        return new object[] { Entity };
    }
}
