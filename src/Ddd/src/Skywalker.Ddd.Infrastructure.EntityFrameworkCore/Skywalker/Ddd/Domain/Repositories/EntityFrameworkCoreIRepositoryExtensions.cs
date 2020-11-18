using Microsoft.EntityFrameworkCore;
using Skywalker.Domain.Entities;
using Skywalker.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Skywalker.Ddd.Domain.Repositories
{
    public static class EntityFrameworkCoreIRepositoryExtensions
    {

        public static IQueryable<TEntity> WithDetails<TEntity>(this IRepository<TEntity> repository, params Expression<Func<TEntity, object>>[] propertySelectors) where TEntity : class, IEntity
        {
            if (repository is not IQueryable<TEntity> query)
            {
                throw new ArgumentException("Repository must be IReadOnlyRepository");
            }

            if (!propertySelectors.IsNullOrEmpty())
            {
                foreach (var propertySelector in propertySelectors)
                {
                    query = query.Include(propertySelector);
                }
            }
            return query;
        }
    }
}
