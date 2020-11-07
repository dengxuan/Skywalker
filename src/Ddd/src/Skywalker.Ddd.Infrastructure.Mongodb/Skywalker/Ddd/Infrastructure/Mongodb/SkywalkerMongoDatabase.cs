using MongoDB.Driver;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.MongoDB;

namespace Skywalker.Ddd.Infrastructure.Mongodb
{
    public class SkywalkerMongoDatabase<TDbContext, TEntity> : ISkywalkerDatabase<TEntity> where TEntity : IEntity where TDbContext : IMongodbContext
    {
        protected TDbContext Database { get; }

        public IQueryable<TEntity> Entities => Database.Collection<TEntity>().AsQueryable();

        public SkywalkerMongoDatabase(IMongoDbContextProvider<TDbContext> dbContextProvider)
        {
            Database = dbContextProvider.GetDbContext();
        }

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

        public Task<TEntity> InsertAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> UpdateAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public class SkywalkerMongoDatabase<TDbContext, TEntity, TKey> : SkywalkerMongoDatabase<TDbContext, TEntity>, ISkywalkerDatabase<TEntity, TKey> where TEntity : class, IEntity<TKey> where TDbContext : IMongodbContext
    {
        public SkywalkerMongoDatabase(IMongoDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
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
