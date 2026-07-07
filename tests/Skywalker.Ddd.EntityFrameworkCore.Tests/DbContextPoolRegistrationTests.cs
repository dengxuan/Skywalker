using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.Domain.Services;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.EventBus.Abstractions;
using Skywalker.Extensions.Timezone;

namespace Skywalker.Ddd.EntityFrameworkCore.Tests;

/// <summary>
/// 池化注册测试用 DbContext
/// </summary>
public class PooledTestDbContext(DbContextOptions<PooledTestDbContext> options) : SkywalkerDbContext<PooledTestDbContext>(options)
{
    public DbSet<TestEntity> TestEntities => Set<TestEntity>();
}

public class DbContextPoolRegistrationTests
{
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Substitute.For<IClock>());
        services.AddSingleton(Substitute.For<IEventBus>());
        services.AddSingleton(Substitute.For<IUnitOfWorkAccessor>());
        services.AddSingleton(Substitute.For<IUnitOfWorkManager>());
        services.AddSingleton(Substitute.For<IConnectionStringResolver>());

        services.AddSkywalkerDbContextPool<PooledTestDbContext>(options =>
        {
            options.UseInMemoryDatabase("pooled-registration");
        });

        return services.BuildServiceProvider();
    }

    [Fact]
    public void AddSkywalkerDbContextPool_ShouldResolveRepositories()
    {
        using var provider = BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IRepository<TestEntity, Guid>>());
        Assert.NotNull(provider.GetRequiredService<IRepository<TestEntity>>());
        Assert.NotNull(provider.GetRequiredService<IBasicRepository<TestEntity, Guid>>());
        Assert.NotNull(provider.GetRequiredService<IReadOnlyRepository<TestEntity, Guid>>());
    }

    [Fact]
    public void AddSkywalkerDbContextPool_ShouldRegisterDomainServices()
    {
        var services = new ServiceCollection();

        services.AddSkywalkerDbContextPool<PooledTestDbContext>(options =>
        {
            options.UseInMemoryDatabase("pooled-domain-services");
        });

        Assert.Contains(services, d => d.ServiceType == typeof(IDomainService<TestEntity, Guid>));
        Assert.Contains(services, d => d.ServiceType == typeof(IDomainService<TestEntity>));
    }

    [Fact]
    public void AddSkywalkerDbContextPool_ShouldResolvePooledDbContext()
    {
        using var provider = BuildServiceProvider();
        using var scope = provider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<PooledTestDbContext>();

        Assert.NotNull(dbContext);
        Assert.NotNull(dbContext.TestEntities);
    }

    [Fact]
    public void AddSkywalkerDbContextPool_ShouldHonorOptionsAction()
    {
        using var provider = BuildServiceProvider();
        using var scope = provider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<PooledTestDbContext>();

        Assert.True(dbContext.Database.IsInMemory());
    }
}
