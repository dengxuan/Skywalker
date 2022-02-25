// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Linq.Expressions;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Extensions.Linq;

namespace Skywalker.Ddd.Domain.Services;

/// <summary>
/// <inheritdoc/>
/// </summary>
public class DomainService<TEntity> : DomainService<TEntity, Guid>, IDomainService<TEntity> where TEntity : class, IEntity<Guid>
{
    public DomainService(IRepository<TEntity, Guid> repository, IAsyncQueryableExecuter asyncExecuter) : base(repository, asyncExecuter)
    {
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default) => Repository.DeleteAsync(entity, cancellationToken);

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
    public Task<List<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, CancellationToken cancellationToken = default) => Repository.GetPagedListAsync(skipCount, maxResultCount, sorting, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<List<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> expression, int skipCount, int maxResultCount, string sorting, CancellationToken cancellationToken = default) => Repository.GetPagedListAsync(expression, skipCount, maxResultCount, sorting, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default) => Repository.InsertAsync(entity, cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) => Repository.UpdateAsync(entity, cancellationToken);
}

/// <summary>
/// <inheritdoc/>
/// </summary>
public class DomainService<TEntity, TKey> : IDomainService<TEntity, TKey> where TEntity : class, IEntity<TKey> where TKey : notnull
{
    /// <summary>
    /// 仓储实例，<see cref="IRepository{TEntity, TKey}"/>
    /// 提供对<see cref="TEntity"/>的CRUD操作
    /// </summary>
    protected IRepository<TEntity, TKey> Repository { get; }

    /// <summary>
    /// 异步查询执行器，添加对异步查询的支持<see cref="IAsyncQueryableExecuter"/>,<seealso cref="IAsyncQueryableProvider"/>
    /// </summary>
    protected IAsyncQueryableExecuter AsyncExecuter { get; }

    public DomainService(IRepository<TEntity, TKey> repository, IAsyncQueryableExecuter asyncExecuter)
    {
        Repository = repository;
        AsyncExecuter = asyncExecuter;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default) => Repository.FindAsync(id,cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default) => Repository.GetAsync(id, cancellationToken);

}
