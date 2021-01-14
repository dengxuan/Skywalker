using Microsoft.EntityFrameworkCore;
using Skywalker.Data;
using Skywalker.Ddd.EntityFrameworkCore;
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

namespace Skywalker.Ddd.Domain.Repositories
{
    public class SkywalkerRepository<TDbContext, TEntity> : RepositoryBase<TEntity>, ISkywalkerRepository<TEntity>, IAsyncEnumerable<TEntity>
        where TDbContext : ISkywalkerDbContext
        where TEntity : class, IEntity
    {
        private readonly IClock _clock;

        private readonly IEventBus _eventBus;

        private readonly IGuidGenerator _guidGenerator;

        private readonly IDbContextProvider<TDbContext> _dbContextProvider;

        public virtual DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

        DbContext ISkywalkerRepository<TEntity>.DbContext => DbContext.As<DbContext>();

        protected virtual TDbContext DbContext => _dbContextProvider.GetDbContext();

        protected IEntityChangeEventHelper EntityChangeEventHelper { get; set; }

        public SkywalkerRepository(IDbContextProvider<TDbContext> dbContextProvider, IClock clock, IGuidGenerator guidGenerator)
        {
            _dbContextProvider = dbContextProvider;
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

            EntityHelper.TrySetId(entity, () => _guidGenerator.Create(), true);
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
            return DbSet.AsQueryable();
        }

        public override async Task<TEntity> FindAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(predicate).SingleOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        public override async Task DeleteAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await GetQueryable().Where(predicate).ToListAsync(GetCancellationToken(cancellationToken));

            foreach (var entity in entities)
            {
                DbSet.Remove(entity);
            }
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

            var savedEntity = DbSet.Add(entity).Entity;

            return savedEntity;
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
            await DbSet.AddRangeAsync(entities, GetCancellationToken(cancellationToken));

            return entities.Count();
        }

        public override async Task<TEntity> UpdateAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default)
        {
            await ApplySkywalkerConceptsForUpdatedEntityAsync(entity);

            DbContext.Attach(entity);

            var updatedEntity = DbContext.Update(entity).Entity;

            return updatedEntity;
        }

        public override async Task DeleteAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default)
        {
            await ApplySkywalkerConceptsForDeletedEntityAsync(entity);
            DbSet.Remove(entity);
        }

        public override Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
        {
            return DbSet.ToListAsync(GetCancellationToken(cancellationToken));
        }

        public override Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return DbSet.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return DbSet.AsAsyncEnumerable().GetAsyncEnumerator(cancellationToken);
        }


        public async Task EnsureCollectionLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken) where TProperty : class
        {
            await DbContext
                .Entry(entity)
                .Collection(propertyExpression)
                .LoadAsync(GetCancellationToken(cancellationToken));
        }

        public async Task EnsurePropertyLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyExpression, CancellationToken cancellationToken) where TProperty : class
        {
            await DbContext
                .Entry(entity)
                .Reference(propertyExpression)
                .LoadAsync(GetCancellationToken(cancellationToken));
        }
    }

    public class SkywalkerRepository<TDbContext, TEntity, TKey> : SkywalkerRepository<TDbContext, TEntity>, ISkywalkerRepository<TEntity, TKey>, ISupportsExplicitLoading<TEntity, TKey>
        where TDbContext : ISkywalkerDbContext
        where TEntity : class, IEntity<TKey>
    {
        public SkywalkerRepository(IDbContextProvider<TDbContext> dbContextProvider, IClock clock, IGuidGenerator guidGenerator)
            : base(dbContextProvider, clock, guidGenerator)
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
            await DeleteAsync(entity, GetCancellationToken(cancellationToken));
        }

        public async Task<TEntity> FindAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await DbSet.FindAsync(new object[] { id! }, GetCancellationToken(cancellationToken));
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
