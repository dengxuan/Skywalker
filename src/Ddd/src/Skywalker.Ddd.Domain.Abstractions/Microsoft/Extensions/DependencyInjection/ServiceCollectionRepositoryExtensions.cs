﻿using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Data;
using Skywalker.Data.Filtering;
using Skywalker.Data.Seeding;
using Skywalker.Domain.Entities;
using Skywalker.Domain.Entities.Events.Distributed;
using Skywalker.Domain.Repositories;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionRepositoryExtensions
    {
        public static IServiceCollection AddDefaultRepository(this IServiceCollection services, Type entityType, Type repositoryImplementationType)
        {

            //IReadOnlyRepository<TEntity>
            var readOnlyRepositoryInterface = typeof(IReadOnlyRepository<>).MakeGenericType(entityType);
            if (readOnlyRepositoryInterface.IsAssignableFrom(repositoryImplementationType))
            {
                services.TryAddTransient(readOnlyRepositoryInterface, repositoryImplementationType);
            }

            //IBasicRepository<TEntity>
            var basicRepositoryInterface = typeof(IBasicRepository<>).MakeGenericType(entityType);
            if (basicRepositoryInterface.IsAssignableFrom(repositoryImplementationType))
            {
                services.TryAddTransient(basicRepositoryInterface, repositoryImplementationType);

                //IRepository<TEntity>
                var repositoryInterface = typeof(IRepository<>).MakeGenericType(entityType);
                if (repositoryInterface.IsAssignableFrom(repositoryImplementationType))
                {
                    services.TryAddTransient(repositoryInterface, repositoryImplementationType);
                }
            }

            var primaryKeyType = EntityHelper.FindPrimaryKeyType(entityType);
            if (primaryKeyType != null)
            {

                //IReadOnlyRepository<TEntity, TKey>
                var readOnlyRepositoryInterfaceWithPk = typeof(IReadOnlyRepository<,>).MakeGenericType(entityType, primaryKeyType);
                if (readOnlyRepositoryInterfaceWithPk.IsAssignableFrom(repositoryImplementationType))
                {
                    services.TryAddTransient(readOnlyRepositoryInterfaceWithPk, repositoryImplementationType);
                }

                //IBasicRepository<TEntity, TKey>
                var basicRepositoryInterfaceWithPk = typeof(IBasicRepository<,>).MakeGenericType(entityType, primaryKeyType);
                if (basicRepositoryInterfaceWithPk.IsAssignableFrom(repositoryImplementationType))
                {
                    services.TryAddTransient(basicRepositoryInterfaceWithPk, repositoryImplementationType);

                    //IRepository<TEntity, TKey>
                    var repositoryInterfaceWithPk = typeof(IRepository<,>).MakeGenericType(entityType, primaryKeyType);
                    if (repositoryInterfaceWithPk.IsAssignableFrom(repositoryImplementationType))
                    {
                        services.TryAddTransient(repositoryInterfaceWithPk, repositoryImplementationType);
                    }
                }
            }

            return services;
        }

        internal static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            var configuration = services.GetConfiguration();
            services.Configure<SkywalkerDbConnectionOptions>(configuration);

            services.AddSingleton(typeof(IDataFilter<>), typeof(DataFilter<>));
            services.AddSingleton<IDataSeeder, DataSeeder>();
            services.AddTransient<IConnectionStringResolver, DefaultConnectionStringResolver>();
            services.AddTransient<IEntityToEtoMapper, EntityToEtoMapper>();
            return services;
        }
    }
}