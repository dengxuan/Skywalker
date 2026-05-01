using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Identity.Domain.Repositories;

[assembly: SkywalkerGeneratedRepositoryRegistrationAttribute(
    typeof(Skywalker.Ddd.Tests.GeneratedRegistrationDbContext),
    typeof(Skywalker.Ddd.Tests.GeneratedRegistrationRegistrar),
    nameof(Skywalker.Ddd.Tests.GeneratedRegistrationRegistrar.AddGeneratedServices))]

namespace Skywalker.Ddd.Tests;

public sealed class GeneratedRepositoryRegistrationBridgeTests
{
    private const string DisableReflectionRepositoryFallbackSwitch = "Skywalker.Ddd.EntityFrameworkCore.DisableReflectionRepositoryFallback";

    [Fact]
    public void AddSkywalkerDbContext_UsesGeneratedRegistration_WhenAssemblyMetadataExists()
    {
        var services = CreateServices();

        services.AddSkywalkerDbContext<GeneratedRegistrationDbContext>(options =>
        {
            options.Configure(context => context.DbContextOptions.UseInMemoryDatabase("GeneratedRegistrationTestDb"));
        });

        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(GeneratedRegistrationMarker));
    }

    [Fact]
    public void AddSkywalkerDbContext_DoesNotOverwriteExistingRegistration_WhenGeneratedRegistrarUsesTryAdd()
    {
        var services = CreateServices();
        var existingMarker = new GeneratedRegistrationMarker();
        services.AddSingleton(existingMarker);

        services.AddSkywalkerDbContext<GeneratedRegistrationDbContext>(options =>
        {
            options.Configure(context => context.DbContextOptions.UseInMemoryDatabase("GeneratedRegistrationPreservesManualDb"));
        });

        var descriptor = Assert.Single(services, descriptor => descriptor.ServiceType == typeof(GeneratedRegistrationMarker));
        Assert.Same(existingMarker, descriptor.ImplementationInstance);
    }

    [Fact]
    public void AddSkywalkerDbContext_UsesGeneratedRegistration_WhenReflectionFallbackIsDisabled()
    {
        AppContext.SetSwitch(DisableReflectionRepositoryFallbackSwitch, true);
        try
        {
            var services = CreateServices();

            services.AddSkywalkerDbContext<GeneratedRegistrationDbContext>(options =>
            {
                options.Configure(context => context.DbContextOptions.UseInMemoryDatabase("GeneratedRegistrationNoFallbackDb"));
            });

            Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(GeneratedRegistrationMarker));
        }
        finally
        {
            AppContext.SetSwitch(DisableReflectionRepositoryFallbackSwitch, false);
        }
    }

    [Fact]
    public void AddSkywalkerDbContext_UsesReflectionFallback_WhenGeneratedRegistrationMetadataIsMissing()
    {
        var services = CreateServices();

        services.AddSkywalkerDbContext<FallbackRegistrationDbContext>(options =>
        {
            options.Configure(context => context.DbContextOptions.UseInMemoryDatabase("ReflectionFallbackDb"));
        });

        Assert.Contains(services, descriptor =>
            descriptor.ServiceType == typeof(IRepository<FallbackRegistrationEntity, int>) &&
            descriptor.ImplementationType == typeof(Repository<FallbackRegistrationDbContext, FallbackRegistrationEntity, int>));
        Assert.Contains(services, descriptor =>
            descriptor.ServiceType == typeof(IRepository<FallbackRegistrationEntity>) &&
            descriptor.ImplementationType == typeof(Repository<FallbackRegistrationDbContext, FallbackRegistrationEntity, int>));
        Assert.Contains(services, descriptor =>
            descriptor.ServiceType == typeof(Skywalker.Ddd.Domain.Services.IDomainService<FallbackRegistrationEntity, int>) &&
            descriptor.ImplementationType == typeof(EntityFrameworkCoreDomainService<FallbackRegistrationEntity, int>));
    }

    [Fact]
    public void AddSkywalkerDbContext_Throws_WhenGeneratedRegistrationMetadataIsMissingAndReflectionFallbackIsDisabled()
    {
        AppContext.SetSwitch(DisableReflectionRepositoryFallbackSwitch, true);
        try
        {
            var services = CreateServices();

            var exception = Assert.Throws<InvalidOperationException>(() => services.AddSkywalkerDbContext<FallbackRegistrationDbContext>(options =>
            {
                options.Configure(context => context.DbContextOptions.UseInMemoryDatabase("ReflectionFallbackDisabledDb"));
            }));

            Assert.Contains("Reflection fallback is disabled", exception.Message);
            Assert.DoesNotContain(services, descriptor => descriptor.ServiceType == typeof(IRepository<FallbackRegistrationEntity, int>));
        }
        finally
        {
            AppContext.SetSwitch(DisableReflectionRepositoryFallbackSwitch, false);
        }
    }

    private static IServiceCollection CreateServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        services.AddSkywalker(typeof(GeneratedRepositoryRegistrationBridgeTests).Assembly);
        return services;
    }
}

public sealed class GeneratedRegistrationDbContext(DbContextOptions<GeneratedRegistrationDbContext> options) : SkywalkerDbContext<GeneratedRegistrationDbContext>(options)
{
    public DbSet<GeneratedRegistrationEntity> Entities { get; set; } = default!;
}

public sealed class GeneratedRegistrationEntity : Entity<int>
{
}

public sealed class FallbackRegistrationDbContext(DbContextOptions<FallbackRegistrationDbContext> options) : SkywalkerDbContext<FallbackRegistrationDbContext>(options)
{
    public DbSet<FallbackRegistrationEntity> Entities { get; set; } = default!;
}

public sealed class FallbackRegistrationEntity : Entity<int>
{
}

public sealed class GeneratedRegistrationMarker
{
}

internal static class GeneratedRegistrationRegistrar
{
    public static IServiceCollection AddGeneratedServices(IServiceCollection services)
    {
        services.TryAddTransient<GeneratedRegistrationMarker>();
        return services;
    }
}