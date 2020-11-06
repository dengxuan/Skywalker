using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Ddd.Infrastruture.Extensions.EntityFrameworkCore;
using Skywalker.Domain.Entities;
using Skywalker.EntityFrameworkCore;
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

        public Task DeleteAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FindAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return Entities.Where(predicate).FirstOrDefaultAsync(cancellationToken);
            throw new NotImplementedException();
        }

        public Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity> InsertAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            EntityEntry<TEntity> entry = await Database.AddAsync(entity, cancellationToken);
            if (autoSave)
            {
                await Database.SaveChangesAsync(cancellationToken);
            }
            return entry.Entity;
        }

        public Task<TEntity> UpdateAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public class SkywalkerEntityFrameworkCoreDatabase<TDbContext, TEntity, TKey> : SkywalkerEntityFrameworkCoreDatabase<TDbContext, TEntity>, ISkywalkerDatabase<TEntity, TKey> where TEntity : class, IEntity<TKey> where TDbContext : SkywalkerDbContext<TDbContext>
    {
        public SkywalkerEntityFrameworkCoreDatabase(ISkywalkerDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task EnsureCollectionLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken) where TProperty : class
        {
            throw new NotImplementedException();
        }

        public Task EnsurePropertyLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyExpression, CancellationToken cancellationToken) where TProperty : class
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FindAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
