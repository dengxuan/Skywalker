using Skywalker.Caching.Abstractions;
using Skywalker.Domain.Entities;
using Skywalker.Domain.Repositories;

namespace Skywalker.Domain.Services
{
    public abstract class DomainService : IDomainService
    {
        protected ICachingProvider CachingProvider { get; }

        protected IRepository Repository { get; }

        protected DomainService(IRepository repository, ICachingProvider cachingProvider)
        {
            Repository = repository;
            CachingProvider = cachingProvider;
        }
    }

    public abstract class DomainService<TEntity> : DomainService, IDomainService<TEntity> where TEntity : class, IEntity
    {
        protected DomainService(IRepository repository, ICachingProvider cachingProvider) : base(repository, cachingProvider)
        {
        }
    }

    public abstract class DomainService<TEntity, TKey> : DomainService<TEntity> where TEntity : class, IEntity<TKey>
    {
        protected DomainService(IRepository repository, ICachingProvider cachingProvider) : base(repository, cachingProvider)
        {
        }
    }
}
