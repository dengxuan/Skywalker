using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skywalker.Data;
using Skywalker.Data.Filtering;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Domain.Entities;
using Skywalker.Domain.Entities.Events;
using Skywalker.Extensions.Timing;
using Skywalker.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Infrastructure
{
    public class SkywalkerDbContext<TDbContext> where TDbContext : IDbContext
    {
        protected virtual bool IsSoftDeleteFilterEnabled => DataFilter?.IsEnabled<IDeleteable>() ?? false;
        protected ILazyLoader LazyLoader { get; }

        protected IGuidGenerator GuidGenerator => LazyLoader.GetRequiredService<IGuidGenerator>();

        protected IDataFilter DataFilter => LazyLoader.GetRequiredService<IDataFilter>();

        protected IEntityChangeEventHelper EntityChangeEventHelper => LazyLoader.GetRequiredService<IEntityChangeEventHelper>();

        protected IClock Clock => LazyLoader.GetRequiredService<IClock>();

        protected ILogger<SkywalkerDbContext<TDbContext>> Logger => LazyLoader.GetRequiredService<ILogger<SkywalkerDbContext<TDbContext>>>();

        protected TDbContext DbContext => LazyLoader.GetRequiredService<TDbContext>();

        protected SkywalkerDbContext(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }

        public IDataCollection<T> DataCollection<T>() where T : class
        {
            return DbContext.DataCollection<T>();
        }

        public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return DbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected virtual bool IsHardDeleted(IEntity entry)
        {
            return !(entry is IDeleteable);
        }

        protected virtual void CheckAndSetId(IEntity entry)
        {
            if (entry is IEntity<Guid> entityWithGuidId)
            {
                TrySetGuidId(entry, entityWithGuidId);
            }
        }

        protected virtual void TrySetGuidId(IEntity entry, IEntity<Guid> entity)
        {
            if (entity.Id != default)
            {
                return;
            }

            var idProperty = entry.GetType().GetProperty("Id");

            //Check for DatabaseGeneratedAttribute
            var dbGeneratedAttr = ReflectionHelper.GetSingleAttributeOrDefault<DatabaseGeneratedAttribute>(idProperty!);

            if (dbGeneratedAttr != null && dbGeneratedAttr.DatabaseGeneratedOption != DatabaseGeneratedOption.None)
            {
                return;
            }
            EntityHelper.TrySetId(entity, () => GuidGenerator.Create(), true);
        }

        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }

        public int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return DbContext.SaveChanges(acceptAllChangesOnSuccess);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return DbContext.SaveChangesAsync(cancellationToken);
        }

        public void AddRange([NotNull] IEnumerable<object> entities)
        {
            DbContext.AddRange(entities);
        }

        public void AddRange([NotNull] params object[] entities)
        {
            DbContext.AddRange(entities);
        }

        public Task AddRangeAsync([NotNull] params object[] entities)
        {
            return DbContext.AddRangeAsync(entities);
        }

        public Task AddRangeAsync([NotNull] IEnumerable<object> entities, CancellationToken cancellationToken = default)
        {
            return DbContext.AddRangeAsync(entities, cancellationToken);
        }

        public void AttachRange([NotNull] IEnumerable<object> entities)
        {
            DbContext.AttachRange(entities);
        }

        public void AttachRange([NotNull] params object[] entities)
        {
            DbContext.AttachRange(entities);
        }

        public object Find([NotNull] Type entityType, [NotNull] params object[] keyValues)
        {
            return DbContext.Find(entityType, keyValues);
        }

        public TEntity Find<TEntity>([NotNull] params object[] keyValues) where TEntity : class
        {
            return DbContext.Find<TEntity>(keyValues);
        }

        public ValueTask<object> FindAsync([NotNull] Type entityType, [NotNull] object[] keyValues, CancellationToken cancellationToken)
        {
            return DbContext.FindAsync(entityType, keyValues, cancellationToken);
        }

        public ValueTask<TEntity> FindAsync<TEntity>([NotNull] object[] keyValues, CancellationToken cancellationToken) where TEntity : class
        {
            return DbContext.FindAsync<TEntity>(keyValues, cancellationToken);
        }

        public ValueTask<TEntity> FindAsync<TEntity>([NotNull] params object[] keyValues) where TEntity : class
        {
            return DbContext.FindAsync<TEntity>(keyValues);
        }

        public ValueTask<object> FindAsync([NotNull] Type entityType, [NotNull] params object[] keyValues)
        {
            return DbContext.FindAsync(entityType, keyValues);
        }

        public void RemoveRange([NotNull] IEnumerable<object> entities)
        {
            DbContext.RemoveRange(entities);
        }

        public void RemoveRange([NotNull] params object[] entities)
        {
            DbContext.RemoveRange(entities);
        }

        public void UpdateRange([NotNull] params object[] entities)
        {
            DbContext.UpdateRange(entities);
        }

        public void UpdateRange([NotNull] IEnumerable<object> entities)
        {
            DbContext.UpdateRange(entities);
        }
    }
}
