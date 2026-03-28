// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.Data.Filtering;
using Skywalker.Ddd.Data.Seeding;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.Domain.Services;
using Skywalker.Extensions.Threading;


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// DDD Domain ������չ������
/// </summary>
public static class DomainDddBuilderExtensions
{
    /// <summary>
    /// 添加 DDD Domain 模块服务到服务集合。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddDddDomain(this IServiceCollection services)
    {
        services.AddDddExceptions();
        services.AddEventBusCore();
        return services.AddAutoServices();
    }

    private static void RegisterService(IServiceCollection services, Type serviceType, Type implementationType, bool replaceExisting, bool isReadOnlyRepository = false)
    {
        var descriptor = ServiceDescriptor.Transient(serviceType, implementationType);

        if (replaceExisting)
        {
            services.Replace(descriptor);
        }
        else
        {
            services.TryAdd(descriptor);
        }
    }

    internal static IServiceCollection AddDataFilter(this IServiceCollection services)
    {
        SkywalkerDddDomainAutoServiceExtensions.AddAutoServices(services);
        services.TryAddSingleton(typeof(IDataFilter<>), typeof(DataFilter<>));

        return services;
    }

    internal static IServiceCollection AddThreadingServices(this IServiceCollection services)
    {
        services.TryAddScoped<ICancellationTokenProvider>(sp => NullCancellationTokenProvider.Instance);
        services.TryAddSingleton(typeof(IAmbientScopeProvider<>), typeof(AmbientDataContextAmbientScopeProvider<>));
        return services;
    }

    internal static IServiceCollection AddDefaultRepository(this IServiceCollection services, Type entityType, Type repositoryImplementationType, bool replaceExisting = false)
    {
        //IReadOnlyRepository<TEntity>
        var readOnlyRepositoryInterface = typeof(IReadOnlyRepository<>).MakeGenericType(entityType);
        if (readOnlyRepositoryInterface.IsAssignableFrom(repositoryImplementationType))
        {
            RegisterService(services, readOnlyRepositoryInterface, repositoryImplementationType, replaceExisting, true);
        }

        //IBasicRepository<TEntity>
        var basicRepositoryInterface = typeof(IBasicRepository<>).MakeGenericType(entityType);
        if (basicRepositoryInterface.IsAssignableFrom(repositoryImplementationType))
        {
            RegisterService(services, basicRepositoryInterface, repositoryImplementationType, replaceExisting);
        }

        //IRepository<TEntity>
        var repositoryInterface = typeof(IRepository<>).MakeGenericType(entityType);
        if (repositoryInterface.IsAssignableFrom(repositoryImplementationType))
        {
            RegisterService(services, repositoryInterface, repositoryImplementationType, replaceExisting);
        }

        return services;
    }

    internal static IServiceCollection AddDefaultRepository(this IServiceCollection services, Type entityType, Type repositoryImplementationType, Type primaryKeyType, bool replaceExisting = false)
    {
        //IReadOnlyRepository<TEntity, TKey>
        var readOnlyRepositoryInterfaceWithPk = typeof(IReadOnlyRepository<,>).MakeGenericType(entityType, primaryKeyType);
        if (readOnlyRepositoryInterfaceWithPk.IsAssignableFrom(repositoryImplementationType))
        {
            RegisterService(services, readOnlyRepositoryInterfaceWithPk, repositoryImplementationType, replaceExisting, true);
        }

        //IBasicRepository<TEntity, TKey>
        var basicRepositoryInterfaceWithPk = typeof(IBasicRepository<,>).MakeGenericType(entityType, primaryKeyType);
        if (basicRepositoryInterfaceWithPk.IsAssignableFrom(repositoryImplementationType))
        {
            RegisterService(services, basicRepositoryInterfaceWithPk, repositoryImplementationType, replaceExisting);
        }

        //IRepository<TEntity, TKey>
        var repositoryInterfaceWithPk = typeof(IRepository<,>).MakeGenericType(entityType, primaryKeyType);
        if (repositoryInterfaceWithPk.IsAssignableFrom(repositoryImplementationType))
        {
            RegisterService(services, repositoryInterfaceWithPk, repositoryImplementationType, replaceExisting);
        }

        return services;
    }

    internal static IServiceCollection AddDefaultDomainService(this IServiceCollection services, Type entityType, Type domainServiceImplementationType, bool replaceExisting = false)
    {
        //IDomainService<TEntity>
        var domainServiceInterface = typeof(IDomainService<>).MakeGenericType(entityType);
        if (domainServiceInterface.IsAssignableFrom(domainServiceImplementationType))
        {
            RegisterService(services, domainServiceInterface, domainServiceImplementationType, replaceExisting);
        }

        return services;
    }

    internal static IServiceCollection AddDefaultDomainService(this IServiceCollection services, Type entityType, Type domainServiceImplementationType, Type primaryKeyType, bool replaceExisting = false)
    {
        //IDomainService<TEntity, TKey>
        var domainServiceInterface = typeof(IDomainService<,>).MakeGenericType(entityType, primaryKeyType);
        if (domainServiceInterface.IsAssignableFrom(domainServiceImplementationType))
        {
            RegisterService(services, domainServiceInterface, domainServiceImplementationType, replaceExisting);
        }

        return services;
    }
}
