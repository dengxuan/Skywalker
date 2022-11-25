// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;

namespace Skywalker.Ddd.EntityFrameworkCore;

public interface IEntityFrameworkCoreRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
    Task<DbContext> GetDbContextAsync();

    Task<DbSet<TEntity>> GetDbSetAsync();
}

public interface IEntityFrameworkCoreRepository<TEntity, TKey> : IEntityFrameworkCoreRepository<TEntity>, IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey> where TKey : notnull
{

}
