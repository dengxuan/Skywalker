using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker;
using Skywalker.DependencyInjection;

[assembly: SkywalkerGeneratedDependencyInjectionRegistrationAttribute(
    typeof(Skywalker.Ddd.Tests.GeneratedDependencyInjectionRegistrar),
    nameof(Skywalker.Ddd.Tests.GeneratedDependencyInjectionRegistrar.AddGeneratedServices))]

namespace Skywalker.Ddd.Tests;

public sealed class GeneratedDependencyInjectionRegistrationBridgeTests
{
    [Fact]
    public void AddSkywalker_UsesGeneratedDependencyInjectionRegistration_WhenAssemblyMetadataExists()
    {
        var services = CreateServices();

        services.AddSkywalker(typeof(GeneratedDependencyInjectionRegistrationBridgeTests).Assembly);

        Assert.Contains(services, descriptor =>
            descriptor.ServiceType == typeof(IGeneratedDependencyInjectionService) &&
            descriptor.ImplementationType == typeof(GeneratedDependencyInjectionService) &&
            descriptor.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddSkywalker_DoesNotOverwriteExistingRegistration_WhenGeneratedRegistrarUsesTryAdd()
    {
        var services = CreateServices();
        var service = new ManualGeneratedDependencyInjectionService();
        services.AddSingleton<IGeneratedDependencyInjectionService>(service);

        services.AddSkywalker(typeof(GeneratedDependencyInjectionRegistrationBridgeTests).Assembly);

        var descriptor = Assert.Single(services, descriptor => descriptor.ServiceType == typeof(IGeneratedDependencyInjectionService));
        Assert.Same(service, descriptor.ImplementationInstance);
    }

    [Fact]
    public void AddSkywalker_ResolvesGeneratedDependencyInjectionRegistration()
    {
        var services = CreateServices();

        services.AddSkywalker(typeof(GeneratedDependencyInjectionRegistrationBridgeTests).Assembly);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IGeneratedDependencyInjectionService>();
        Assert.IsType<GeneratedDependencyInjectionService>(service);
    }

    [Fact]
    public void AddSkywalker_GeneratedDependencyInjectionRegistration_IsScoped()
    {
        var services = CreateServices();

        services.AddSkywalker(typeof(GeneratedDependencyInjectionRegistrationBridgeTests).Assembly);

        using var provider = services.BuildServiceProvider();
        using var firstScope = provider.CreateScope();
        using var secondScope = provider.CreateScope();

        var first = firstScope.ServiceProvider.GetRequiredService<IGeneratedDependencyInjectionService>();
        var sameScope = firstScope.ServiceProvider.GetRequiredService<IGeneratedDependencyInjectionService>();
        var second = secondScope.ServiceProvider.GetRequiredService<IGeneratedDependencyInjectionService>();

        Assert.Same(first, sameScope);
        Assert.NotSame(first, second);
    }

    [Fact]
    public void AddSkywalker_GeneratedDependencyInjectionRegistration_IsIdempotent()
    {
        var services = CreateServices();

        services.AddSkywalker(typeof(GeneratedDependencyInjectionRegistrationBridgeTests).Assembly);
        services.AddSkywalker(typeof(GeneratedDependencyInjectionRegistrationBridgeTests).Assembly);

        Assert.Single(services, descriptor => descriptor.ServiceType == typeof(IGeneratedDependencyInjectionService));
    }

    [Fact]
    public void AddSkywalker_KeepsFeatureProviderFallback_WhenGeneratedDependencyInjectionMetadataIsMissing()
    {
        var services = CreateServices();

        services.AddSkywalker(typeof(SkywalkerServiceCollectionExtensions).Assembly);

        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(SkywalkerPartManager));
    }

    private static IServiceCollection CreateServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        return services;
    }
}

public interface IGeneratedDependencyInjectionService
{
}

public sealed class GeneratedDependencyInjectionService : IGeneratedDependencyInjectionService
{
}

public sealed class ManualGeneratedDependencyInjectionService : IGeneratedDependencyInjectionService
{
}

internal static class GeneratedDependencyInjectionRegistrar
{
    public static IServiceCollection AddGeneratedServices(IServiceCollection services)
    {
        services.TryAddScoped<IGeneratedDependencyInjectionService, GeneratedDependencyInjectionService>();
        return services;
    }
}