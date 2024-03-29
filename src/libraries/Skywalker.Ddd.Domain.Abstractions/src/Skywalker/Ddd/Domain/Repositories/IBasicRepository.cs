﻿using System.Linq.Expressions;
using Skywalker.Ddd.Domain.Entities;

namespace Skywalker.Ddd.Domain.Repositories;

public interface IBasicRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class, IEntity
{
    /// <summary>
    /// Inserts a new entity.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <param name="entity">Inserted entity</param>
    Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts a new entity from entity collection.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <param name="entities"></param>
    Task<int> InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity. 
    /// </summary>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <param name="entity">Entity</param>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entity">Entity to be deleted</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes many entities by function.
    /// Notice that: All entities fits to given filter are retrieved and deleted.
    /// This may cause major performance problems if there are too many entities with
    /// given filter.
    /// </summary>
    /// <param name="filter">A condition to filter entities</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task DeleteAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
}

public interface IBasicRepository<TEntity, TKey> : IBasicRepository<TEntity>, IReadOnlyRepository<TEntity, TKey> where TEntity : class, IEntity<TKey> where TKey : notnull
{
    /// <summary>
    /// Deletes an entity by primary key.
    /// </summary>
    /// <param name="id">Primary key of the entity</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);
}
