using Microsoft.EntityFrameworkCore;
using Skywalker.Domain.Entities;
using Skywalker.Domain.Repositories;
using System;

namespace Skywalker.Ddd.Domain.Repositories
{
    public static class EntityFrameworkCoreIRepositoryExtensions
    {
        public static DbContext GetDbContext<TEntity>(this IReadOnlyRepository<TEntity> repository)
            where TEntity : class, IEntity
        {
            return repository.ToEfCoreRepository().DbContext;
        }

        public static DbSet<TEntity> GetDbSet<TEntity>(this IReadOnlyRepository<TEntity> repository)
            where TEntity : class, IEntity
        {
            return repository.ToEfCoreRepository().DbSet;
        }

        public static ISkywalkerRepository<TEntity> ToEfCoreRepository<TEntity>(this IReadOnlyRepository<TEntity> repository)
            where TEntity : class, IEntity
        {
            if (repository is ISkywalkerRepository<TEntity> efCoreRepository)
            {
                return efCoreRepository;
            }

            throw new ArgumentException("Given repository does not implement " + typeof(ISkywalkerRepository<TEntity>).AssemblyQualifiedName, nameof(repository));
        }
    }
}
