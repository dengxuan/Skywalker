// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Linq.Expressions;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Extensions.Linq;

namespace Skywalker.Ddd.Domain.Services;

public class DomainService<TEntity> : DomainService<TEntity, Guid>, IDomainService<TEntity> where TEntity : class, IEntity<Guid>
{
    public DomainService(IRepository<TEntity, Guid> repository, IAsyncQueryableExecuter asyncExecuter) : base(repository, asyncExecuter)
    {
    }
}

public class DomainService<TEntity, TKey> : IDomainService<TEntity, TKey> where TEntity : class, IEntity<TKey> where TKey : notnull
{
    protected IRepository<TEntity, TKey> Repository { get; }

    protected IAsyncQueryableExecuter AsyncExecuter { get; }

    public DomainService(IRepository<TEntity, TKey> repository, IAsyncQueryableExecuter asyncExecuter)
    {
        Repository = repository;
        AsyncExecuter = asyncExecuter;
    }

    public Task<TEntity?> FindAsync(TKey id) => Repository.FindAsync(id);

    public Task<TEntity> GetAsync(TKey id) => Repository.GetAsync(id);

    public Task<List<TEntity>> GetListAsync() => Repository.GetListAsync();

    public Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> expression) => AsyncExecuter.ToListAsync(Repository.Where(expression));

    public Task<List<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, CancellationToken cancellationToken = default) => Repository.GetPagedListAsync(skipCount, maxResultCount, sorting, cancellationToken);

    public Task<List<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> expression, int skipCount, int maxResultCount, string sorting, CancellationToken cancellationToken = default) => Repository.GetPagedListAsync(expression, skipCount, maxResultCount, sorting, cancellationToken);

}
