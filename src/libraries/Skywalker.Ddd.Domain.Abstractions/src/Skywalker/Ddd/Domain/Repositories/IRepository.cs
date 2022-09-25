using System.Linq.Expressions;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Domain.Repositories;

/// <summary>
/// Just to mark a class as repository.
/// </summary>
public interface IRepository
{
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>, IBasicRepository<TEntity> where TEntity : class, IEntity
{

    /// <summary>
    /// Determine if an entity with a given primary key exists
    /// </summary>
    /// <param name="filter">The condition of the entity to determine</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total count of all entities.
    /// </summary>
    /// <returns></returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total count of all entities.
    /// </summary>
    /// <returns></returns>
    Task<long> LongCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total count of entities with filter
    /// </summary>
    /// <param name="filter">The entities filter</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total count of entities with filter
    /// </summary>
    /// <param name="filter">The entities filter</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<long> LongCountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKey"></typeparam>
public interface IRepository<TEntity, TKey> : IRepository<TEntity>, IReadOnlyRepository<TEntity, TKey>, IBasicRepository<TEntity, TKey> where TEntity : class, IEntity<TKey> where TKey : notnull
{
    /// <summary>
    /// Determine if an entity with a given primary key exists
    /// </summary>
    /// <param name="id">Primary key of the entity to determine</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<bool> AnyAsync(TKey id, CancellationToken cancellationToken = default);
}
