// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Services;

namespace Skywalker.Identity.Domain.Repositories;

public static class EntityFrameworkCoreDomainServiceExtensions
{

    public static IQueryable<TEntity> GetQueryable<TEntity>(this IDomainService<TEntity> repository)
        where TEntity : class, IEntity
    {
        return repository.ToEntityFrameworkCoreDomainService().Entities;
    }

    public static IEntityFrameworkCoreDomainService<TEntity> ToEntityFrameworkCoreDomainService<TEntity>(this IDomainService<TEntity> domainService)
        where TEntity : class, IEntity
    {
        if (domainService is IEntityFrameworkCoreDomainService<TEntity> efCoreDomainService)
        {
            return efCoreDomainService;
        }

        throw new ArgumentException("Given domain service does not implement " + typeof(IEntityFrameworkCoreDomainService<TEntity>).AssemblyQualifiedName, nameof(domainService));
    }
}
