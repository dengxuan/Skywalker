// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.Domain.Services;

namespace Skywalker.Identity.Domain.Repositories;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public abstract class EntityFrameworkCoreDomainService<TEntity> : DomainService<TEntity>, IEntityFrameworkCoreDomainService<TEntity>
    where TEntity : class, IEntity
{
    public EntityFrameworkCoreDomainService(IRepository<TEntity> repository, IAsyncQueryableExecuter asyncExecuter) : base(repository, asyncExecuter)
    {
    }

    public IQueryable<TEntity> Entities => Repository.AsQueryable();
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKey"></typeparam>
public abstract class EntityFrameworkCoreDomainService<TEntity, TKey> : DomainService<TEntity, TKey>, IEntityFrameworkCoreDomainService<TEntity,TKey>
    where TEntity : class, IEntity<TKey> where TKey : notnull
{
    public EntityFrameworkCoreDomainService(IRepository<TEntity, TKey> repository, IAsyncQueryableExecuter asyncExecuter) : base(repository, asyncExecuter)
    {
    }

    public IQueryable<TEntity> Entities => Repository.AsQueryable();
}
