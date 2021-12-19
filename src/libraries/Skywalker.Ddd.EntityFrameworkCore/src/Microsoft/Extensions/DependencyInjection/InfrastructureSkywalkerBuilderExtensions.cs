using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;
using Skywalker.Ddd.EntityFrameworkCore.Repositories;
using Skywalker.Domain.Entities;
using Skywalker.Extensions.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InfrastructureSkywalkerBuilderExtensions
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

        public static IServiceCollection AddEntityFrameworkCore<TDbContext>(this IServiceCollection services, Action<SkywalkerDbContextOptions> optionsBuilder) where TDbContext : SkywalkerDbContext<TDbContext>
        {
            services.Configure(optionsBuilder);
            services.AddMemoryCache();
            services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
            //skywalker.Services.AddTransient(typeof(IDbContextProvider<>), typeof(DbContextProvider<>));
            services.TryAddTransient<IAsyncQueryableProvider, EfCoreAsyncQueryableProvider>();
            services.AddDbContext<TDbContext>();
            services.AddDomainServices();
            SkywalkerDbContextRegistrationOptions options = new(typeof(TDbContext), services);
            SkywalkerRepositoryRegistrar repositoryRegistrar = new(options);
            IEnumerable<Type> entityTypes = DbContextHelper.GetEntityTypes<TDbContext>();
            repositoryRegistrar.AddRepositories(entityTypes);
            return services;
        }
    }
}
