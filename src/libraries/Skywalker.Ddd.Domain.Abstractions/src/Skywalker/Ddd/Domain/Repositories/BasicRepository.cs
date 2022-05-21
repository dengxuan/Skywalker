using System.Collections;
using System.Linq.Expressions;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.Data.Filtering;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Extensions.Collections.Generic;
using Skywalker.Extensions.DependencyInjection.Abstractions;
using Skywalker.Extensions.Threading;

namespace Skywalker.Ddd.Domain.Repositories;

/// <summary>
/// <inheritdoc/>
/// </summary>
public abstract class BasicRepository<TEntity> : IBasicRepository<TEntity>, IServiceProviderAccessor where TEntity : class, IEntity
{

    public IServiceProvider? ServiceProvider { get; set; }

    public IDataFilter? DataFilter { get; set; }

    public ICancellationTokenProvider CancellationTokenProvider { get; set; }

    public Type ElementType => GetQueryable().ElementType;

    public Expression Expression => GetQueryable().Expression;

    public IQueryProvider Provider => GetQueryable().Provider;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerator<TEntity> GetEnumerator()
    {
        return GetQueryable().GetEnumerator();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetQueryable().GetEnumerator();
    }

    protected BasicRepository()
    {
        CancellationTokenProvider = NullCancellationTokenProvider.Instance;
    }


    protected virtual CancellationToken GetCancellationToken(CancellationToken preferredValue = default)
    {
        return CancellationTokenProvider.FallbackToProvider(preferredValue);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected abstract IQueryable<TEntity> GetQueryable();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract Task<int> InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract Task<long> LongCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract Task<long> LongCountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, string sorting, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> expression, int skip, int limit, string sorting, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> expression, int skip, int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(predicate, cancellationToken);

        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(TEntity));
        }

        return entity;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    protected virtual TQueryable ApplyDataFilters<TQueryable>(TQueryable query) where TQueryable : IQueryable<TEntity>
    {
        if (typeof(IDeleteable).IsAssignableFrom(typeof(TEntity)))
        {
            query = (TQueryable)query.WhereIf(DataFilter!.IsEnabled<IDeleteable>(), e => ((IDeleteable)e).IsDeleted == false);
        }

        return query;
    }
}

/// <summary>
/// <inheritdoc/>
/// </summary>
public abstract class BasicRepository<TEntity, TKey> : BasicRepository<TEntity>, IBasicRepository<TEntity, TKey> where TEntity : class, IEntity<TKey> where TKey : notnull
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, cancellationToken);

        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(TEntity), id);
        }

        return entity;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            return;
        }

        await DeleteAsync(entity, cancellationToken);
    }
}
