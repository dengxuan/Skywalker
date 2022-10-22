using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Entities.Events;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.EventBus;
using Skywalker.EventBus.Abstractions;
using Skywalker.Extensions.Collections.Generic;
using Skywalker.Extensions.Timezone;
using Skywalker.Identifier.Abstractions;

namespace Skywalker.Ddd.EntityFrameworkCore.Repositories;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
/// <typeparam name="TEntity"></typeparam>
public abstract class Repository<TDbContext, TEntity> : BasicRepository<TEntity>, IRepository<TEntity>, IAsyncEnumerable<TEntity>
    where TDbContext : DbContext
    where TEntity : class, IEntity
{
    private readonly IClock _clock;

    private readonly IEventBus _eventBus;

    private readonly IDbContextProvider<TDbContext> _dbContextProvider;

    /// <inheritdoc/>
    protected IEntityChangeEventHelper EntityChangeEventHelper { get; set; }

    /// <inheritdoc/>
    protected DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

    /// <inheritdoc/>
    protected DbContext DbContext => _dbContextProvider.GetDbContext();

    /// <inheritdoc/>
    public Repository(IClock clock, IDbContextProvider<TDbContext> dbContextProvider)
    {
        _clock = clock;
        _eventBus = NullEventBus.Instance;
        _dbContextProvider = dbContextProvider;
        EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
    }

    /// <inheritdoc/>
    protected virtual async Task ApplySkywalkerConceptsForAddedEntityAsync(TEntity entity)
    {
        await TriggerEntityCreateEvents(entity);
        await TriggerDomainEventsAsync(entity);
    }

    /// <inheritdoc/>
    protected virtual async Task ApplySkywalkerConceptsForDeletedEntityAsync(TEntity entity)
    {
        await TriggerEntityDeleteEventsAsync(entity);
        await TriggerDomainEventsAsync(entity);
    }

    /// <inheritdoc/>
    protected virtual async Task ApplySkywalkerConceptsForUpdatedEntityAsync(TEntity entity)
    {
        if (entity is IDeleteable deleteable && deleteable.IsDeleted)
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

    /// <inheritdoc/>
    private async Task TriggerEntityCreateEvents(TEntity entity)
    {
        await EntityChangeEventHelper.TriggerEntityCreatedEventOnUowCompletedAsync(entity);
        await EntityChangeEventHelper.TriggerEntityCreatingEventAsync(entity);
    }

    /// <inheritdoc/>
    protected virtual async Task TriggerEntityUpdateEventsAsync(TEntity entity)
    {
        await EntityChangeEventHelper.TriggerEntityUpdatedEventOnUowCompletedAsync(entity);
        await EntityChangeEventHelper.TriggerEntityUpdatingEventAsync(entity);
    }

    /// <inheritdoc/>
    protected virtual async Task TriggerEntityDeleteEventsAsync(TEntity entity)
    {
        await EntityChangeEventHelper.TriggerEntityDeletedEventOnUowCompletedAsync(entity);
        await EntityChangeEventHelper.TriggerEntityDeletingEventAsync(entity);
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    protected override IQueryable<TEntity> GetQueryable()
    {
        return DbSet.AsQueryable();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    protected virtual void SetIdentifier(TEntity entity) { }

    /// <inheritdoc/>
    public override async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).SingleOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await GetQueryable().Where(predicate).ToListAsync(GetCancellationToken(cancellationToken));

        foreach (var entity in entities)
        {
            DbSet.Remove(entity);
        }
    }

    /// <inheritdoc/>
    public override async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is IHasCreationTime objectWithCreationTime)
        {
            objectWithCreationTime.CreationTime = _clock.Now;
        }

        SetIdentifier(entity);

        SetConcurrencyStampIfNull(entity);

        await ApplySkywalkerConceptsForAddedEntityAsync(entity);

        var savedEntity = DbSet.Add(entity).Entity;

        return savedEntity;
    }

    /// <inheritdoc/>
    public override async Task<int> InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            if (entity is IHasCreationTime objectWithCreationTime)
            {
                objectWithCreationTime.CreationTime = _clock.Now;
            }

            SetIdentifier(entity);

            SetConcurrencyStampIfNull(entity);

            await ApplySkywalkerConceptsForAddedEntityAsync(entity);
        }
        await DbSet.AddRangeAsync(entities, GetCancellationToken(cancellationToken));

        return entities.Count();
    }

    /// <inheritdoc/>
    public override async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await ApplySkywalkerConceptsForUpdatedEntityAsync(entity);

        var upgraded = DbContext.Update(entity).Entity;

        return upgraded;
    }

    /// <inheritdoc/>
    public override async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await ApplySkywalkerConceptsForDeletedEntityAsync(entity);
        DbSet.Remove(entity);
    }

    /// <inheritdoc/>
    public override Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.CountAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<int> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return DbSet.Where(filter).CountAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<long> LongCountAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<long> LongCountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return DbSet.Where(filter).LongCountAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, string sorting, CancellationToken cancellationToken = default)
    {
        return DbSet.OrderBy(sorting).ToPagedListAsync(skip, limit, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override async Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> filter, int skip, int limit, string sorting, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(filter)
                          .OrderBy(sorting)
                          .ToPagedListAsync(skip, limit, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, CancellationToken cancellationToken = default)
    {
        return DbSet.OrderBy(ordering => ((IHasCreationTime)ordering).CreationTime).ToPagedListAsync(skip, limit, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> expression, int skip, int limit, CancellationToken cancellationToken = default)
    {
        return DbSet.Where(expression)
                    .OrderBy(ordering => ((IHasCreationTime)ordering).CreationTime)
                    .ToPagedListAsync(skip, limit, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return DbSet.AsAsyncEnumerable().GetAsyncEnumerator(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task EnsureCollectionLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken) where TProperty : class
    {
        await DbContext.Entry(entity)
                       .Collection(propertyExpression)
                       .LoadAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public async Task EnsurePropertyLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty?>> propertyExpression, CancellationToken cancellationToken) where TProperty : class
    {
        await DbContext.Entry(entity)
                       .Reference(propertyExpression)
                       .LoadAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(filter, GetCancellationToken(cancellationToken));
    }
}

/// <inheritdoc/>
public abstract class Repository<TDbContext, TEntity, TKey> : BasicRepository<TEntity, TKey>, IRepository<TEntity, TKey>, ISupportsExplicitLoading<TEntity, TKey>
    where TDbContext : DbContext
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{

    private readonly IClock _clock;

    private readonly IEventBus _eventBus = NullEventBus.Instance;

    private readonly IIdentifier _identifier;

    private readonly IDbContextProvider<TDbContext> _dbContextProvider;

    /// <inheritdoc/>
    public Repository(IClock clock, IIdentifier identifier, IDbContextProvider<TDbContext> dbContextProvider)
    {
        _clock = clock;
        _identifier = identifier;
        _dbContextProvider = dbContextProvider;
        EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    protected void SetIdentifier(TEntity entity)
    {
        if (entity == null)
        {
            return;
        }
        var identifier = typeof(TEntity).GetProperty(nameof(entity.Id));

        //Check for DatabaseGeneratedAttribute
        var dbGeneratedAttr = ReflectionHelper.GetSingleAttributeOrDefault<DatabaseGeneratedAttribute>(identifier!);

        if (dbGeneratedAttr != null && dbGeneratedAttr.DatabaseGeneratedOption != DatabaseGeneratedOption.None)
        {
            return;
        }

        EntityHelper.TrySetEntityId(entity, () => _identifier.GetIdentifier<TKey>(), true);
    }

    /// <inheritdoc/>
    protected IEntityChangeEventHelper EntityChangeEventHelper { get; set; }

    /// <inheritdoc/>
    protected DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

    /// <inheritdoc/>
    protected DbContext DbContext => _dbContextProvider.GetDbContext();

    /// <inheritdoc/>
    protected virtual async Task ApplySkywalkerConceptsForAddedEntityAsync(TEntity entity)
    {
        await TriggerEntityCreateEvents(entity);
        await TriggerDomainEventsAsync(entity);
    }

    /// <inheritdoc/>
    protected virtual async Task ApplySkywalkerConceptsForDeletedEntityAsync(TEntity entity)
    {
        await TriggerEntityDeleteEventsAsync(entity);
        await TriggerDomainEventsAsync(entity);
    }

    /// <inheritdoc/>
    protected virtual async Task ApplySkywalkerConceptsForUpdatedEntityAsync(TEntity entity)
    {
        if (entity is IDeleteable deleteable && deleteable.IsDeleted)
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

    /// <inheritdoc/>
    private async Task TriggerEntityCreateEvents(TEntity entity)
    {
        await EntityChangeEventHelper.TriggerEntityCreatedEventOnUowCompletedAsync(entity);
        await EntityChangeEventHelper.TriggerEntityCreatingEventAsync(entity);
    }

    /// <inheritdoc/>
    protected virtual async Task TriggerEntityUpdateEventsAsync(TEntity entity)
    {
        await EntityChangeEventHelper.TriggerEntityUpdatedEventOnUowCompletedAsync(entity);
        await EntityChangeEventHelper.TriggerEntityUpdatingEventAsync(entity);
    }

    /// <inheritdoc/>
    protected virtual async Task TriggerEntityDeleteEventsAsync(TEntity entity)
    {
        await EntityChangeEventHelper.TriggerEntityDeletedEventOnUowCompletedAsync(entity);
        await EntityChangeEventHelper.TriggerEntityDeletingEventAsync(entity);
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    protected override IQueryable<TEntity> GetQueryable()
    {
        return DbSet.AsQueryable();
    }

    /// <inheritdoc/>
    public override async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).SingleOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await GetQueryable().Where(predicate).ToListAsync(GetCancellationToken(cancellationToken));

        foreach (var entity in entities)
        {
            DbSet.Remove(entity);
        }
    }

    /// <inheritdoc/>
    public override async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is IHasCreationTime objectWithCreationTime)
        {
            objectWithCreationTime.CreationTime = _clock.Now;
        }

        SetIdentifier(entity);

        SetConcurrencyStampIfNull(entity);

        await ApplySkywalkerConceptsForAddedEntityAsync(entity);

        var savedEntity = DbSet.Add(entity).Entity;

        return savedEntity;
    }

    /// <inheritdoc/>
    public override async Task<int> InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            if (entity is IHasCreationTime objectWithCreationTime)
            {
                objectWithCreationTime.CreationTime = _clock.Now;
            }

            SetIdentifier(entity);

            SetConcurrencyStampIfNull(entity);

            await ApplySkywalkerConceptsForAddedEntityAsync(entity);
        }
        await DbSet.AddRangeAsync(entities, GetCancellationToken(cancellationToken));
        return entities.Count();
    }

    /// <inheritdoc/>
    public override async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await ApplySkywalkerConceptsForUpdatedEntityAsync(entity);

        var upgraded = DbContext.Update(entity).Entity;
        return upgraded;
    }

    /// <inheritdoc/>
    public override async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await ApplySkywalkerConceptsForDeletedEntityAsync(entity);
        DbSet.Remove(entity);
    }

    /// <inheritdoc/>
    public override Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.CountAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<int> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return DbSet.Where(filter).CountAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<long> LongCountAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<long> LongCountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return DbSet.Where(filter).LongCountAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, string sorting, CancellationToken cancellationToken = default)
    {
        return DbSet.OrderBy(sorting).ToPagedListAsync(skip, limit, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override async Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> filter, int skip, int limit, string sorting, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(filter)
                          .OrderBy(sorting)
                          .ToPagedListAsync(skip, limit, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, CancellationToken cancellationToken = default)
    {
        return DbSet.OrderBy(ordering => ((IHasCreationTime)ordering).CreationTime).ToPagedListAsync(skip, limit, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> expression, int skip, int limit, CancellationToken cancellationToken = default)
    {
        return DbSet.Where(expression)
                    .OrderBy(ordering => ((IHasCreationTime)ordering).CreationTime)
                    .ToPagedListAsync(skip, limit, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return DbSet.AsAsyncEnumerable().GetAsyncEnumerator(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task EnsureCollectionLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken) where TProperty : class
    {
        await DbContext.Entry(entity)
                       .Collection(propertyExpression)
                       .LoadAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public async Task EnsurePropertyLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty?>> propertyExpression, CancellationToken cancellationToken) where TProperty : class
    {
        await DbContext.Entry(entity)
                       .Reference(propertyExpression)
                       .LoadAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(filter, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, GetCancellationToken(cancellationToken));
        if (entity == null)
        {
            return;
        }
        await ApplySkywalkerConceptsForDeletedEntityAsync(entity);
        await DeleteAsync(entity, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override async Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object[] { id }, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, cancellationToken);
        if (entity == null)
        {
            throw new EntityNotFoundException();
        }
        return entity;
    }

    /// <inheritdoc/>
    public Task<bool> AnyAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return AnyAsync(filter => filter.Id.Equals(id), GetCancellationToken(cancellationToken));
    }
}
