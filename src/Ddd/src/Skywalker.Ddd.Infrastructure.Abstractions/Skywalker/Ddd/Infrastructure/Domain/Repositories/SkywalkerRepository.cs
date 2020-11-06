using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Domain.Entities;
using Skywalker.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Infrastructure.Domain.Repositories
{
    public class SkywalkerRepository<TEntity> : RepositoryBase<TEntity>, ISkywalkerRepository<TEntity> where TEntity : class, IEntity
    {
        protected ISkywalkerDatabase<TEntity> Database { get; }

        //public SkywalkerRepository(ISkywalkerDatabase<TEntity> database)
        //{
        //    Database = database;
        //}

        protected override IQueryable<TEntity> GetQueryable()
        {
            return Database.Entities;
        }

        public override Task<TEntity> FindAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return Database.FindAsync(predicate, includeDetails, cancellationToken);
        }

        public override Task DeleteAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            return Database.DeleteAsync(predicate, autoSave, cancellationToken);
        }

        public override Task<TEntity> InsertAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            return Database.InsertAsync(entity, autoSave, cancellationToken);
        }

        public override Task<TEntity> UpdateAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            return Database.UpdateAsync(entity, autoSave, cancellationToken);
        }

        public override Task DeleteAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            return Database.DeleteAsync(entity, autoSave, cancellationToken);
        }

        public override Task<List<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return Database.GetListAsync(includeDetails, cancellationToken);
        }

        public override Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return Database.GetCountAsync(cancellationToken);
        }
    }

    public class SkywalkerRepository<TEntity, TKey> : SkywalkerRepository<TEntity>,
        ISkywalkerRepository<TEntity, TKey>,
        ISupportsExplicitLoading<TEntity, TKey>

        where TEntity : class, IEntity<TKey>
    {
        //public SkywalkerRepository(ISkywalkerDatabase<TEntity, TKey> database)
        //    : base(database)
        //{

        //}

        public async Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var entity = await FindAsync(id, false, GetCancellationToken(cancellationToken));
            if (entity == null)
            {
                return;
            }
            await Database.DeleteAsync(entity, autoSave, GetCancellationToken(cancellationToken));
        }

        public Task EnsureCollectionLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken) where TProperty : class
        {
            throw new NotImplementedException();
        }

        public Task EnsurePropertyLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyExpression, CancellationToken cancellationToken) where TProperty : class
        {
            throw new NotImplementedException();
        }

        public Task<TEntity?> FindAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Database.Entities.FirstOrDefault(predicate => predicate.Id!.Equals(id)));
        }

        public async Task<TEntity> GetAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            TEntity? entity = await FindAsync(id, includeDetails, cancellationToken);
            if (entity == null)
            {
                throw new EntityNotFoundException();
            }
            return entity;
        }
    }
}
