using System.Linq.Expressions;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Extensions.Collections.Generic;

namespace Skywalker.Ddd.Domain.Repositories;

public interface IReadOnlyRepository<TEntity> : IRepository, IQueryable<TEntity> where TEntity : class, IEntity
{

    /// <summary>
    /// Gets a list of all the entities.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>Entity</returns>
    Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total count of all entities.
    /// </summary>
    /// <returns></returns>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total count of all entities.
    /// </summary>
    /// <returns></returns>
    Task<long> GetLongCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total count of a condition to filter entities.
    /// </summary>
    /// <param name="expression">A condition to filter entities</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<int> GetCountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total count of a condition to filter entities.
    /// </summary>
    /// <param name="expression">A condition to filter entities</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<long> GetLongCountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged list of the entities sorted by default
    /// if entity is <see cref="IHasCreationTime"/> the sorting by <see cref="IHasCreationTime.CreationTime"/>
    /// else sorting by <see cref="IEntity{TKey}.Id"/>
    /// </summary>
    /// <param name="skip">To skiping count</param>
    /// <param name="limit">A limit of the page size</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>Entities with no more than <paramref name="limit"/> after skipping a <paramref name="skip"/> of rows</returns>
    Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged list of the entities sorted by <paramref name="sorting"/>
    /// </summary>
    /// <param name="skip">To skiping count</param>
    /// <param name="limit">A limit of the page size</param>
    /// <param name="sorting">Sorting rules for this pagination</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>Entities with no more than <paramref name="limit"/> after skipping a <paramref name="skip"/> of rows</returns>
    Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, string sorting, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged list of the entities by the given <paramref name="predicate"/> and sorted by default.
    /// if entity is <see cref="IHasCreationTime"/> the sorting by <see cref="IHasCreationTime.CreationTime"/>
    /// else sorting by <see cref="IEntity{TKey}.Id"/>
    /// </summary>
    /// <param name="expression">A condition to filter entities</param>
    /// <param name="skip">To skiping count</param>
    /// <param name="limit">A limit of the page size</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> expression, int skip, int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged list of the entities by the given <paramref name="predicate"/> and sorted by <paramref name="sorting"/>
    /// </summary>
    /// <param name="expression">A condition to filter entities</param>
    /// <param name="skip">To skiping count</param>
    /// <param name="limit">A limit of the page size</param>
    /// <param name="sorting">Sorting rules for this pagination</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>Entities with no more than <paramref name="limit"/> after skipping a <paramref name="skip"/> of rows</returns>
    Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> expression, int skip, int limit, string sorting, CancellationToken cancellationToken = default);
}

public interface IReadOnlyRepository<TEntity, TKey> : IReadOnlyRepository<TEntity> where TEntity : class, IEntity<TKey> where TKey : notnull
{
    /// <summary>
    /// Gets an entity with given primary key.
    /// Throws <see cref="EntityNotFoundException"/> if can not find an entity with given id.
    /// </summary>
    /// <param name="id">Primary key of the entity to get</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>Entity</returns>
    Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity with given primary key or null if not found.
    /// </summary>
    /// <param name="id">Primary key of the entity to get</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>Entity or null</returns>
    Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);
}
