using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Skywalker;
using Skywalker.Data;
using Skywalker.Domain.Entities;
using Skywalker.Domain.Entities.Events;
using Skywalker.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Skywalker.Ddd.Infrastructure.Mongodb;

namespace Volo.Abp.Domain.Repositories.MongoDB
{
    public class MongoDbRepository<TMongoDbContext, TEntity>
        : RepositoryBase<TEntity>,
        IMongoDbRepository<TEntity>,
        IMongoQueryable<TEntity>
        where TMongoDbContext : IMongodbContext
        where TEntity : class, IEntity
    {
        public virtual IMongoCollection<TEntity> Collection => DbContext.Collection<TEntity>();

        public virtual IMongoDatabase Database => DbContext.Database;

        public virtual TMongoDbContext DbContext => DbContextProvider.GetDbContext();

        protected IMongoDbContextProvider<TMongoDbContext> DbContextProvider { get; }

        public IEntityChangeEventHelper EntityChangeEventHelper { get; set; }

        public IGuidGenerator GuidGenerator { get; set; }

        public MongoDbRepository(IMongoDbContextProvider<TMongoDbContext> dbContextProvider)
        {
            DbContextProvider = dbContextProvider;

            EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
        }

        public override async Task<TEntity> InsertAsync(
            TEntity entity,
            bool autoSave = false,
            CancellationToken cancellationToken = default)
        {
            await ApplyAbpConceptsForAddedEntityAsync(entity);

            await Collection.InsertOneAsync(
                entity,
                cancellationToken: GetCancellationToken(cancellationToken)
            );

            return entity;
        }

        public override async Task<TEntity> UpdateAsync(
            TEntity entity,
            bool autoSave = false,
            CancellationToken cancellationToken = default)
        {

            if (entity is IDeleteable softDeleteEntity && softDeleteEntity.IsDeleted)
            {
                await TriggerEntityDeleteEventsAsync(entity);
            }
            else
            {
                await TriggerEntityUpdateEventsAsync(entity);
            }

            await TriggerDomainEventsAsync(entity);

            var oldConcurrencyStamp = SetNewConcurrencyStamp(entity);

            var result = await Collection.ReplaceOneAsync(
                CreateEntityFilter(entity, true, oldConcurrencyStamp),
                entity,
                cancellationToken: GetCancellationToken(cancellationToken)
            );

            if (result.MatchedCount <= 0)
            {
                ThrowOptimisticConcurrencyException();
            }

            return entity;
        }

        public override async Task DeleteAsync(
            TEntity entity,
            bool autoSave = false,
            CancellationToken cancellationToken = default)
        {
            await ApplyAbpConceptsForDeletedEntityAsync(entity);
            var oldConcurrencyStamp = SetNewConcurrencyStamp(entity);

            if (entity is IDeleteable softDeleteEntity && !IsHardDeleted(entity))
            {
                softDeleteEntity.IsDeleted = true;
                var result = await Collection.ReplaceOneAsync(
                    CreateEntityFilter(entity, true, oldConcurrencyStamp),
                    entity,
                    cancellationToken: GetCancellationToken(cancellationToken)
                );

                if (result.MatchedCount <= 0)
                {
                    ThrowOptimisticConcurrencyException();
                }
            }
            else
            {
                var result = await Collection.DeleteOneAsync(
                    CreateEntityFilter(entity, true, oldConcurrencyStamp),
                    GetCancellationToken(cancellationToken)
                );

                if (result.DeletedCount <= 0)
                {
                    ThrowOptimisticConcurrencyException();
                }
            }
        }

        public override async Task<List<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await GetMongoQueryable().ToListAsync(GetCancellationToken(cancellationToken));
        }

        public override async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return await GetMongoQueryable().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public override async Task DeleteAsync(
            Expression<Func<TEntity, bool>> predicate,
            bool autoSave = false,
            CancellationToken cancellationToken = default)
        {
            var entities = await GetMongoQueryable()
                .Where(predicate)
                .ToListAsync(GetCancellationToken(cancellationToken));

            foreach (var entity in entities)
            {
                await DeleteAsync(entity, autoSave, cancellationToken);
            }
        }

        protected override IQueryable<TEntity> GetQueryable()
        {
            return GetMongoQueryable();
        }

        public override async Task<TEntity> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await GetMongoQueryable()
                .Where(predicate)
                .SingleOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        public virtual IMongoQueryable<TEntity> GetMongoQueryable()
        {
            return ApplyDataFilters(
                Collection.AsQueryable()
            );
        }
        protected virtual bool IsHardDeleted(TEntity entity)
        {
            return true;
            //var hardDeletedEntities = UnitOfWorkManager?.Current?.Items.GetOrDefault(UnitOfWorkItemNames.HardDeletedEntities) as HashSet<IEntity>;
            //if (hardDeletedEntities == null)
            //{
            //    return false;
            //}

            //return hardDeletedEntities.Contains(entity);
        }

        protected virtual FilterDefinition<TEntity> CreateEntityFilter(TEntity entity, bool withConcurrencyStamp = false, string concurrencyStamp = null)
        {
            throw new NotImplementedException(
                $"{nameof(CreateEntityFilter)} is not implemented for MongoDB by default. It should be overriden and implemented by the deriving class!"
            );
        }

        protected virtual async Task ApplyAbpConceptsForAddedEntityAsync(TEntity entity)
        {
            CheckAndSetId(entity);
            await TriggerEntityCreateEvents(entity);
            await TriggerDomainEventsAsync(entity);
        }

        private async Task TriggerEntityCreateEvents(TEntity entity)
        {
            await EntityChangeEventHelper.TriggerEntityCreatedEventOnUowCompletedAsync(entity);
            await EntityChangeEventHelper.TriggerEntityCreatingEventAsync(entity);
        }

        protected virtual async Task TriggerEntityUpdateEventsAsync(TEntity entity)
        {
            await EntityChangeEventHelper.TriggerEntityUpdatedEventOnUowCompletedAsync(entity);
            await EntityChangeEventHelper.TriggerEntityUpdatingEventAsync(entity);
        }

        protected virtual async Task ApplyAbpConceptsForDeletedEntityAsync(TEntity entity)
        {
            await TriggerEntityDeleteEventsAsync(entity);
            await TriggerDomainEventsAsync(entity);
        }

        protected virtual async Task TriggerEntityDeleteEventsAsync(TEntity entity)
        {
            await EntityChangeEventHelper.TriggerEntityDeletedEventOnUowCompletedAsync(entity);
            await EntityChangeEventHelper.TriggerEntityDeletingEventAsync(entity);
        }

        protected virtual void CheckAndSetId(TEntity entity)
        {
            if (entity is IEntity<Guid> entityWithGuidId)
            {
                TrySetGuidId(entityWithGuidId);
            }
        }

        protected virtual void TrySetGuidId(IEntity<Guid> entity)
        {
            if (entity.Id != default)
            {
                return;
            }

            EntityHelper.TrySetId(
                entity,
                () => GuidGenerator.Create(),
                true
            );
        }

        protected virtual Task TriggerDomainEventsAsync(object entity)
        {
            if (entity is not IGeneratesDomainEvents generatesDomainEventsEntity)
            {
                return Task.CompletedTask;
            }

            var distributedEvents = generatesDomainEventsEntity.GetDistributedEvents()?.ToArray();
            if (distributedEvents != null && distributedEvents.Any())
            {
                foreach (var distributedEvent in distributedEvents)
                {
                    //await DistributedEventBus.PublishAsync(distributedEvent.GetType(), distributedEvent);
                }

                generatesDomainEventsEntity.ClearDistributedEvents();
            }
            return Task.CompletedTask;
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

        protected virtual void ThrowOptimisticConcurrencyException()
        {
            throw new SkywalkerDbConcurrencyException("Database operation expected to affect 1 row but actually affected 0 row. Data may have been modified or deleted since entities were loaded. This exception has been thrown on optimistic concurrency check.");
        }

        /// <summary>
        /// IMongoQueryable<TEntity>
        /// </summary>
        /// <returns></returns>
        public QueryableExecutionModel GetExecutionModel()
        {
            return GetMongoQueryable().GetExecutionModel();
        }

        /// <summary>
        /// IMongoQueryable<TEntity>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IAsyncCursor<TEntity> ToCursor(CancellationToken cancellationToken = new CancellationToken())
        {
            return GetMongoQueryable().ToCursor(cancellationToken);
        }

        /// <summary>
        /// IMongoQueryable<TEntity>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IAsyncCursor<TEntity>> ToCursorAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return GetMongoQueryable().ToCursorAsync(cancellationToken);
        }
    }

    public class MongoDbRepository<TMongoDbContext, TEntity, TKey>
        : MongoDbRepository<TMongoDbContext, TEntity>,
        IMongoDbRepository<TEntity, TKey>
        where TMongoDbContext : IMongodbContext
        where TEntity : class, IEntity<TKey>
    {
        public IMongoDbRepositoryFilterer<TEntity, TKey> RepositoryFilterer { get; set; }

        public MongoDbRepository(IMongoDbContextProvider<TMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task<TEntity> GetAsync(
            TKey id,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            var entity = await FindAsync(id, includeDetails, cancellationToken);

            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TEntity), id);
            }

            return entity;
        }

        public virtual async Task<TEntity> FindAsync(
            TKey id,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await Collection
                .Find(RepositoryFilterer.CreateEntityFilter(id, true))
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        public virtual Task DeleteAsync(
            TKey id,
            bool autoSave = false,
            CancellationToken cancellationToken = default)
        {
            return DeleteAsync(x => x.Id.Equals(id), autoSave, cancellationToken);
        }

        protected override FilterDefinition<TEntity> CreateEntityFilter(TEntity entity, bool withConcurrencyStamp = false, string concurrencyStamp = null)
        {
            return RepositoryFilterer.CreateEntityFilter(entity, withConcurrencyStamp, concurrencyStamp);
        }
    }
}