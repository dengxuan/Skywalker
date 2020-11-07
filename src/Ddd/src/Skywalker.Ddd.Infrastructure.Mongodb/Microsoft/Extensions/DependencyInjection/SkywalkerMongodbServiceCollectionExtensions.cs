using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.Infrastructure;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Ddd.Infrastructure.Mongodb;
using Skywalker.Ddd.Mongodb;
using Skywalker.Domain.Entities;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SkywalkerMongodbServiceCollectionExtensions
    {
        public static SkywalkerRepositoryInitializer AddMongodb<TDbContext>(this SkywalkerRepositoryInitializer initializer) where TDbContext : IMongodbContext
        {
            initializer.Services.TryAddTransient(typeof(IMongoDbContextProvider<>), typeof(DefaultMongoDbContextProvider<>));

            initializer.Services.TryAddTransient(typeof(IMongoDbRepositoryFilterer<>), typeof(MongoDbRepositoryFilterer<>));

            initializer.Services.TryAddTransient(typeof(IMongoDbRepositoryFilterer<,>), typeof(MongoDbRepositoryFilterer<,>));

            //initializer.Services.AddDbContext<TDbContext>();
            initializer.Initialize(typeof(TDbContext), new MongodbDatabaseInitializer<TDbContext>(initializer.Services));

            return initializer;
        }
    }


    internal class MongodbDatabaseInitializer<TDbContext> : ISkywalkerDatabaseInitializer where TDbContext : IMongodbContext
    {
        IServiceCollection Services { get; }

        public MongodbDatabaseInitializer(IServiceCollection services)
        {
            Services = services;
        }

        public void AddDatabases(IEnumerable<Type> entityTypes)
        {
            foreach (var entityType in entityTypes)
            {
                Type databaseType = typeof(ISkywalkerDatabase<>).MakeGenericType(entityType);
                Type databaseTypeImplementionType = typeof(SkywalkerMongoDatabase<,>).MakeGenericType(typeof(TDbContext), entityType);
                Services.AddTransient(databaseType, databaseTypeImplementionType);
                var primaryKeyType = EntityHelper.FindPrimaryKeyType(entityType);
                if (primaryKeyType != null)
                {
                    Type databaseTypeWithPrimaryKey = typeof(ISkywalkerDatabase<,>).MakeGenericType(entityType, primaryKeyType);
                    Type databaseTypeImplementionTypeWithPrimaryKey = typeof(SkywalkerMongoDatabase<,,>).MakeGenericType(typeof(TDbContext), entityType, primaryKeyType);
                    Services.AddTransient(databaseTypeWithPrimaryKey, databaseTypeImplementionTypeWithPrimaryKey);
                }
            }
        }
    }
}
