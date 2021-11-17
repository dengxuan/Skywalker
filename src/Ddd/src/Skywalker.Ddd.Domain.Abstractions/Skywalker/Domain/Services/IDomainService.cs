using Microsoft.Extensions.DependencyInjection;
using Skywalker.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Skywalker.Domain.Services
{
    /// <summary>
    /// This interface can be implemented by all domain services to identify them by convention.
    /// </summary>
    public interface IDomainService/* : ITransientDependency*/
    {
    }

    /// <summary>
    /// This interface can be implemented by all domain services to identify them by convention.
    /// </summary>
    public interface IDomainService<TEntity> : IDomainService<TEntity,Guid> where TEntity : class, IEntity<Guid>
    {
    }

    /// <summary>
    /// This interface can be implemented by all domain services to identify them by convention.
    /// </summary>
    public interface IDomainService<TEntity, TKey> : IDomainService where TEntity : class, IEntity<TKey>
    {
        /// <summary>
        /// 根据Id查询商户,若不存在，则抛出异常<see cref="EntityNotFoundException"/>
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>商户<see cref="TEntity"/></returns>
        Task<TEntity> GetAsync(TKey id);

        /// <summary>
        /// 根据Id查询实体，不存在时返回null
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>实体<see cref="TEntity"/></returns>
        Task<TEntity?> FindAsync(TKey id);
    }
}