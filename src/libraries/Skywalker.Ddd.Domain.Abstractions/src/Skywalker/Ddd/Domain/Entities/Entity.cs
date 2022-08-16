namespace Skywalker.Ddd.Domain.Entities;

/// <inheritdoc/>
[Serializable]
public abstract class Entity : IEntity, IHasConcurrencyStamp, IHasCreationTime
{
    public virtual string ConcurrencyStamp { get; set; }

    public virtual DateTime CreationTime { get; set; }

    protected Entity()
    {
        ConcurrencyStamp = Guid.NewGuid().ToString("N");
    }

    /// <inheritdoc/>
    public override string ToString() => $"[ENTITY: {GetType().Name}] Keys = {GetKeys().JoinAsString(",")}";

    public abstract object[] GetKeys();

    public bool EntityEquals(IEntity other) => EntityHelper.EntityEquals(this, other);
}

/// <inheritdoc cref="IEntity{TKey}" />
[Serializable]
public abstract class Entity<TKey> : Entity, IEntity<TKey> where TKey : notnull
{
    /// <inheritdoc/>
    public virtual TKey Id { get; protected set; }

    protected Entity(TKey id) => Id = id;

    protected Entity() : this(default!) { }

    public override object[] GetKeys() => new object[] { Id };

    /// <inheritdoc/>
    public override string ToString() => $"[ENTITY: {GetType().Name}] Id = {Id}";

#if NETSTANDARD
    public bool Equals(TKey other) => Id.Equals(other);
#elif NETCOREAPP3_1_OR_GREATER
    public bool Equals(TKey? other) => Id.Equals(other);
#endif
}
