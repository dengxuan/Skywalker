// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.EntityFrameworkCore;

namespace Skywalker.Ddd.Domain.Repositories;

public static class EntityFrameworkCoreRepositoryExtensions
{

    public static Task<DbContext> GetDbContextAsync<TEntity>(this IReadOnlyRepository<TEntity> repository)
        where TEntity : class, IEntity
    {
        return repository.ToEntityFrameworkCoreRepository().GetDbContextAsync();
    }

    public static Task<DbSet<TEntity>> GetDbSetAsync<TEntity>(this IReadOnlyRepository<TEntity> repository)
        where TEntity : class, IEntity
    {
        return repository.ToEntityFrameworkCoreRepository().GetDbSetAsync();
    }

    public static IEntityFrameworkCoreRepository<TEntity> ToEntityFrameworkCoreRepository<TEntity>(this IReadOnlyRepository<TEntity> repository)
        where TEntity : class, IEntity
    {
        if (repository is IEntityFrameworkCoreRepository<TEntity> efCoreRepository)
        {
            return efCoreRepository;
        }

        throw new ArgumentException("Given repository does not implement " + typeof(IEntityFrameworkCoreRepository<TEntity>).AssemblyQualifiedName, nameof(repository));
    }
}
