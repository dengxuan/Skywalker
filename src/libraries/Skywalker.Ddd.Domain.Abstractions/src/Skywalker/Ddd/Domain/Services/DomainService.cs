// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Linq.Expressions;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.Uow;
using Skywalker.Extensions.Collections.Generic;

namespace Skywalker.Ddd.Domain.Services;

/// <summary>
/// <inheritdoc/>
/// </summary>
public class DomainService<TEntity> : IDomainService<TEntity> where TEntity : class, IEntity
{
    /// <summary>
    /// 仓储实例，<see cref="IRepository{TEntity, TKey}"/>
    /// 提供对<see cref="TEntity"/>的CRUD操作
    /// </summary>
    protected virtual IRepository<TEntity> Repository { get; }

    /// <summary>
    /// 异步查询执行器，添加对异步查询的支持<see cref="IAsyncQueryableExecuter"/>,<seealso cref="IAsyncQueryableProvider"/>
    /// </summary>
    protected virtual IAsyncQueryableExecuter AsyncExecuter { get; }

    public DomainService(IRepository<TEntity> repository, IAsyncQueryableExecuter asyncExecuter)
    {
        Repository = repository;
        AsyncExecuter = asyncExecuter;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [UnitOfWork]
    public Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default) => Repository.InsertAsync(entity, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [UnitOfWork]
    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) => Repository.UpdateAsync(entity, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [UnitOfWork]
    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default) => Repository.DeleteAsync(entity, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<int> CountAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<long> LongCountAsync(CancellationToken cancellationToken = default) => Repository.LongCountAsync(cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default) => Repository.CountAsync(expression, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<long> LongCountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default) => Repository.LongCountAsync(expression, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default) => Repository.AnyAsync(filter, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default) => Repository.GetListAsync(cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default) => AsyncExecuter.ToListAsync(Repository.Where(expression), cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, CancellationToken cancellationToken = default) => Repository.GetPagedListAsync(skip, limit, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, string sorting, CancellationToken cancellationToken = default) => Repository.GetPagedListAsync(skip, limit, sorting, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> expression, int skip, int limit, CancellationToken cancellationToken = default) => Repository.GetPagedListAsync(skip, limit, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> expression, int skip, int limit, string sorting, CancellationToken cancellationToken = default) => Repository.GetPagedListAsync(expression, skip, limit, sorting, cancellationToken);

}

/// <summary>
/// <inheritdoc/>
/// </summary>
public class DomainService<TEntity, TKey> : DomainService<TEntity>, IDomainService<TEntity, TKey> where TEntity : class, IEntity<TKey> where TKey : notnull
{
    protected new virtual IRepository<TEntity, TKey> Repository { get; }

    public DomainService(IRepository<TEntity, TKey> repository, IAsyncQueryableExecuter asyncExecuter) : base(repository, asyncExecuter)
    {
        Repository = repository;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<bool> AnyAsync(TKey id, CancellationToken cancellationToken = default) => Repository.AnyAsync(id, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default) => Repository.FindAsync(id, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default) => Repository.GetAsync(id, cancellationToken);
}
