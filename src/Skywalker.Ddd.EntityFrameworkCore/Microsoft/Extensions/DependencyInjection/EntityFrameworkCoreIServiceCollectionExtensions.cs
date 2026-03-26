using Microsoft.EntityFrameworkCore;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;
using Skywalker.Ddd.Uow.EntityFrameworkCore;
using Skywalker.Identity.Domain.Repositories;


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Entity Framework Core 服务扩展方法。
/// </summary>
public static class EntityFrameworkCoreIServiceCollectionExtensions
{
    /// <summary>
    /// 通过 DbContext 类型添加仓储，反射获取所有的 DbSet 然后构建仓储。
    /// </summary>
    /// <typeparam name="TDbContext">DbContext 类型。</typeparam>
    /// <param name="services">服务集合。</param>
    /// <returns>服务集合。</returns>
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
                services.AddDefaultRepository(entityType, repositoryImplType, false);

                var domainServiceImplType = typeof(EntityFrameworkCoreDomainService<>).MakeGenericType(entityType);
                services.AddDefaultDomainService(entityType, domainServiceImplType, false);
            }
            else
            {
                var repositoryImplType = typeof(Repository<,,>).MakeGenericType(dbContextType, entityType, primaryKeyType);
                services.AddDefaultRepository(entityType, repositoryImplType, primaryKeyType, false);

                var domainServiceImplType = typeof(EntityFrameworkCoreDomainService<,>).MakeGenericType(entityType, primaryKeyType);
                services.AddDefaultDomainService(entityType, domainServiceImplType, primaryKeyType, false);
            }
        }
        return services;
    }

    /// <summary>
    /// 添加 Entity Framework Core 支持。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddEntityFrameworkCore(this IServiceCollection services)
    {
        services.AddDataFilter();
        services.AddThreadingServices();
        services.AddGuidGenerator();
        services.AddTimezone();
        SkywalkerDddEntityFrameworkCoreAutoServiceExtensions.AddAutoServices(services);
        services.AddSingleton(typeof(IDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>));

        return services;
    }

    /// <summary>
    /// 添加 DbContext。
    /// </summary>
    /// <typeparam name="TDbContext">DbContext 类型。</typeparam>
    /// <param name="services">服务集合。</param>
    /// <param name="options">DbContext 配置。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddSkywalkerDbContext<TDbContext>(this IServiceCollection services, Action<SkywalkerDbContextOptions> options) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        services.Configure(options);
        services.AddDefaultServices<TDbContext>();
        services.AddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
        services.AddDbContext<TDbContext>();
        return services;
    }

    /// <summary>
    /// 添加 DbContext 连接池。
    /// </summary>
    /// <typeparam name="TDbContext">DbContext 类型。</typeparam>
    /// <param name="services">服务集合。</param>
    /// <param name="options">DbContext 配置。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddSkywalkerDbContextPool<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        services.Configure(options);
        services.AddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
        services.AddSingleton<IDbContextProvider<TDbContext>, UnitOfWorkDbContextProvider<TDbContext>>();
        services.AddDbContextPool<TDbContext>(options);
        return services;
    }
}
