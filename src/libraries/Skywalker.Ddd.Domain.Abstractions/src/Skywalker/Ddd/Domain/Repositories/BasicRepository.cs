using System.Collections;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.Data.Filtering;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Extensions.Threading;

namespace Skywalker.Domain.Repositories;

public abstract class BasicRepository<TEntity> : IBasicRepository<TEntity>, IServiceProviderAccessor where TEntity : class, IEntity
{

    public IServiceProvider? ServiceProvider { get; set; }

    public IDataFilter? DataFilter { get; set; }

    public ICancellationTokenProvider CancellationTokenProvider { get; set; }

    public Type ElementType => GetQueryable().ElementType;

    public Expression Expression => GetQueryable().Expression;

    public IQueryProvider Provider => GetQueryable().Provider;

    public IEnumerator<TEntity> GetEnumerator()
    {
        return GetQueryable().GetEnumerator();
    }

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

    protected abstract IQueryable<TEntity> GetQueryable();

    public abstract Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    public abstract Task<int> InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    public abstract Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    public abstract Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    public abstract Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

    public abstract Task<long> GetCountAsync(CancellationToken cancellationToken = default);

    public abstract Task<List<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, CancellationToken cancellationToken = default);

    public abstract Task<List<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> expression, int skipCount, int maxResultCount, string sorting, CancellationToken cancellationToken = default);

    public abstract Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(predicate, cancellationToken);

        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(TEntity));
        }

        return entity;
    }

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

public abstract class BasicRepository<TEntity, TKey> : BasicRepository<TEntity>, IBasicRepository<TEntity, TKey> where TEntity : class, IEntity<TKey> where TKey : notnull
{
    public virtual async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, cancellationToken);

        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(TEntity), id);
        }

        return entity;
    }

    public abstract Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);

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
