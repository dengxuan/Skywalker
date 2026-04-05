using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.Domain.Services;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;
using Skywalker.Ddd.Uow.EntityFrameworkCore;
using Skywalker.Identity.Domain.Repositories;
using System.Linq;


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Entity Framework Core ������չ������
/// </summary>
public static class EntityFrameworkCoreIServiceCollectionExtensions
{
    /// <summary>
    /// ͨ�� DbContext �������Ӳִ��������ȡ���е� DbSet Ȼ�󹹽��ִ���
    /// </summary>
    /// <typeparam name="TDbContext">DbContext ���͡�</typeparam>
    /// <param name="services">���񼯺ϡ�</param>
    /// <returns>���񼯺ϡ�</returns>
    private static IServiceCollection AddDefaultServices<TDbContext>(this IServiceCollection services) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        var dbContextType = typeof(TDbContext);
        var entityTypes = DbContextHelper.GetEntityTypes(dbContextType);
        foreach (var entityType in entityTypes)
        {
            var primaryKeyType = EntityHelper.FindPrimaryKeyType(entityType);
            if (primaryKeyType == null)
            {
                var repositoryImplType = typeof(Repository<,>).MakeGenericType(dbContextType, entityType);
                RegisterDefaultRepository(services, entityType, repositoryImplType);

                var domainServiceImplType = typeof(EntityFrameworkCoreDomainService<>).MakeGenericType(entityType);
                RegisterDefaultDomainService(services, entityType, domainServiceImplType);
            }
            else
            {
                var repositoryImplType = typeof(Repository<,,>).MakeGenericType(dbContextType, entityType, primaryKeyType);
                RegisterDefaultRepository(services, entityType, repositoryImplType, primaryKeyType);

                var domainServiceImplType = typeof(EntityFrameworkCoreDomainService<,>).MakeGenericType(entityType, primaryKeyType);
                RegisterDefaultDomainService(services, entityType, domainServiceImplType, primaryKeyType);
            }
        }
        return services;
    }

    private static void RegisterDefaultRepository(IServiceCollection services, Type entityType, Type repositoryImplType, Type? primaryKeyType = null)
    {
        if (primaryKeyType != null)
        {
            TryAddTransient(services, typeof(IReadOnlyRepository<,>).MakeGenericType(entityType, primaryKeyType), repositoryImplType);
            TryAddTransient(services, typeof(IBasicRepository<,>).MakeGenericType(entityType, primaryKeyType), repositoryImplType);
            TryAddTransient(services, typeof(IRepository<,>).MakeGenericType(entityType, primaryKeyType), repositoryImplType);
        }

        TryAddTransient(services, typeof(IReadOnlyRepository<>).MakeGenericType(entityType), repositoryImplType);
        TryAddTransient(services, typeof(IBasicRepository<>).MakeGenericType(entityType), repositoryImplType);
        TryAddTransient(services, typeof(IRepository<>).MakeGenericType(entityType), repositoryImplType);
    }

    private static void RegisterDefaultDomainService(IServiceCollection services, Type entityType, Type domainServiceImplType, Type? primaryKeyType = null)
    {
        if (primaryKeyType != null)
        {
            TryAddTransient(services, typeof(IDomainService<,>).MakeGenericType(entityType, primaryKeyType), domainServiceImplType);
        }

        TryAddTransient(services, typeof(IDomainService<>).MakeGenericType(entityType), domainServiceImplType);
    }

    private static void TryAddTransient(IServiceCollection services, Type serviceType, Type implementationType)
    {
        if (serviceType.IsAssignableFrom(implementationType))
        {
            services.TryAdd(ServiceDescriptor.Transient(serviceType, implementationType));
        }
    }

    /// <summary>
    /// 添加 DbContext。
    /// </summary>
    /// <typeparam name="TDbContext">DbContext ���͡�</typeparam>
    /// <param name="services">���񼯺ϡ�</param>
    /// <param name="options">DbContext ���á�</param>
    /// <returns>���񼯺ϡ�</returns>
    public static IServiceCollection AddSkywalkerDbContext<TDbContext>(this IServiceCollection services, Action<SkywalkerDbContextOptions> options) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        services.Configure(options);
        services.AddDefaultServices<TDbContext>();
        services.AddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
        services.AddDbContext<TDbContext>();
        return services;
    }

    /// <summary>
    /// ���� DbContext ���ӳء�
    /// </summary>
    /// <typeparam name="TDbContext">DbContext ���͡�</typeparam>
    /// <param name="services">���񼯺ϡ�</param>
    /// <param name="options">DbContext ���á�</param>
    /// <returns>���񼯺ϡ�</returns>
    public static IServiceCollection AddSkywalkerDbContextPool<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        services.Configure(options);
        services.AddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
        services.AddSingleton<IDbContextProvider<TDbContext>, UnitOfWorkDbContextProvider<TDbContext>>();
        services.AddDbContextPool<TDbContext>(options);
        return services;
    }
}
