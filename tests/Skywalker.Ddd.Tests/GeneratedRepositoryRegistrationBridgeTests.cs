using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.EntityFrameworkCore;

[assembly: SkywalkerGeneratedRepositoryRegistrationAttribute(
    typeof(Skywalker.Ddd.Tests.GeneratedRegistrationDbContext),
    typeof(Skywalker.Ddd.Tests.GeneratedRegistrationRegistrar),
    nameof(Skywalker.Ddd.Tests.GeneratedRegistrationRegistrar.AddGeneratedServices))]

namespace Skywalker.Ddd.Tests;

public sealed class GeneratedRepositoryRegistrationBridgeTests
{
    [Fact]
    public void AddSkywalkerDbContext_UsesGeneratedRegistration_WhenAssemblyMetadataExists()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        services.AddSkywalker(typeof(GeneratedRepositoryRegistrationBridgeTests).Assembly);

        services.AddSkywalkerDbContext<GeneratedRegistrationDbContext>(options =>
        {
            options.Configure(context => context.DbContextOptions.UseInMemoryDatabase("GeneratedRegistrationTestDb"));
        });

        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(GeneratedRegistrationMarker));
    }
}

public sealed class GeneratedRegistrationDbContext(DbContextOptions<GeneratedRegistrationDbContext> options) : SkywalkerDbContext<GeneratedRegistrationDbContext>(options)
{
    public DbSet<GeneratedRegistrationEntity> Entities { get; set; } = default!;
}

public sealed class GeneratedRegistrationEntity : Entity<int>
{
}

public sealed class GeneratedRegistrationMarker
{
}

internal static class GeneratedRegistrationRegistrar
{
    public static IServiceCollection AddGeneratedServices(IServiceCollection services)
    {
        services.AddTransient<GeneratedRegistrationMarker>();
        return services;
    }
}