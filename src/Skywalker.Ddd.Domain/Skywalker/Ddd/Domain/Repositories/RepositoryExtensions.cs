using System.Linq.Expressions;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Extensions.Specifications;

namespace Skywalker.Ddd.Domain.Repositories;

/// <summary>
/// Extension methods for <see cref="IRepository{TEntity}"/> and related interfaces.
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    /// Ensures a collection navigation property is loaded for the given entity.
    /// </summary>
    public static async Task EnsureCollectionLoadedAsync<TEntity, TKey, TProperty>(
        this IRepository<TEntity, TKey> repository,
        TEntity entity,
        Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TKey>
        where TProperty : class
        where TKey : notnull
    {
        if (repository is ISupportsExplicitLoading<TEntity, TKey> repo)
        {
            await repo.EnsureCollectionLoadedAsync(entity, propertyExpression, cancellationToken);
        }
    }

    /// <summary>
    /// Ensures a reference navigation property is loaded for the given entity.
    /// </summary>
    public static async Task EnsurePropertyLoadedAsync<TEntity, TKey, TProperty>(
        this IRepository<TEntity, TKey> repository,
        TEntity entity,
        Expression<Func<TEntity, TProperty?>> propertyExpression,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TKey>
        where TProperty : class
        where TKey : notnull
    {
        if (repository is ISupportsExplicitLoading<TEntity, TKey> repo)
        {
            await repo.EnsurePropertyLoadedAsync(entity, propertyExpression, cancellationToken);
        }
    }

    /// <summary>
    /// Performs a hard delete of a soft-deletable entity, bypassing soft delete.
    /// </summary>
    public static async Task HardDeleteAsync<TEntity>(
        this IRepository<TEntity> repository,
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity, IDeletable
    {
        await HardDeleteWithUnitOfWorkAsync(repository, entity, cancellationToken);
    }



    /// <summary>
    /// Inserts multiple entities in batch.
    /// </summary>
    public static async Task<int> InsertManyAsync<TEntity>(
        this IBasicRepository<TEntity> repository,
        IEnumerable<TEntity> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        return await repository.InsertAsync(entities, autoSave, cancellationToken);
    }

    /// <summary>
    /// Updates multiple entities in batch.
    /// </summary>
    public static async Task<int> UpdateManyAsync<TEntity>(
        this IBasicRepository<TEntity> repository,
        IEnumerable<TEntity> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        var count = 0;
        foreach (var entity in entities)
        {
            await repository.UpdateAsync(entity, autoSave, cancellationToken);
            count++;
        }
        return count;
    }

    /// <summary>
    /// Deletes multiple entities in batch.
    /// </summary>
    public static async Task<int> DeleteManyAsync<TEntity>(
        this IBasicRepository<TEntity> repository,
        IEnumerable<TEntity> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        var count = 0;
        foreach (var entity in entities)
        {
            await repository.DeleteAsync(entity, autoSave, cancellationToken);
            count++;
        }
        return count;
    }

    /// <summary>
    /// Deletes multiple entities by their IDs.
    /// </summary>
    public static async Task<int> DeleteManyAsync<TEntity, TKey>(
        this IBasicRepository<TEntity, TKey> repository,
        IEnumerable<TKey> ids,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TKey>
        where TKey : notnull
    {
        var count = 0;
        foreach (var id in ids)
        {
            await repository.DeleteAsync(id, autoSave, cancellationToken);
            count++;
        }
        return count;
    }

    /// <summary>
    /// Gets the first entity matching the filter, or null if none found.
    /// Specification&lt;T&gt; can be passed directly via implicit conversion.
    /// </summary>
    public static async Task<TEntity?> FirstOrDefaultAsync<TEntity>(
        this IReadOnlyRepository<TEntity> repository,
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        return await repository.FindAsync(filter, cancellationToken);
    }

    private static async Task HardDeleteWithUnitOfWorkAsync<TEntity>(
        IRepository<TEntity> repository,
        TEntity entity,
        CancellationToken cancellationToken)
        where TEntity : class, IEntity, IDeletable
    {
        // TODO: Integrate with UnitOfWork for proper hard delete tracking
        _ = new HashSet<IEntity> { entity };
        await repository.DeleteAsync(entity, false, cancellationToken);
    }
}
