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
        /// ����Id��ѯ�̻�,�������ڣ����׳��쳣<see cref="EntityNotFoundException"/>
        /// </summary>
        /// <param name="id">����</param>
        /// <returns>�̻�<see cref="TEntity"/></returns>
        Task<TEntity> GetAsync(TKey id);

        /// <summary>
        /// ����Id��ѯʵ�壬������ʱ����null
        /// </summary>
        /// <param name="id">����</param>
        /// <returns>ʵ��<see cref="TEntity"/></returns>
        Task<TEntity?> FindAsync(TKey id);
    }
}