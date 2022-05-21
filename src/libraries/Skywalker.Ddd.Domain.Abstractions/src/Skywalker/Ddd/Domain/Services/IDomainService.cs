using System.Linq.Expressions;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Extensions.Collections.Generic;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Domain.Services;

/// <summary>
/// 所有域服务都可以实现此接口，以按约定识别它们.
/// 仅标记一个类是一个领域服务
/// </summary>
public interface IDomainService : ITransientDependency { }

/// <summary>
/// 所有域服务都可以实现此接口，以按约定识别它们.
/// 提供基本的数据操作接口
/// </summary>
public interface IDomainService<TEntity> : IDomainService where TEntity : class, IEntity
{
    /// <summary>
    /// 添加一个实体
    /// </summary>
    /// <param name="entity">待添加的实体</param>
    /// <param name="cancellationToken">可取消操作的令牌</param>
    /// <returns>添加后的实体</returns>
    Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新一个实体
    /// </summary>
    /// <param name="entity">要更新的实体</param>
    /// <param name="cancellationToken">可取消操作的令牌</param>
    /// <returns>更新后的实体</returns>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除一个实体
    /// </summary>
    /// <param name="entity">要删除的实体</param>
    /// <param name="cancellationToken">可取消操作的令牌</param>
    /// <returns></returns>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取实体数量
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取实体数量
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> LongCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取符合条件<paramref name="filter"/>的实体数量
    /// </summary>
    /// <param name="filter">数据筛选条件</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取符合条件<paramref name="filter"/>的实体是否存在
    /// </summary>
    /// <param name="filter">筛选条件</param>
    /// <param name="cancellationToken"></param>
    /// <returns>存在返回true，否则返回false</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取符合条件<paramref name="expression"/>的实体数量
    /// </summary>
    /// <param name="expression">数据筛选条件</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> LongCountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询所有实体，慎重使用。
    /// 大表严禁使用此方法，推荐使用<see cref="IDomainService{TEntity, TKey}.GetPagedListAsync(int, int, string, CancellationToken)"/>进行分页读取
    /// </summary>
    /// <param name="cancellationToken">可取消操作的令牌</param>
    /// <returns>当前实体<see cref="TEntity"/>的所有数据集，或者空集合</returns>
    Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询符合条件的所有实体集，慎重使用。
    /// 筛选结果较多时，推荐使用<see cref="IDomainService{TEntity, TKey}.GetPagedListAsync(int, int, string, CancellationToken)"/>进行分页读取
    /// </summary>
    /// <param name="expression">查询条件</param>
    /// <param name="cancellationToken">可取消操作的令牌</param>
    /// <returns>当前实体<see cref="TEntity"/>的所有数据集，或者空集合</returns>
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// 分页查询当前实体<see cref="TEntity"/>的数据集.
    /// 默认按创建时间排序,如果实体未实现<see cref="IHasCreationTime"/>接口,则按照Id排序
    /// </summary>
    /// <param name="skip">排序后跳过的数据行数</param>
    /// <param name="limit">最大获取数据行数</param>
    /// <param name="sorting">需要包含排序字段和可选的排序指令(ASC 或 DESC)，可包含多个字段，使用逗号(,)分割。 例如： "Name ASC, Age DESC"</param>
    /// <param name="cancellationToken">可取消操作的令牌</param>
    /// <returns>符合条件的数据集</returns>
    Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// 分页查询当前实体<see cref="TEntity"/>的数据集
    /// </summary>
    /// <param name="skip">排序后跳过的数据行数</param>
    /// <param name="limit">最大获取数据行数</param>
    /// <param name="sorting">需要包含排序字段和可选的排序指令(ASC 或 DESC)，可包含多个字段，使用逗号(,)分割。 例如： "Name ASC, Age DESC"</param>
    /// <param name="cancellationToken">可取消操作的令牌</param>
    /// <returns>符合条件的数据集</returns>
    Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, string sorting, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件分页查询实体<see cref="TEntity"/>数据集
    /// 默认按创建时间排序,如果实体未实现<see cref="IHasCreationTime"/>接口,则按照Id排序
    /// </summary>
    /// <param name="expression">筛选条件</param>
    /// <param name="skip">排序后跳过的数据行数</param>
    /// <param name="limit">最大获取数据行数</param>
    /// <param name="cancellationToken">可取消操作的令牌</param>
    /// <returns></returns>
    Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> filter, int skip, int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件分页查询实体<see cref="TEntity"/>数据集
    /// </summary>
    /// <param name="expression">筛选条件</param>
    /// <param name="skip">排序后跳过的数据行数</param>
    /// <param name="limit">最大获取数据行数</param>
    /// <param name="sorting">需要包含排序字段和可选的排序指令(ASC 或 DESC)，可包含多个字段，使用逗号(,)分割。 例如： "Name ASC, Age DESC"</param>
    /// <param name="cancellationToken">可取消操作的令牌</param>
    /// <returns></returns>
    Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> filter, int skip, int limit, string sorting, CancellationToken cancellationToken = default);
}

/// <summary>
/// 所有域服务都可以实现此接口，以按约定识别它们.
/// 提供基本的数据操作接口
/// </summary>
public interface IDomainService<TEntity, TKey> : IDomainService<TEntity> where TEntity : class, IEntity<TKey> where TKey : notnull
{

    /// <summary>
    /// 获取给定主键<paramref name="id"/>的实体是否存在
    /// </summary>
    /// <param name="id">实体编号</param>
    /// <param name="cancellationToken"></param>
    /// <returns>存在返回true，否则返回false</returns>
    Task<bool> AnyAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取给定主键的实体。
    /// 如果给定主键的实体不存在，则抛出 <see cref="EntityNotFoundException"/> 异常
    /// </summary>
    /// <param name="id">要获取实体的主键</param>
    /// <param name="cancellationToken">可取消操作的令牌</param>
    /// <exception cref="EntityNotFoundException">给定主键的实体不存在时抛出</exception>
    /// <returns>符合条件的实体<see cref="TEntity"/></returns>
    Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取给定主键的实体，如果没有找到返回null
    /// </summary>
    /// <param name="id">要获取实体的主键</param>
    /// <param name="cancellationToken">可取消操作的令牌</param>
    /// <returns>符合条件的实体<see cref="TEntity"/>或者null</returns>
    Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);

}
