using Skywalker.Caching.Abstractions;
using Skywalker.Domain.Entities;
using Skywalker.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace Skywalker.Domain.Services
{
    public abstract class DomainService : IDomainService
    {
        protected ICachingProvider CachingProvider { get; }

        protected DomainService(ICachingProvider cachingProvider)
        {
            CachingProvider = cachingProvider;
        }
    }

    public abstract class DomainService<TEntity> : DomainService<TEntity, Guid>, IDomainService<TEntity, Guid> where TEntity : class, IEntity<Guid>
    {
        protected DomainService(IRepository<TEntity, Guid> repository, ICachingProvider cachingProvider) : base(repository, cachingProvider)
        {
        }
    }

    public abstract class DomainService<TEntity, TKey> : DomainService where TEntity : class, IEntity<TKey>
    {

        protected IRepository<TEntity, TKey> Repository { get; }

        protected DomainService(IRepository<TEntity, TKey> repository, ICachingProvider cachingProvider) : base(cachingProvider)
        {
            Repository = repository;
        }

        public Task<TEntity> GetAsync(TKey id)
        {
            return Repository.GetAsync(predicate => predicate.Id!.Equals(id));
        }

        public Task<TEntity?> FindAsync(TKey id)
        {
            return Repository.FindAsync(predicate => predicate.Id!.Equals(id));
        }
    }
}
