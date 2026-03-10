using System.Linq.Expressions;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Uow;
using Skywalker.Extensions.Specifications;

namespace Skywalker.Ddd.Domain.Repositories;

/// <summary>
/// Just to mark a class as repository.
/// </summary>
[UnitOfWork]
public interface IRepository
{

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
}

/// <summary>
/// A repository interface that provides full CRUD operations for entities.
/// </summary>
/// <typeparam name="TEntity">The type of entity</typeparam>
public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>, IBasicRepository<TEntity> where TEntity : class, IEntity
{
    /// <summary>
    /// Determine if any entity matches the given <paramref name="filter"/>.
    /// </summary>
    /// <param name="filter">The condition to determine</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>True if any entity matches, otherwise false</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determine if any entity matches the given <paramref name="specification"/>.
    /// </summary>
    /// <param name="specification">The specification to determine</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>True if any entity matches, otherwise false</returns>
    Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total count of entities matching the <paramref name="filter"/>.
    /// </summary>
    /// <param name="filter">The entities filter</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>Count of matching entities</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total count of entities matching the <paramref name="specification"/>.
    /// </summary>
    /// <param name="specification">The specification to filter entities</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>Count of matching entities</returns>
    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total count of entities matching the <paramref name="filter"/> as long.
    /// </summary>
    /// <param name="filter">The entities filter</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>Long count of matching entities</returns>
    Task<long> LongCountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total count of entities matching the <paramref name="specification"/> as long.
    /// </summary>
    /// <param name="specification">The specification to filter entities</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>Long count of matching entities</returns>
    Task<long> LongCountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
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
