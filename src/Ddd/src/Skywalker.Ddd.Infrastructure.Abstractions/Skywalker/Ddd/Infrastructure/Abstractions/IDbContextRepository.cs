using Skywalker.Domain.Entities;
using Skywalker.Domain.Repositories;

namespace Skywalker.Ddd.Infrastructure.Abstractions
{
    public interface IDbContextRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        IDbContext DbContext { get; }

        IDataCollection<TEntity> DataCollection { get; }
    }

    public interface IDbContextRepository<TEntity, TKey> : IDbContextRepository<TEntity>, IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {

    }
}