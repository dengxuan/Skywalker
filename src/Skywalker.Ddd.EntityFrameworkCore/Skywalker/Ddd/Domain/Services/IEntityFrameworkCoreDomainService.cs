// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Domain.Entities;

namespace Skywalker.Ddd.Domain.Services;

public interface IEntityFrameworkCoreDomainService<TEntity> : IDomainService<TEntity> where TEntity : class, IEntity
{
    public Task<IQueryable<TEntity>> GetQueryableAsync();
}

public interface IEntityFrameworkCoreDomainService<TEntity, TKey> : IEntityFrameworkCoreDomainService<TEntity>, IDomainService<TEntity, TKey>
    where TEntity : class, IEntity<TKey> where TKey : notnull
{

}
