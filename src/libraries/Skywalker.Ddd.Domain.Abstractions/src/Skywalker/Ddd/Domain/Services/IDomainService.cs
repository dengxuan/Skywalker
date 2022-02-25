using System.Linq.Expressions;
using Skywalker.Ddd.Domain.Entities;

namespace Skywalker.Ddd.Domain.Services;

/// <summary>
/// 所有域服务都可以实现此接口，以按约定识别它们.
/// 仅标记一个类是一个领域服务
/// </summary>
public interface IDomainService
{
}

/// <summary>
/// 所有域服务都可以实现此接口，以按约定识别它们.
/// 提供基本的数据操作接口
/// </summary>
public interface IDomainService<TEntity> : IDomainService<TEntity, Guid> where TEntity : class, IEntity<Guid>
{
    /// <summary>
    /// 更新一个实体
    /// </summary>
    /// <param name="entity">要更新的实体</param>
    /// <returns>更新后的实体</returns>
    Task<TEntity> UpdateAsync(TEntity entity);

    /// <summary>
    /// 删除一个实体
    /// </summary>
    /// <param name="entity">要删除的实体</param>
    /// <returns></returns>
    Task DeleteAsync(TEntity entity);

    /// <summary>
    /// 查询所有实体，慎重使用。
    /// 大表严禁使用此方法，推荐使用<see cref="IDomainService{TEntity, TKey}.GetPagedListAsync(int, int, string, CancellationToken)"/>进行分页读取
    /// </summary>
    /// <returns>当前实体<see cref="TEntity"/>的所有数据集，或者空集合</returns>
    Task<List<TEntity>> GetListAsync();

    /// <summary>
    /// 查询符合条件的所有实体集，慎重使用。
    /// 筛选结果较多时，推荐使用<see cref="IDomainService{TEntity, TKey}.GetPagedListAsync(int, int, string, CancellationToken)"/>进行分页读取
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>当前实体<see cref="TEntity"/>的所有数据集，或者空集合</returns>
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> expression);

    /// <summary>
    /// 分页查询当前实体<see cref="TEntity"/>的数据集
    /// </summary>
    /// <param name="skipCount">排序后跳过的数据行数</param>
    /// <param name="maxResultCount">最大获取数据行数</param>
    /// <param name="sorting">需要包含排序字段和可选的排序指令(ASC 或 DESC)，可包含多个字段，使用逗号(,)分割。 例如： "Name ASC, Age DESC"</param>
    /// <param name="cancellationToken">可取消操作的令牌</param>
    /// <returns>符合条件的数据集</returns>
    Task<List<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件分页查询实体<see cref="TEntity"/>数据集
    /// </summary>
    /// <param name="expression">筛选条件</param>
    /// <param name="skipCount">排序后跳过的数据行数</param>
    /// <param name="maxResultCount">最大获取数据行数</param>
    /// <param name="sorting">需要包含排序字段和可选的排序指令(ASC 或 DESC)，可包含多个字段，使用逗号(,)分割。 例如： "Name ASC, Age DESC"</param>
    /// <param name="cancellationToken">可取消操作的令牌</param>
    /// <returns></returns>
    Task<List<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> expression, int skipCount, int maxResultCount, string sorting, CancellationToken cancellationToken = default);
}

/// <summary>
/// 所有域服务都可以实现此接口，以按约定识别它们.
/// 提供基本的数据操作接口
/// </summary>
public interface IDomainService<TEntity, TKey> : IDomainService where TEntity : class, IEntity<TKey> where TKey : notnull
{

    /// <summary>
    /// 获取给定主键的实体。
    /// 如果给定主键的实体不存在，则抛出 <see cref="EntityNotFoundException"/> 异常
    /// </summary>
    /// <param name="id">要获取实体的主键</param>
    /// <exception cref="EntityNotFoundException">给定主键的实体不存在时抛出</exception>
    /// <returns>符合条件的实体<see cref="TEntity"/></returns>
    Task<TEntity> GetAsync(TKey id);

    /// <summary>
    /// 获取给定主键的实体，如果没有找到返回null
    /// </summary>
    /// <param name="id">要获取实体的主键</param>
    /// <returns>符合条件的实体<see cref="TEntity"/>或者null</returns>
    Task<TEntity?> FindAsync(TKey id);

}
