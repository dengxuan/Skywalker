using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.Infrastructure;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Ddd.Infrastructure.EntityFrameworkCore;
using Skywalker.Ddd.Infrastruture.Extensions.EntityFrameworkCore;
using Skywalker.Domain.Entities;
using Skywalker.EntityFrameworkCore;
using Skywalker.EntityFrameworkCore.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InfrastructureSkywalkerBuilderExtensions
    {
        public static SkywalkerRepositoryInitializer AddEntityFrameworkCore<TDbContext>(this SkywalkerRepositoryInitializer builder, Action<SkywalkerDbContextOptions> optionsBuilder) where TDbContext : SkywalkerDbContext<TDbContext>
        {
            builder.Services.Configure(optionsBuilder);
            builder.Services.AddMemoryCache();
            builder.Services.TryAddTransient(DbContextOptionsFactory.Create<TDbContext>);
            builder.Services.AddTransient(typeof(ISkywalkerDbContextProvider<>), typeof(SkywalkerDbContextProvider<>));
            builder.Services.AddDbContext<TDbContext>();
            builder.Initialize(typeof(TDbContext), new EntityFrameworkCoreDatabaseInitializer<TDbContext>(builder.Services));
            return builder;
        }
    }

    internal class EntityFrameworkCoreDatabaseInitializer<TDbContext> : ISkywalkerDatabaseInitializer where TDbContext : DbContext
    {
        IServiceCollection Services { get; }

        public EntityFrameworkCoreDatabaseInitializer(IServiceCollection services)
        {
            Services = services;
        }

        public void AddDatabases(IEnumerable<Type> entityTypes)
        {
            foreach (var entityType in entityTypes)
            {
                Type databaseType = typeof(ISkywalkerDatabase<>).MakeGenericType(entityType);
                Type databaseTypeImplementionType = typeof(SkywalkerEntityFrameworkCoreDatabase<,>).MakeGenericType(typeof(TDbContext), entityType);
                Services.AddTransient(databaseType, databaseTypeImplementionType);
                var primaryKeyType = EntityHelper.FindPrimaryKeyType(entityType);
                if (primaryKeyType != null)
                {
                    Type databaseTypeWithPrimaryKey = typeof(ISkywalkerDatabase<,>).MakeGenericType(entityType, primaryKeyType);
                    Type databaseTypeImplementionTypeWithPrimaryKey = typeof(SkywalkerEntityFrameworkCoreDatabase<,,>).MakeGenericType(typeof(TDbContext), entityType, primaryKeyType);
                    Services.AddTransient(databaseTypeWithPrimaryKey, databaseTypeImplementionTypeWithPrimaryKey);
                }
            }
        }
    }
}
