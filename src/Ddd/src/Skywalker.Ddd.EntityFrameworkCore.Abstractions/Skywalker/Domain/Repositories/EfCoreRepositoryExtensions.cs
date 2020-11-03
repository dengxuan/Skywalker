﻿using System;
using Microsoft.EntityFrameworkCore;
using Skywalker.Domain.Entities;
using Skywalker.Domain.Repositories.EntityFrameworkCore;

namespace Skywalker.Domain.Repositories
{
    //TODO: Should work for any IRepository implementation

    public static class EfCoreRepositoryExtensions
    {
        public static DbContext GetDbContext<TEntity, TKey>(this IReadOnlyBasicRepository<TEntity, TKey> repository)
            where TEntity : class, IEntity<TKey>
        {
            return repository.ToEfCoreRepository().DbContext;
        }

        public static DbSet<TEntity> GetDbSet<TEntity, TKey>(this IReadOnlyBasicRepository<TEntity, TKey> repository)
            where TEntity : class, IEntity<TKey>
        {
            return repository.ToEfCoreRepository().DbSet;
        }

        public static IEfCoreRepository<TEntity, TKey> ToEfCoreRepository<TEntity, TKey>(this IReadOnlyBasicRepository<TEntity, TKey> repository)
            where TEntity : class, IEntity<TKey>
        {
            if (!(repository is IEfCoreRepository<TEntity, TKey> efCoreRepository))
            {
                throw new ArgumentException("Given repository does not implement " + typeof(IEfCoreRepository<TEntity, TKey>).AssemblyQualifiedName, nameof(repository));
            }

            return efCoreRepository;
        }
    }
}
