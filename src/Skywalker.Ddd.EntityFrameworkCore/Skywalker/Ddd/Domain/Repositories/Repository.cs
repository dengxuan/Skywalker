// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Events;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.EventBus.Abstractions;
using Skywalker.Extensions.Collections.Generic;
using Skywalker.Extensions.Specifications;
using Skywalker.Extensions.Timezone;

namespace Skywalker.Ddd.Domain.Repositories;

/// <summary>
///
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
/// <typeparam name="TEntity"></typeparam>
/// <inheritdoc/>
public class Repository<TDbContext, TEntity>(IClock clock, IEventBus eventBus, IDbContextProvider<TDbContext> dbContextProvider, IUnitOfWorkAccessor unitOfWorkAccessor) : BasicRepository<TEntity>, IRepository<TEntity>, IAsyncEnumerable<TEntity>, IEntityFrameworkCoreRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : class, IEntity
{
    private readonly IClock _clock = clock;

    private readonly IEventBus _eventBus = eventBus;

    private readonly IDbContextProvider<TDbContext> _dbContextProvider = dbContextProvider;

    private readonly IUnitOfWorkAccessor _unitOfWorkAccessor = unitOfWorkAccessor;

    /// <inheritdoc/>
    protected IEntityChangeEventHelper EntityChangeEventHelper { get; set; } = NullEntityChangeEventHelper.Instance;

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
        if (entity is IDeletable deleteable && deleteable.IsDeleted)
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
    protected virtual Task TriggerDomainEventsAsync(object entity)
    {
        if (entity is not IGeneratesDomainEvents generatesDomainEventsEntity)
        {
            return Task.CompletedTask;
        }

        var distributedEvents = generatesDomainEventsEntity.GetDistributedEvents()?.ToArray();
        if (distributedEvents == null || distributedEvents.Length == 0)
        {
            return Task.CompletedTask;
        }

        // 注册到 UnitOfWork 完成后执行，确保事务提交成功后才发布事件
        var unitOfWork = _unitOfWorkAccessor.UnitOfWork;
        if (unitOfWork != null)
        {
            unitOfWork.OnCompleted(async () =>
            {
                foreach (var distributedEvent in distributedEvents)
                {
                    await _eventBus.PublishAsync(distributedEvent.GetType(), distributedEvent);
                }
            });
        }

        generatesDomainEventsEntity.ClearDistributedEvents();
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override IQueryable<TEntity> GetQueryable()
    {
        return DbSet.AsQueryable();
    }

    Task<DbContext> IEntityFrameworkCoreRepository<TEntity>.GetDbContextAsync() => Task.FromResult(DbContext);

    Task<DbSet<TEntity>> IEntityFrameworkCoreRepository<TEntity>.GetDbSetAsync() => Task.FromResult(DbSet);

    /// <inheritdoc/>
    public override async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).SingleOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var entities = await GetQueryable().Where(predicate).ToListAsync(GetCancellationToken(cancellationToken));
        foreach (var entity in entities)
        {
            DbSet.Remove(entity);
        }
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
        }
    }

    /// <inheritdoc/>
    public override async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        // CreationTime 和 ConcurrencyStamp 统一在 SkywalkerDbContext.ApplySkywalkerConceptsForAddedEntity 中设置
        await ApplySkywalkerConceptsForAddedEntityAsync(entity);
        var savedEntity = DbSet.Add(entity).Entity;
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
        }
        return savedEntity;
    }

    /// <inheritdoc/>
    public override async Task<int> InsertAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            // CreationTime 和 ConcurrencyStamp 统一在 SkywalkerDbContext.ApplySkywalkerConceptsForAddedEntity 中设置
            await ApplySkywalkerConceptsForAddedEntityAsync(entity);
        }
        await DbSet.AddRangeAsync(entities, GetCancellationToken(cancellationToken));
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
        }
        return entities.Count();
    }

    /// <inheritdoc/>
    public override async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        await ApplySkywalkerConceptsForUpdatedEntityAsync(entity);
        var upgraded = DbContext.Update(entity).Entity;
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
        }
        return upgraded;
    }

    /// <inheritdoc/>
    public override async Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        await ApplySkywalkerConceptsForDeletedEntityAsync(entity);
        DbSet.Remove(entity);
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
        }
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
    public override Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, string? sorting, CancellationToken cancellationToken = default)
    {
        return DbSet.OrderByIf(!sorting.IsNullOrEmpty(), sorting).ToPagedListAsync(skip, limit, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override async Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> filter, int skip, int limit, string? sorting, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(filter)
                          .OrderByIf(!sorting.IsNullOrEmpty(), sorting)
                          .ToPagedListAsync(skip, limit, GetCancellationToken(cancellationToken));
    }

    public override async Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, string? sorting, string? filter, CancellationToken cancellationToken = default)
    {
        return await DbSet.WhereIf(!filter.IsNullOrEmpty(), filter)
                          .OrderByIf(!sorting.IsNullOrEmpty(), sorting)
                          .ToPagedListAsync(skip, limit, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, CancellationToken cancellationToken = default)
    {
        var query = typeof(IHasCreationTime).IsAssignableFrom(typeof(TEntity))
            ? DbSet.OrderBy(ordering => ((IHasCreationTime)ordering).CreationTime)
            : DbSet.AsQueryable();
        return query.ToPagedListAsync(skip, limit, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> expression, int skip, int limit, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(expression);
        if (typeof(IHasCreationTime).IsAssignableFrom(typeof(TEntity)))
        {
            query = query.OrderBy(ordering => ((IHasCreationTime)ordering).CreationTime);
        }
        return query.ToPagedListAsync(skip, limit, GetCancellationToken(cancellationToken));
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
    public Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(specification.ToExpression(), GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return DbSet.CountAsync(specification.ToExpression(), GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public Task<long> LongCountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return DbSet.LongCountAsync(specification.ToExpression(), GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<List<TEntity>> GetListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return DbSet.Where(specification.ToExpression()).ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<PagedList<TEntity>> GetPagedListAsync(ISpecification<TEntity> specification, int skip, int limit, string? sorting = null, CancellationToken cancellationToken = default)
    {
        return DbSet.Where(specification.ToExpression())
                    .OrderByIf(!sorting.IsNullOrEmpty(), sorting)
                    .ToPagedListAsync(skip, limit, GetCancellationToken(cancellationToken));
    }
}

/// <inheritdoc/>
public class Repository<TDbContext, TEntity, TKey> : Repository<TDbContext, TEntity>, IRepository<TEntity, TKey>, ISupportsExplicitLoading<TEntity, TKey>
    where TDbContext : DbContext
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{

    private readonly IClock _clock;

    /// <inheritdoc/>
    public Repository(IClock clock, IEventBus eventBus, IDbContextProvider<TDbContext> dbContextProvider, IUnitOfWorkAccessor unitOfWorkAccessor) : base(clock, eventBus, dbContextProvider, unitOfWorkAccessor)
    {
        _clock = clock;
        EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
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
    public override async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var entities = await GetQueryable().Where(predicate).ToListAsync(GetCancellationToken(cancellationToken));

        foreach (var entity in entities)
        {
            DbSet.Remove(entity);
        }
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
        }
    }

    /// <inheritdoc/>
    public override async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        // CreationTime 和 ConcurrencyStamp 统一在 SkywalkerDbContext.ApplySkywalkerConceptsForAddedEntity 中设置
        await ApplySkywalkerConceptsForAddedEntityAsync(entity);
        var savedEntity = DbSet.Add(entity).Entity;
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
        }
        return savedEntity;
    }

    /// <inheritdoc/>
    public override async Task<int> InsertAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            // CreationTime 和 ConcurrencyStamp 统一在 SkywalkerDbContext.ApplySkywalkerConceptsForAddedEntity 中设置
            await ApplySkywalkerConceptsForAddedEntityAsync(entity);
        }
        await DbSet.AddRangeAsync(entities, GetCancellationToken(cancellationToken));
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
        }
        return entities.Count();
    }

    /// <inheritdoc/>
    public override async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        await ApplySkywalkerConceptsForUpdatedEntityAsync(entity);
        var upgraded = DbContext.Update(entity).Entity;
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
        }
        return upgraded;
    }

    /// <inheritdoc/>
    public override async Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        await ApplySkywalkerConceptsForDeletedEntityAsync(entity);
        DbSet.Remove(entity);
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
        }
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
    public override Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, string? sorting, CancellationToken cancellationToken = default)
    {
        return DbSet.OrderByIf(!sorting.IsNullOrEmpty(), sorting).ToPagedListAsync(skip, limit, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override async Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> filter, int skip, int limit, string? sorting, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(filter)
                          .OrderByIf(!sorting.IsNullOrEmpty(), sorting)
                          .ToPagedListAsync(skip, limit, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, CancellationToken cancellationToken = default)
    {
        var query = typeof(IHasCreationTime).IsAssignableFrom(typeof(TEntity))
            ? DbSet.OrderBy(ordering => ((IHasCreationTime)ordering).CreationTime)
            : DbSet.OrderBy(ordering => ordering.Id);
        return query.ToPagedListAsync(skip, limit, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public override Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> expression, int skip, int limit, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(expression);
        var orderedQuery = typeof(IHasCreationTime).IsAssignableFrom(typeof(TEntity))
            ? query.OrderBy(ordering => ((IHasCreationTime)ordering).CreationTime)
            : query.OrderBy(ordering => ordering.Id);
        return orderedQuery.ToPagedListAsync(skip, limit, GetCancellationToken(cancellationToken));
    }


    /// <inheritdoc/>
    public async Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, GetCancellationToken(cancellationToken));
        if (entity == null)
        {
            return;
        }
        await ApplySkywalkerConceptsForDeletedEntityAsync(entity);
        await DeleteAsync(entity, autoSave, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public async Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, cancellationToken) ?? throw new EntityNotFoundException();
        return entity;
    }

    /// <inheritdoc/>
    public Task<bool> AnyAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return AnyAsync(filter => filter.Id.Equals(id), GetCancellationToken(cancellationToken));
    }
}
