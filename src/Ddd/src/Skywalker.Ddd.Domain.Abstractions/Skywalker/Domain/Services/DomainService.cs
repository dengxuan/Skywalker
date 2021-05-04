using Skywalker.Domain.Entities;
using Skywalker.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace Skywalker.Domain.Services
{
    public abstract class DomainService : IDomainService
    {
    }

    public abstract class DomainService<TEntity> : DomainService<TEntity, Guid>, IDomainService<TEntity, Guid> where TEntity : class, IEntity<Guid>
    {
        protected DomainService(IRepository<TEntity, Guid> repository) : base(repository)
        {
        }
    }

    public abstract class DomainService<TEntity, TKey> : DomainService where TEntity : class, IEntity<TKey>
    {

        protected IRepository<TEntity, TKey> Repository { get; }

        protected DomainService(IRepository<TEntity, TKey> repository)
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
