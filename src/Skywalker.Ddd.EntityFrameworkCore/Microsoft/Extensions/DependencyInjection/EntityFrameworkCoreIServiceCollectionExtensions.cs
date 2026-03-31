using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;
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
    /// ���� Entity Framework Core ֧�֡�
    /// </summary>
    /// <param name="services">���񼯺ϡ�</param>
    /// <returns>���񼯺ϡ�</returns>
    public static IServiceCollection AddEntityFrameworkCore(this IServiceCollection services)
    {
        services.AddDataFilter();
        services.AddThreadingServices();
        services.AddGuidGenerator();
        services.AddTimezone();
        services.AddSingleton(typeof(IDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>));
        services.TryAddTransient<IAsyncQueryableProvider, EfCoreAsyncQueryableProvider>();

        return services;
    }

    /// <summary>
    /// ���� DbContext��
    /// </summary>
    /// <typeparam name="TDbContext">DbContext ���͡�</typeparam>
    /// <param name="services">���񼯺ϡ�</param>
    /// <param name="options">DbContext ���á�</param>
    /// <returns>���񼯺ϡ�</returns>
    public static IServiceCollection AddSkywalkerDbContext<TDbContext>(this IServiceCollection services, Action<SkywalkerDbContextOptions> options) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        services.AddEntityFrameworkCore();
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
