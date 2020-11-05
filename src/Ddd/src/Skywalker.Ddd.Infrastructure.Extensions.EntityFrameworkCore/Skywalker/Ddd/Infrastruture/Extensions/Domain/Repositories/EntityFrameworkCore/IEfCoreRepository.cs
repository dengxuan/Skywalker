using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Domain.Entities;

namespace Skywalker.Domain.Repositories.EntityFrameworkCore
{
    public interface IEfCoreRepository<TEntity> : IDbContextRepository<TEntity> where TEntity : class, IEntity
    {

    }

    public interface IEfCoreRepository<TEntity, TKey> : IEfCoreRepository<TEntity>, IDbContextRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {

    }
}