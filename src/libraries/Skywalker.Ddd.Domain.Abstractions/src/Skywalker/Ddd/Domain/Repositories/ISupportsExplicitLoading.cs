using System.Linq.Expressions;
using Skywalker.Ddd.Domain.Entities;

namespace Skywalker.Ddd.Domain.Repositories;

public interface ISupportsExplicitLoading<TEntity, TKey> where TEntity : class, IEntity<TKey> where TKey : notnull
{
    Task EnsureCollectionLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken) where TProperty : class;

    Task EnsurePropertyLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty?>> propertyExpression, CancellationToken cancellationToken) where TProperty : class;
}
