// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Linq.Expressions;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.Uow;
using Skywalker.Extensions.Collections.Generic;
using Skywalker.Extensions.Specifications;

namespace Skywalker.Ddd.Domain.Services;

/// <summary>
/// 领域服务基类，自动启用 UnitOfWork 拦截。
/// 默认为 Scoped 生命周期。
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <param name="repository">仓储实例</param>
/// <param name="asyncExecuter">异步查询执行器</param>
[UnitOfWork]
public class DomainService<TEntity>(IRepository<TEntity> repository, IAsyncQueryableExecuter asyncExecuter) : IDomainService<TEntity> where TEntity : class, IEntity
{
    /// <summary>
    /// 仓储实例，<see cref="IRepository{TEntity, TKey}"/>
    /// 提供对<typeparamref name="TEntity"/>的CRUD操作
    /// </summary>
    protected virtual IRepository<TEntity> Repository { get; } = repository;

    /// <summary>
    /// 异步查询执行器，添加对异步查询的支持<see cref="IAsyncQueryableExecuter"/>,<seealso cref="IAsyncQueryableProvider"/>
    /// </summary>
    protected virtual IAsyncQueryableExecuter AsyncExecuter { get; } = asyncExecuter;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default) => Repository.InsertAsync(entity, autoSave, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default) => Repository.UpdateAsync(entity, autoSave, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default) => Repository.DeleteAsync(entity, autoSave, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<int> CountAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<long> LongCountAsync(CancellationToken cancellationToken = default) => Repository.LongCountAsync(cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default) => Repository.CountAsync(expression, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default) => Repository.LongCountAsync(expression, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default) => Repository.AnyAsync(filter, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<TEntity> GetAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default) => Repository.GetAsync((ISpecification<TEntity>)specification, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default) => Repository.GetAsync(expression, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<TEntity?> FindAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default) => Repository.FindAsync((ISpecification<TEntity>)specification, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default) => Repository.FindAsync(expression, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default) => Repository.GetListAsync(cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default) => AsyncExecuter.ToListAsync(Repository.Where(expression), cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, CancellationToken cancellationToken = default) => Repository.GetPagedListAsync(skip, limit, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, string? sorting, CancellationToken cancellationToken = default) => Repository.GetPagedListAsync(skip, limit, sorting, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, string? sorting, string? filter, CancellationToken cancellationToken = default) => Repository.GetPagedListAsync(skip, limit, sorting, filter, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> expression, int skip, int limit, CancellationToken cancellationToken = default) => Repository.GetPagedListAsync(expression, skip, limit, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> expression, int skip, int limit, string? sorting, CancellationToken cancellationToken = default) => Repository.GetPagedListAsync(expression, skip, limit, sorting, cancellationToken);

}

/// <inheritdoc/>
[UnitOfWork]
public class DomainService<TEntity, TKey>(IRepository<TEntity, TKey> repository, IAsyncQueryableExecuter asyncExecuter) : DomainService<TEntity>(repository, asyncExecuter), IDomainService<TEntity, TKey> where TEntity : class, IEntity<TKey> where TKey : notnull
{
    /// <inheritdoc/>
    protected new virtual IRepository<TEntity, TKey> Repository { get; } = repository;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<bool> AnyAsync(TKey id, CancellationToken cancellationToken = default) => Repository.AnyAsync(id, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default) => Repository.FindAsync(id, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default) => Repository.GetAsync(id, cancellationToken);
}
