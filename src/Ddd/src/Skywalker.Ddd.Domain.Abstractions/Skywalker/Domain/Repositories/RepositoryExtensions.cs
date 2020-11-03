using Skywalker.Data;
using Skywalker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Domain.Repositories
{
    public static class RepositoryExtensions
    {
        public static async Task EnsureCollectionLoadedAsync<TEntity, TKey, TProperty>(
            this IBasicRepository<TEntity, TKey> repository,
            TEntity entity,
            Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression,
            CancellationToken cancellationToken = default
        )
            where TEntity : class, IEntity<TKey>
            where TProperty : class
        {
            if (repository is ISupportsExplicitLoading<TEntity, TKey> repo)
            {
                await repo.EnsureCollectionLoadedAsync(entity, propertyExpression, cancellationToken);
            }
        }

        public static async Task EnsurePropertyLoadedAsync<TEntity, TKey, TProperty>(
            this IBasicRepository<TEntity, TKey> repository,
            TEntity entity,
            Expression<Func<TEntity, TProperty>> propertyExpression,
            CancellationToken cancellationToken = default
        )
            where TEntity : class, IEntity<TKey>
            where TProperty : class
        {
            if (repository is ISupportsExplicitLoading<TEntity, TKey> repo)
            {
                await repo.EnsurePropertyLoadedAsync(entity, propertyExpression, cancellationToken);
            }
        }

        public static async Task HardDeleteAsync<TEntity>(
            this IBasicRepository<TEntity> repository,
            TEntity entity,
            bool autoSave = false,
            CancellationToken cancellationToken = default
        )
            where TEntity : class, IEntity, IDeleteable
        {

                await HardDeleteWithUnitOfWorkAsync(repository, entity, autoSave, cancellationToken);
        }

        private static async Task HardDeleteWithUnitOfWorkAsync<TEntity>(
            IBasicRepository<TEntity> repository, 
            TEntity entity, 
            bool autoSave,
            CancellationToken cancellationToken
        )
            where TEntity : class, IEntity, IDeleteable
        {
            //var hardDeleteEntities = (HashSet<IEntity>) currentUow.Items.GetOrAdd(
            //    UnitOfWorkItemNames.HardDeletedEntities,
            //    () => new HashSet<IEntity>()
            //);
            var hardDeleteEntities = new HashSet<IEntity>
            {
                entity
            };

            await repository.DeleteAsync(entity, autoSave, cancellationToken);
        }
    }
}
