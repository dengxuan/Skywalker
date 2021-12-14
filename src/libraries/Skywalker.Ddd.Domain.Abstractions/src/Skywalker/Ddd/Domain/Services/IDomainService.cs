using Skywalker.Domain.Entities;

namespace Skywalker.Domain.Services;

/// <summary>
/// This interface can be implemented by all domain services to identify them by convention.
/// Just to mark a class as domain service
/// </summary>
public interface IDomainService/* : ITransientDependency*/
{
}

/// <summary>
/// This interface can be implemented by all domain services to identify them by convention.
/// </summary>
public interface IDomainService<TEntity> : IDomainService<TEntity, Guid> where TEntity : class, IEntity<Guid>
{
}

/// <summary>
/// This interface can be implemented by all domain services to identify them by convention.
/// </summary>
public interface IDomainService<TEntity, TKey> : IDomainService where TEntity : class, IEntity<TKey> where TKey : notnull
{
    /// <summary>
    /// Gets an entity with given primary key.
    /// Throws <see cref="EntityNotFoundException"/> if can not find an entity with given id.
    /// </summary>
    /// <param name="id">Primary key of the entity to get</param>
    /// <returns>Entity <see cref="TEntity"/></returns>
    Task<TEntity> GetAsync(TKey id);

    /// <summary>
    /// Gets an entity with given id or null if not found.
    /// </summary>
    /// <param name="id">Primary key of the entity to get</param>
    /// <returns>Entity <see cref="TEntity"/> or null</returns>
    Task<TEntity?> FindAsync(TKey id);
}
