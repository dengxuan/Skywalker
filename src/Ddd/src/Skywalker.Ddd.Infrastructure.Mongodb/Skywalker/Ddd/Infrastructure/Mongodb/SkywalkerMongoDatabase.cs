using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Skywalker.Data;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Infrastructure.Mongodb
{
    public class SkywalkerMongoDatabase<TDbContext, TEntity> : ISkywalkerDatabase<TEntity> where TEntity : IEntity where TDbContext : ISkywalkerContext
    {
        protected TDbContext Database { get; }

        protected IMongoQueryable<TEntity> Queryable => Database.Collection<TEntity>().AsQueryable();

        public IQueryable<TEntity> Entities => Database.Collection<TEntity>().AsQueryable();

        public SkywalkerMongoDatabase(ISkywalkerContextProvider<TDbContext> dbContextProvider)
        {
            Database = dbContextProvider.GetDbContext();
        }


        protected virtual void ThrowOptimisticConcurrencyException()
        {
            throw new SkywalkerDbConcurrencyException("Database operation expected to affect 1 row but actually affected 0 row. Data may have been modified or deleted since entities were loaded. This exception has been thrown on optimistic concurrency check.");
        }

        protected virtual FilterDefinition<TEntity> CreateEntityFilter(TEntity entity, bool withConcurrencyStamp = false, string? concurrencyStamp = null)
        {
            throw new NotImplementedException(
                $"{nameof(CreateEntityFilter)} is not implemented for MongoDB by default. It should be overriden and implemented by the deriving class!"
            );
        }

        /// <summary>
        /// Sets a new <see cref="IHasConcurrencyStamp.ConcurrencyStamp"/> value
        /// if given entity implements <see cref="IHasConcurrencyStamp"/> interface.
        /// Returns the old <see cref="IHasConcurrencyStamp.ConcurrencyStamp"/> value.
        /// </summary>
        protected virtual string? SetNewConcurrencyStamp(TEntity entity)
        {
            if (entity is not IHasConcurrencyStamp concurrencyStampEntity)
            {
                return null;
            }

            var oldConcurrencyStamp = concurrencyStampEntity.ConcurrencyStamp;
            concurrencyStampEntity.ConcurrencyStamp = Guid.NewGuid().ToString("N");
            return oldConcurrencyStamp;
        }

        public async Task DeleteAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = Entities.Where(predicate).ToList();

            foreach (var entity in entities)
            {
                await DeleteAsync(entity, cancellationToken);
            }
        }

        public async Task DeleteAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default)
        {
            var oldConcurrencyStamp = SetNewConcurrencyStamp(entity);
            if (entity is IDeleteable deleteable)
            {
                deleteable.IsDeleted = true;
                var result = await Database.Collection<TEntity>().ReplaceOneAsync(CreateEntityFilter(entity, true, oldConcurrencyStamp!), entity, cancellationToken: cancellationToken);

                if (result.MatchedCount <= 0)
                {
                    ThrowOptimisticConcurrencyException();
                }
            }
            else
            {
                var result = await Database.Collection<TEntity>().DeleteOneAsync(CreateEntityFilter(entity, true, oldConcurrencyStamp), cancellationToken);

                if (result.DeletedCount <= 0)
                {
                    ThrowOptimisticConcurrencyException();
                }
            }
        }

        public Task<TEntity> FindAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            TEntity entity = Entities.Where(predicate).FirstOrDefault();
            return Task.FromResult(entity!);
        }

        public async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return await Queryable.LongCountAsync(cancellationToken);
        }

        public async Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
        {
            return await Queryable.ToListAsync(cancellationToken);
        }

        public async Task<TEntity> InsertAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default)
        {
            await Database.Collection<TEntity>().InsertOneAsync(entity, cancellationToken: cancellationToken);
            return entity;
        }


        public async Task<int> InsertAsync([NotNull] IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await Database.Collection<TEntity>().InsertManyAsync(entities, cancellationToken: cancellationToken);
            return entities.Count();
        }

        public async Task<TEntity> UpdateAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default)
        {
            var oldConcurrencyStamp = SetNewConcurrencyStamp(entity);

            var result = await Database.Collection<TEntity>().ReplaceOneAsync(CreateEntityFilter(entity, true, oldConcurrencyStamp), entity, cancellationToken: cancellationToken);

            if (result.MatchedCount <= 0)
            {
                ThrowOptimisticConcurrencyException();
            }

            return entity;
        }
    }

    public class SkywalkerMongoDatabase<TDbContext, TEntity, TKey> : SkywalkerMongoDatabase<TDbContext, TEntity>, ISkywalkerDatabase<TEntity, TKey> where TEntity : class, IEntity<TKey> where TDbContext : ISkywalkerContext
    {
        public SkywalkerMongoDatabase(ISkywalkerContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            TEntity entity = await FindAsync(predicate => predicate.Id!.Equals(id), cancellationToken);
            if (entity == null)
            {
                return;
            }
            await DeleteAsync(entity, cancellationToken);
        }

        public Task EnsureCollectionLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken) where TProperty : class
        {
            throw new NotImplementedException();
        }

        public Task EnsurePropertyLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyExpression, CancellationToken cancellationToken) where TProperty : class
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FindAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return FindAsync(predicate => predicate.Id!.Equals(id), cancellationToken);
        }

        public async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
        {
            TEntity entity = await FindAsync(id, cancellationToken);
            if (entity == null)
            {
                throw new EntityNotFoundException();
            }
            return entity;
        }
    }
}
