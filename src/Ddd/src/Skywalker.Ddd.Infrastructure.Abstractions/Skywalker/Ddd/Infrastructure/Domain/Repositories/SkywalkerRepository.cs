using Skywalker.Data;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Domain.Entities;
using Skywalker.Domain.Entities.Events;
using Skywalker.Domain.Repositories;
using Skywalker.EventBus;
using Skywalker.EventBus.Distributed;
using Skywalker.Extensions.Timing;
using Skywalker.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Infrastructure.Domain.Repositories
{
    public class SkywalkerRepository<TEntity> : RepositoryBase<TEntity>, IRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly IClock _clock;

        private readonly IEventBus _eventBus;

        private readonly IGuidGenerator _guidGenerator;
        protected ISkywalkerDatabase<TEntity> Database { get; }

        protected IEntityChangeEventHelper EntityChangeEventHelper { get; set; }

        public SkywalkerRepository(ISkywalkerDatabase<TEntity> database, IClock clock, IGuidGenerator guidGenerator)
        {
            Database = database;
            _clock = clock;
            _guidGenerator = guidGenerator;
            _eventBus = NullEventBus.Instance;
            EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
        }

        protected virtual async Task ApplySkywalkerConceptsForAddedEntityAsync(TEntity entity)
        {
            await TriggerEntityCreateEvents(entity);
            await TriggerDomainEventsAsync(entity);
        }

        protected virtual async Task ApplySkywalkerConceptsForDeletedEntityAsync(TEntity entity)
        {
            await TriggerEntityDeleteEventsAsync(entity);
            await TriggerDomainEventsAsync(entity);
        }

        protected virtual async Task ApplySkywalkerConceptsForUpdatedEntityAsync(TEntity entity)
        {
            if (entity is IDeleteable)
            {
                await TriggerEntityDeleteEventsAsync(entity);
                await TriggerDomainEventsAsync(entity);
            }
            else
            {
                await TriggerEntityUpdateEventsAsync(entity);
                await TriggerDomainEventsAsync(entity);
            }
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

        protected virtual async Task TriggerEntityDeleteEventsAsync(TEntity entity)
        {
            await EntityChangeEventHelper.TriggerEntityDeletedEventOnUowCompletedAsync(entity);
            await EntityChangeEventHelper.TriggerEntityDeletingEventAsync(entity);
        }

        protected virtual async Task TriggerDomainEventsAsync(object entity)
        {
            if (entity is not IGeneratesDomainEvents generatesDomainEventsEntity)
            {
                return;
            }

            var distributedEvents = generatesDomainEventsEntity.GetDistributedEvents()?.ToArray();
            if (distributedEvents != null && distributedEvents.Any())
            {
                foreach (var distributedEvent in distributedEvents)
                {
                    await _eventBus.PublishAsync(distributedEvent.GetType(), distributedEvent);
                }

                generatesDomainEventsEntity.ClearDistributedEvents();
            }
        }

        protected void TrySetGuidId(IEntity<Guid> entity)
        {
            if (entity.Id != default)
            {
                return;
            }

            var idProperty = entity.GetType().GetProperty("Id");

            //Check for DatabaseGeneratedAttribute
            var dbGeneratedAttr = ReflectionHelper.GetSingleAttributeOrDefault<DatabaseGeneratedAttribute>(idProperty!);

            if (dbGeneratedAttr != null && dbGeneratedAttr.DatabaseGeneratedOption != DatabaseGeneratedOption.None)
            {
                return;
            }

            EntityHelper.TrySetId(entity, () => _guidGenerator.Create(), true
            );
        }

        protected virtual void SetConcurrencyStampIfNull(TEntity entity)
        {
            if (entity is not IHasConcurrencyStamp hasConcurrencyStamp)
            {
                return;
            }

            if (hasConcurrencyStamp.ConcurrencyStamp != null)
            {
                return;
            }

            hasConcurrencyStamp.ConcurrencyStamp = Guid.NewGuid().ToString("N");
        }


        protected override IQueryable<TEntity> GetQueryable()
        {
            return Database.Entities;
        }

        public override IQueryable<TEntity> WithDetails(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return Database.WithDetails(propertySelectors);
        }

        public override Task<TEntity> FindAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Database.FindAsync(predicate, cancellationToken);
        }

        public override async Task DeleteAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            await Database.DeleteAsync(predicate, cancellationToken);
        }

        public override async Task<TEntity> InsertAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity is IHasCreationTime objectWithCreationTime)
            {
                objectWithCreationTime.CreationTime = _clock.Now;
            }

            if (entity is IEntity<Guid> entityWithGuidId)
            {
                TrySetGuidId(entityWithGuidId);
            }

            SetConcurrencyStampIfNull(entity);

            await ApplySkywalkerConceptsForAddedEntityAsync(entity);

            return await Database.InsertAsync(entity, cancellationToken);
        }

        public override async Task<int> InsertAsync([NotNull] IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                if (entity is IHasCreationTime objectWithCreationTime)
                {
                    objectWithCreationTime.CreationTime = _clock.Now;
                }

                if (entity is IEntity<Guid> entityWithGuidId)
                {
                    TrySetGuidId(entityWithGuidId);
                }

                SetConcurrencyStampIfNull(entity);
                await ApplySkywalkerConceptsForAddedEntityAsync(entity);
            }
            return await Database.InsertAsync(entities, cancellationToken);
        }

        public override async Task<TEntity> UpdateAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default)
        {
            await ApplySkywalkerConceptsForUpdatedEntityAsync(entity);
            return await Database.UpdateAsync(entity, cancellationToken);
        }

        public override async Task DeleteAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default)
        {
            await ApplySkywalkerConceptsForDeletedEntityAsync(entity);
            await Database.DeleteAsync(entity, cancellationToken);
        }

        public override Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
        {
            return Database.GetListAsync(cancellationToken);
        }

        public override Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return Database.GetCountAsync(cancellationToken);
        }
    }

    public class SkywalkerRepository<TEntity, TKey> : SkywalkerRepository<TEntity>, IRepository<TEntity, TKey>, ISupportsExplicitLoading<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        public SkywalkerRepository(ISkywalkerDatabase<TEntity, TKey> database, IClock clock, IGuidGenerator guidGenerator)
            : base(database, clock, guidGenerator)
        {

        }

        public async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await FindAsync(id, GetCancellationToken(cancellationToken));
            if (entity == null)
            {
                return;
            }
            await ApplySkywalkerConceptsForDeletedEntityAsync(entity);
            await Database.DeleteAsync(entity, GetCancellationToken(cancellationToken));
        }

        public Task EnsureCollectionLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken) where TProperty : class
        {
            throw new NotImplementedException();
        }

        public Task EnsurePropertyLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyExpression, CancellationToken cancellationToken) where TProperty : class
        {
            throw new NotImplementedException();
        }

        public Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Database.Entities.FirstOrDefault(predicate => predicate.Id!.Equals(id)));
        }

        public async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
        {
            TEntity? entity = await FindAsync(id, cancellationToken);
            if (entity == null)
            {
                throw new EntityNotFoundException();
            }
            return entity;
        }
    }
}
