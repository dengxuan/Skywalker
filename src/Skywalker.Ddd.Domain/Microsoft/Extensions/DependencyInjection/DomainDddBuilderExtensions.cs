// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker;
using Skywalker.ApplicationParts;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.Data.Filtering;
using Skywalker.Ddd.Data.Seeding;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.Domain.Services;
using Skywalker.Extensions.Threading;


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// DDD Domain 模块扩展方法
/// </summary>
public static class DomainDddBuilderExtensions
{
    /// <summary>
    /// 添加 DDD Domain 模块，自动发现并注册所有 <see cref="IDomainService"/> 实现，
    /// 以及 Domain 层框架服务。
    /// </summary>
    /// <param name="builder">Skywalker 构建器。</param>
    /// <returns>Skywalker 构建器，支持链式调用。</returns>
    public static ISkywalkerBuilder AddDddDomain(this ISkywalkerBuilder builder)
    {
        var services = builder.Services;

        // 基础设施
        services.AddDddExceptions();
        services.AddEventBusCore();

        // UoW
        builder.AddUnitOfWork();

        // Domain 框架服务
        services.TryAddSingleton<IAsyncQueryableExecuter, AsyncQueryableExecuter>();
        services.TryAddSingleton<IDataSeeder, DataSeeder>();
        services.TryAddSingleton<IDataFilter, DataFilter>();
        services.TryAddSingleton(typeof(IDataFilter<>), typeof(DataFilter<>));
        services.TryAddSingleton<IConnectionStringResolver, DefaultConnectionStringResolver>();

        // 通过 FeatureProvider 发现并注册领域服务
        var provider = new DomainServiceFeatureProvider();
        builder.PartManager.FeatureProviders.Add(provider);

        var feature = new ServiceRegistrationFeature();
        provider.PopulateFeature(builder.PartManager.ApplicationParts, feature);

        foreach (var descriptor in feature.Services)
        {
            services.TryAdd(descriptor);
        }

        // 为 IDomainService（实现了 IInterceptable）启用代理
        services.AddInterceptedServices();

        return builder;
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
