// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.ApplicationParts;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.Data.Filtering;
using Skywalker.Ddd.Data.Seeding;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.Domain.Services;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.EventBus;
using Skywalker.EventBus.Abstractions;
using Skywalker.EventBus.Local;
using Skywalker.Extensions.DependencyInjection;
using Skywalker.Extensions.DynamicProxies;
using Skywalker.Extensions.Threading;

namespace Skywalker.Ddd.Tests;

#region Test Fixtures

public interface ITestProductDomainService : IDomainService
{
    string GetProduct();
}

public class TestProductDomainService : ITestProductDomainService
{
    public string GetProduct() => "TestProduct";
}

public interface ITestOrderApplicationService : Application.Abstractions.IApplicationService
{
    string PlaceOrder();
}

public class TestOrderApplicationService : ITestOrderApplicationService
{
    public string PlaceOrder() => "OrderPlaced";
}

public interface ITestDisabledDomainService : IDomainService
{
    string Nope();
}

[DisableAutoRegistration]
public class TestDisabledDomainService : ITestDisabledDomainService
{
    public string Nope() => "Should not be registered";
}

/// <summary>
/// 用于 Repository 和 generic IDomainService 注册测试的实体。
/// </summary>
public class TestOrder : Entity<int>
{
    public string Name { get; set; } = default!;
}

public class TestItem : Entity
{
    public string Title { get; set; } = default!;
    public override object[] GetKeys() => [Title];
}

public class TestDbContext(DbContextOptions<TestDbContext> options) : SkywalkerDbContext<TestDbContext>(options)
{
    public DbSet<TestOrder> Orders { get; set; } = default!;
    public DbSet<TestItem> Items { get; set; } = default!;
}

#endregion

/// <summary>
/// 验证 AddSkywalker() 一站式注册能自动发现 FeatureProvider 并正确注册所有服务。
/// </summary>
public class FeatureProviderRegistrationTests
{
    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        services.AddSkywalker(typeof(FeatureProviderRegistrationTests).Assembly);
        return services.BuildServiceProvider();
    }

    #region UoW Services (UowServiceFeatureProvider)

    [Fact]
    public void AddSkywalker_RegistersUnitOfWorkManager()
    {
        using var provider = BuildProvider();
        var mgr = provider.GetService<IUnitOfWorkManager>();
        Assert.NotNull(mgr);
        Assert.IsType<UnitOfWorkManager>(mgr);
    }

    [Fact]
    public void AddSkywalker_RegistersAmbientUnitOfWork()
    {
        using var provider = BuildProvider();
        var ambient = provider.GetService<IAmbientUnitOfWork>();
        var accessor = provider.GetService<IUnitOfWorkAccessor>();
        Assert.NotNull(ambient);
        Assert.NotNull(accessor);
        Assert.Same(ambient, accessor);
    }

    [Fact]
    public void AddSkywalker_RegistersUnitOfWork_Transient()
    {
        using var provider = BuildProvider();
        var uow1 = provider.GetService<IUnitOfWork>();
        var uow2 = provider.GetService<IUnitOfWork>();
        Assert.NotNull(uow1);
        Assert.NotNull(uow2);
        Assert.NotSame(uow1, uow2);
    }

    [Fact]
    public void AddSkywalker_RegistersUnitOfWorkInterceptor()
    {
        using var provider = BuildProvider();
        var interceptor = provider.GetService<IInterceptor>();
        Assert.NotNull(interceptor);
        Assert.IsType<UnitOfWorkInterceptor>(interceptor);
    }

    #endregion

    #region Domain Services (DomainServiceFeatureProvider)

    [Fact]
    public void AddSkywalker_RegistersDomainFrameworkServices()
    {
        using var provider = BuildProvider();
        Assert.NotNull(provider.GetService<IAsyncQueryableExecuter>());
        Assert.NotNull(provider.GetService<IDataSeeder>());
        Assert.NotNull(provider.GetService<IDataFilter>());
        Assert.NotNull(provider.GetService<IConnectionStringResolver>());
    }

    [Fact]
    public void AddSkywalker_RegistersExceptionsService()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var cached = scope.ServiceProvider.GetService<ICachedServiceProvider>();
        Assert.NotNull(cached);
    }

    [Fact]
    public async Task AddSkywalker_RegistersEventBusLocal()
    {
        await using var provider = BuildProvider();
        var eventBus = provider.GetService<IEventBus>();
        var localBus = provider.GetService<ILocalEventBus>();
        var dispatcher = provider.GetService<IDomainEventDispatcher>();
        Assert.NotNull(eventBus);
        Assert.NotNull(localBus);
        Assert.NotNull(dispatcher);
    }

    [Fact]
    public void AddSkywalker_RegistersEventBusCore()
    {
        using var provider = BuildProvider();
        Assert.NotNull(provider.GetService<IEventHandlerFactory>());
        Assert.NotNull(provider.GetService<IEventHandlerInvoker>());
    }

    [Fact]
    public void AddSkywalker_RegistersThreadingServices()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var cancellation = scope.ServiceProvider.GetService<ICancellationTokenProvider>();
        Assert.NotNull(cancellation);
    }

    [Fact]
    public void AddSkywalker_AutoDiscovers_IDomainService_Implementation()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var svc = scope.ServiceProvider.GetService<ITestProductDomainService>();
        Assert.NotNull(svc);
        Assert.Equal("TestProduct", svc.GetProduct());
    }

    [Fact]
    public void AddSkywalker_DoesNotRegister_DisabledDomainService()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var svc = scope.ServiceProvider.GetService<ITestDisabledDomainService>();
        Assert.Null(svc);
    }

    #endregion

    #region Application Services (ApplicationServiceFeatureProvider)

    [Fact]
    public void AddSkywalker_AutoDiscovers_IApplicationService_Implementation()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var svc = scope.ServiceProvider.GetService<ITestOrderApplicationService>();
        Assert.NotNull(svc);
        Assert.Equal("OrderPlaced", svc.PlaceOrder());
    }

    #endregion

    #region PartManager & FeatureProviders

    [Fact]
    public void AddSkywalker_DiscoversFeatureProviders()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSkywalker(typeof(FeatureProviderRegistrationTests).Assembly);

        var partManager = services.BuildServiceProvider().GetRequiredService<SkywalkerPartManager>();
        var providerTypes = partManager.FeatureProviders
            .OfType<IApplicationFeatureProvider<ServiceRegistrationFeature>>()
            .Select(p => p.GetType().Name)
            .OrderBy(n => n)
            .ToList();

        Assert.Contains("UowServiceFeatureProvider", providerTypes);
        Assert.Contains("DomainServiceFeatureProvider", providerTypes);
        Assert.Contains("ApplicationServiceFeatureProvider", providerTypes);
        Assert.Contains("EntityFrameworkCoreFeatureProvider", providerTypes);
    }

    [Fact]
    public void AddSkywalker_IsIdempotent_NoDuplicateRegistrations()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSkywalker(typeof(FeatureProviderRegistrationTests).Assembly);
        services.AddSkywalker(typeof(FeatureProviderRegistrationTests).Assembly);

        // TryAdd 语义下不会有重复
        var uowManagerCount = services.Count(d => d.ServiceType == typeof(IUnitOfWorkManager));
        Assert.Equal(1, uowManagerCount);
    }

    #endregion

    #region DomainService Lifecycle

    [Fact]
    public void AddSkywalker_DomainService_IsScoped()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        services.AddSkywalker(typeof(FeatureProviderRegistrationTests).Assembly);

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ITestProductDomainService));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        // ImplementationType 可能为 null（被 AddInterceptedServices 替换为代理工厂），检查工厂或类型均可
        Assert.True(
            descriptor.ImplementationType == typeof(TestProductDomainService) ||
            descriptor.ImplementationFactory != null,
            "Should be registered with TestProductDomainService or a proxy factory");
    }

    [Fact]
    public void AddSkywalker_DomainService_ScopedInstances_AreSameWithinScope()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var svc1 = scope.ServiceProvider.GetService<ITestProductDomainService>();
        var svc2 = scope.ServiceProvider.GetService<ITestProductDomainService>();
        Assert.NotNull(svc1);
        Assert.Same(svc1, svc2);
    }

    [Fact]
    public void AddSkywalker_DomainService_DifferentScopes_AreDifferentInstances()
    {
        using var provider = BuildProvider();
        using var scope1 = provider.CreateScope();
        using var scope2 = provider.CreateScope();
        var svc1 = scope1.ServiceProvider.GetService<ITestProductDomainService>();
        var svc2 = scope2.ServiceProvider.GetService<ITestProductDomainService>();
        Assert.NotNull(svc1);
        Assert.NotNull(svc2);
        Assert.NotSame(svc1, svc2);
    }

    #endregion

    #region Repository & Generic DomainService (AddSkywalkerDbContext)

    private static IServiceCollection BuildDbContextServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        services.AddSkywalker(typeof(FeatureProviderRegistrationTests).Assembly);
        services.AddSkywalkerDbContext<TestDbContext>(options =>
        {
            options.Configure(ctx => ctx.DbContextOptions.UseInMemoryDatabase("TestDb"));
        });
        return services;
    }

    [Fact]
    public void AddSkywalkerDbContext_RegistersRepository_ForEntityWithKey()
    {
        var services = BuildDbContextServices();

        // TestOrder : Entity<int> → IRepository<TestOrder>, IRepository<TestOrder, int> 等
        Assert.Contains(services, d => d.ServiceType == typeof(IRepository<TestOrder>));
        Assert.Contains(services, d => d.ServiceType == typeof(IRepository<TestOrder, int>));
        Assert.Contains(services, d => d.ServiceType == typeof(IBasicRepository<TestOrder>));
        Assert.Contains(services, d => d.ServiceType == typeof(IBasicRepository<TestOrder, int>));
        Assert.Contains(services, d => d.ServiceType == typeof(IReadOnlyRepository<TestOrder>));
        Assert.Contains(services, d => d.ServiceType == typeof(IReadOnlyRepository<TestOrder, int>));
    }

    [Fact]
    public void AddSkywalkerDbContext_RegistersRepository_ForEntityWithoutKey()
    {
        var services = BuildDbContextServices();

        // TestItem : Entity (无 TKey) → 只有 IRepository<TestItem> 等，没有带 TKey 的版本
        Assert.Contains(services, d => d.ServiceType == typeof(IRepository<TestItem>));
        Assert.Contains(services, d => d.ServiceType == typeof(IBasicRepository<TestItem>));
        Assert.Contains(services, d => d.ServiceType == typeof(IReadOnlyRepository<TestItem>));
    }

    [Fact]
    public void AddSkywalkerDbContext_RegistersGenericDomainService_ForEntityWithKey()
    {
        var services = BuildDbContextServices();

        // IDomainService<TestOrder> 和 IDomainService<TestOrder, int>
        Assert.Contains(services, d => d.ServiceType == typeof(IDomainService<TestOrder>));
        Assert.Contains(services, d => d.ServiceType == typeof(IDomainService<TestOrder, int>));
    }

    [Fact]
    public void AddSkywalkerDbContext_RegistersGenericDomainService_ForEntityWithoutKey()
    {
        var services = BuildDbContextServices();

        // IDomainService<TestItem> (无 TKey 版本)
        Assert.Contains(services, d => d.ServiceType == typeof(IDomainService<TestItem>));
    }

    [Fact]
    public void AddSkywalkerDbContext_Repository_IsTransient()
    {
        var services = BuildDbContextServices();

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IRepository<TestOrder, int>));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
    }

    [Fact]
    public void AddSkywalkerDbContext_GenericDomainService_IsTransient()
    {
        var services = BuildDbContextServices();

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IDomainService<TestOrder, int>));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
    }

    #endregion

    #region Replace Semantics

    [Fact]
    public void AddSkywalker_DefaultEventBus_CanBeReplaced()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSkywalker(typeof(FeatureProviderRegistrationTests).Assembly);

        // 模拟 RabbitMQ 替换：Replace 覆盖 TryAdd 的默认 IEventBus
        services.Replace(ServiceDescriptor.Singleton<IEventBus, NullEventBus>());

        using var provider = services.BuildServiceProvider();
        var eventBus = provider.GetRequiredService<IEventBus>();
        Assert.IsType<NullEventBus>(eventBus);
    }

    #endregion
}

/// <summary>
/// 用于替换测试的空 EventBus。
/// </summary>
public class NullEventBus : IEventBus
{
    public Task PublishAsync<TEvent>(TEvent eventData) where TEvent : class
        => Task.CompletedTask;

    public Task PublishAsync(Type eventType, object eventArgs)
        => Task.CompletedTask;
}
