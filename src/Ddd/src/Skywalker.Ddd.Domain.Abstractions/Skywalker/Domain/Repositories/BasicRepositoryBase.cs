using Microsoft.Extensions.DependencyInjection;
using Skywalker.Domain.Entities;
using Skywalker.Extensions.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Domain.Repositories
{
    public abstract class BasicRepositoryBase<TEntity> : IBasicRepository<TEntity>, IServiceProviderAccessor, ITransientDependency where TEntity : class, IEntity
    {
        public IServiceProvider? ServiceProvider { get; set; }

        public ICancellationTokenProvider CancellationTokenProvider { get; set; }

        protected BasicRepositoryBase()
        {
            CancellationTokenProvider = NullCancellationTokenProvider.Instance;
        }

        public abstract Task<TEntity> InsertAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

        public abstract Task<TEntity> UpdateAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

        public abstract Task DeleteAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

        public abstract Task<List<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default);

        public abstract Task<long> GetCountAsync(CancellationToken cancellationToken = default);

        protected virtual CancellationToken GetCancellationToken(CancellationToken preferredValue = default)
        {
            return CancellationTokenProvider.FallbackToProvider(preferredValue);
        }
    }

    public abstract class BasicRepositoryBase<TEntity, TKey> : BasicRepositoryBase<TEntity>, IBasicRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        public virtual async Task<TEntity> GetAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            var entity = await FindAsync(id, includeDetails, cancellationToken);

            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TEntity), id!);
            }

            return entity;
        }

        public abstract Task<TEntity> FindAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default);

        public virtual async Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var entity = await FindAsync(id, cancellationToken: cancellationToken);
            if (entity == null)
            {
                return;
            }

            await DeleteAsync(entity, autoSave, cancellationToken);
        }
    }
}
