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
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Infrastructure
{
    public abstract class SkywalkerDbContext<TDbContext> : IDbContext where TDbContext : IDbContext
    {
        protected virtual bool IsSoftDeleteFilterEnabled => DataFilter?.IsEnabled<IDeleteable>() ?? false;
        protected ILazyLoader LazyLoader { get; }

        protected IGuidGenerator GuidGenerator => LazyLoader.GetRequiredService<IGuidGenerator>();

        protected IDataFilter DataFilter => LazyLoader.GetRequiredService<IDataFilter>();

        protected IEntityChangeEventHelper EntityChangeEventHelper => LazyLoader.GetRequiredService<IEntityChangeEventHelper>();

        protected IClock Clock => LazyLoader.GetRequiredService<IClock>();

        protected ILogger<SkywalkerDbContext<TDbContext>> Logger => LazyLoader.GetRequiredService<ILogger<SkywalkerDbContext<TDbContext>>>();

        protected SkywalkerDbContext(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }

        public abstract IDataCollection<T> DataCollection<T>() where T : class;

        public abstract Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

        public abstract void Initialize();

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

        protected virtual Expression<Func<TEntity, bool>>? CreateFilterExpression<TEntity>()
            where TEntity : class
        {
            Expression<Func<TEntity, bool>>? expression = null;

            if (typeof(IDeleteable).IsAssignableFrom(typeof(TEntity)))
            {
                expression = e => !IsSoftDeleteFilterEnabled;
            }

            return expression;
        }

        protected virtual Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expression1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expression1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expression2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expression2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left!, right!), parameter);
        }

        public abstract int SaveChanges();

        public abstract int SaveChanges(bool acceptAllChangesOnSuccess);

        public abstract Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        public abstract void AddRange([NotNull] IEnumerable<object> entities);

        public abstract void AddRange([NotNull] params object[] entities);

        public abstract Task AddRangeAsync([NotNull] params object[] entities);

        public abstract Task AddRangeAsync([NotNull] IEnumerable<object> entities, CancellationToken cancellationToken = default);

        public abstract void AttachRange([NotNull] IEnumerable<object> entities);

        public abstract void AttachRange([NotNull] params object[] entities);

        public abstract object Find([NotNull] Type entityType, [NotNull] params object[] keyValues);

        public abstract TEntity Find<TEntity>([NotNull] params object[] keyValues) where TEntity : class;

        public abstract ValueTask<object> FindAsync([NotNull] Type entityType, [NotNull] object[] keyValues, CancellationToken cancellationToken);

        public abstract ValueTask<TEntity> FindAsync<TEntity>([NotNull] object[] keyValues, CancellationToken cancellationToken) where TEntity : class;

        public abstract ValueTask<TEntity> FindAsync<TEntity>([NotNull] params object[] keyValues) where TEntity : class;

        public abstract ValueTask<object> FindAsync([NotNull] Type entityType, [NotNull] params object[] keyValues);

        public abstract void RemoveRange([NotNull] IEnumerable<object> entities);

        public abstract void RemoveRange([NotNull] params object[] entities);

        public abstract void UpdateRange([NotNull] params object[] entities);

        public abstract void UpdateRange([NotNull] IEnumerable<object> entities);

        class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression? Visit(Expression? node)
            {
                if (node == _oldValue)
                {
                    return _newValue;
                }

                return base.Visit(node);
            }
        }
    }
}
