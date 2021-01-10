using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.UnitOfWork.Abstractions;
using Skywalker.Domain.Entities;
using Skywalker.Extensions.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Domain.Repositories
{
    public abstract class BasicRepositoryBase<TEntity> : IBasicRepository<TEntity>, IServiceProviderAccessor where TEntity : class, IEntity
    {
        public IServiceProvider? ServiceProvider { get; set; }

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

        public IUnitOfWorkManager? UnitOfWorkManager { get; set; }

        protected BasicRepositoryBase()
        {
            CancellationTokenProvider = NullCancellationTokenProvider.Instance;
        }


        protected virtual CancellationToken GetCancellationToken(CancellationToken preferredValue = default)
        {
            return CancellationTokenProvider.FallbackToProvider(preferredValue);
        }

        protected abstract IQueryable<TEntity> GetQueryable();

        public abstract Task<TEntity> InsertAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default);

        public abstract Task InsertAsync([NotNull] IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        public abstract Task<TEntity> UpdateAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default);

        public abstract Task DeleteAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default);

        public abstract Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

        public abstract Task<long> GetCountAsync(CancellationToken cancellationToken = default);
    }

    public abstract class BasicRepositoryBase<TEntity, TKey> : BasicRepositoryBase<TEntity>, IBasicRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        public virtual async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await FindAsync(id, cancellationToken);

            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TEntity), id!);
            }

            return entity;
        }

        public abstract Task<TEntity> FindAsync(TKey id, CancellationToken cancellationToken = default);

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
}
