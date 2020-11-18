using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Infrastructure.EntityFrameworkCore
{
    public class SkywalkerEntityFrameworkCoreDatabase<TDbContext, TEntity> : ISkywalkerDatabase<TEntity> where TEntity : class, IEntity where TDbContext : SkywalkerDbContext<TDbContext>
    {
        [NotNull]
        protected TDbContext Database { get; }

        public SkywalkerEntityFrameworkCoreDatabase(ISkywalkerDbContextProvider<TDbContext> dbContextProvider)
        {
            Database = dbContextProvider.GetDbContext();
        }

        public IQueryable<TEntity> Entities => Database.Set<TEntity>().AsQueryable();

        public Task DeleteAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default)
        {
            Database.Remove(entity);
            await Database.SaveChangesAsync(cancellationToken);
        }

        public Task<TEntity> FindAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Entities.Where(predicate).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return await Entities.LongCountAsync(cancellationToken);
        }

        public async Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
        {
            List<TEntity> entities = await Entities.ToListAsync(cancellationToken);
            return entities;
        }

        public async Task<TEntity> InsertAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default)
        {
            EntityEntry<TEntity> entry = await Database.AddAsync(entity, cancellationToken);
            await Database.SaveChangesAsync(cancellationToken);
            return entry.Entity;
        }

        public async Task<int> InsertAsync([NotNull] IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await Database.AddRangeAsync(entities, cancellationToken);
            return await Database.SaveChangesAsync(cancellationToken);
        }

        public async Task<TEntity> UpdateAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default)
        {
            Database.Attach(entity).State = EntityState.Modified;
            await Database.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public IQueryable<TEntity> WithDetails(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var query = Entities;
            if (!propertySelectors.IsNullOrEmpty())
            {
                foreach (var propertySelector in propertySelectors)
                {
                    query = query.Include(propertySelector);
                }
            }
            return query;
        }
    }

    public class SkywalkerEntityFrameworkCoreDatabase<TDbContext, TEntity, TKey> : SkywalkerEntityFrameworkCoreDatabase<TDbContext, TEntity>, ISkywalkerDatabase<TEntity, TKey> where TEntity : class, IEntity<TKey> where TDbContext : SkywalkerDbContext<TDbContext>
    {
        public SkywalkerEntityFrameworkCoreDatabase(ISkywalkerDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            TEntity entity = await Entities.FirstOrDefaultAsync(predicate => predicate.Id!.Equals(id), cancellationToken);
            if (entity == null)
            {
                return;
            }
            Database.Remove(entity);
            await Database.SaveChangesAsync(cancellationToken);
        }

        public Task EnsureCollectionLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken) where TProperty : class
        {
            throw new NotImplementedException();
        }

        public Task EnsurePropertyLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyExpression, CancellationToken cancellationToken) where TProperty : class
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity> FindAsync(TKey id, CancellationToken cancellationToken = default)
        {
            TEntity entity = await Entities.FirstOrDefaultAsync(predicate => predicate.Id!.Equals(id), cancellationToken);
            return entity;
        }

        public async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
        {
            TEntity entity = await Entities.FirstOrDefaultAsync(predicate => predicate.Id!.Equals(id), cancellationToken);
            if (entity == null)
            {
                throw new EntityNotFoundException();
            }
            return entity;
        }
    }
}
