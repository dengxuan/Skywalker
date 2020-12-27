using Microsoft.EntityFrameworkCore;
using Skywalker.Domain.Entities;
using Skywalker.Domain.Repositories;

namespace Skywalker.Ddd.Infrastructure.Domain.Repositories
{
    public interface ISkywalkerRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        DbContext DbContext { get; }

        DbSet<TEntity> DbSet { get; }
    }

    public interface ISkywalkerRepository<TEntity, TKey> : ISkywalkerRepository<TEntity>, IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {

    }
}