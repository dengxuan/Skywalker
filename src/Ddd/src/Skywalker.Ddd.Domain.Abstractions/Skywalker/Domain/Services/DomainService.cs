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
        protected DomainService(IRepository<TEntity, Guid> entities) : base(entities)
        {
        }
    }

    public abstract class DomainService<TEntity, TKey> : DomainService where TEntity : class, IEntity<TKey>
    {

        protected IRepository<TEntity, TKey> Entities { get; }

        protected DomainService(IRepository<TEntity, TKey> entities)
        {
            Entities = entities;
        }

        public Task<TEntity> GetAsync(TKey id)
        {
            return Entities.GetAsync(predicate => predicate.Id!.Equals(id));
        }

        public Task<TEntity?> FindAsync(TKey id)
        {
            return Entities.FindAsync(predicate => predicate.Id!.Equals(id));
        }
    }
}
