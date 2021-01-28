using Microsoft.Extensions.DependencyInjection;
using Skywalker.Domain.Entities;
using System;

namespace Skywalker.Domain.Services
{
    /// <summary>
    /// This interface can be implemented by all domain services to identify them by convention.
    /// </summary>
    public interface IDomainService : ITransientDependency
    {

    }

    /// <summary>
    /// This interface can be implemented by all domain services to identify them by convention.
    /// </summary>
    public interface IDomainService<TEntity> : IDomainService where TEntity : class, IEntity
    {

    }

    /// <summary>
    /// This interface can be implemented by all domain services to identify them by convention.
    /// </summary>
    public interface IDomainService<TEntity, TKey> : IDomainService<TEntity> where TEntity : class, IEntity<TKey>
    {

    }
}