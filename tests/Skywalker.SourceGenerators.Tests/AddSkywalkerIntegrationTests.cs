using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.SourceGenerators.Tests;

/// <summary>
/// Integration tests for the generated AddAutoServices() method.
/// AddAutoServices() registers services defined in this test project.
/// </summary>
public class AddAutoServicesIntegrationTests
{
    [Fact]
    public void AddAutoServices_RegistersTestProjectServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act - AddAutoServices registers local services
        services.AddAutoServices();
        using var provider = services.BuildServiceProvider();

        // Assert - verify services defined in this test project are registered
        var emailService = provider.GetService<ITestEmailService>();
        Assert.NotNull(emailService);

        var userRepository = provider.GetService<ITestUserRepository>();
        Assert.NotNull(userRepository);

        var cacheService = provider.GetService<ITestCacheService>();
        Assert.NotNull(cacheService);
    }

    [Fact]
    public void AddAutoServices_RegistersServicesWithCorrectLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoServices();

        // Assert - check service descriptors
        var emailDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ITestEmailService));
        Assert.NotNull(emailDescriptor);
        Assert.Equal(ServiceLifetime.Transient, emailDescriptor.Lifetime);

        var userRepoDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ITestUserRepository));
        Assert.NotNull(userRepoDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, userRepoDescriptor.Lifetime);

        var cacheDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ITestCacheService));
        Assert.NotNull(cacheDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, cacheDescriptor.Lifetime);
    }

    [Fact]
    public void AddAutoServices_ServicesAreFunctional()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoServices();
        using var provider = services.BuildServiceProvider();

        // Act & Assert - verify services actually work
        var emailService = provider.GetRequiredService<ITestEmailService>();
        var result = emailService.SendEmail("test@example.com", "Hello");
        Assert.Contains("test@example.com", result);
    }

    [Fact]
    public void AddAutoServices_CanBeCalledMultipleTimes()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act - calling multiple times should not throw
        services.AddAutoServices();
        services.AddAutoServices();

        using var provider = services.BuildServiceProvider();

        // Assert - services should still work
        var emailService = provider.GetService<ITestEmailService>();
        Assert.NotNull(emailService);
    }

    [Fact]
    public void AddAutoServices_ReturnsServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddAutoServices();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddAutoServices_DoesNotRegisterDisabledServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoServices();

        // Assert - TestDisabledService should NOT be registered
        var disabledDescriptor = services.FirstOrDefault(d =>
            d.ImplementationType == typeof(TestDisabledService));
        Assert.Null(disabledDescriptor);
    }

    [Fact]
    public void AddAutoServices_RegistersServiceWithoutInterface()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoServices();
        using var provider = services.BuildServiceProvider();

        // Assert - TestNoInterfaceService should be registered as itself
        var noInterfaceService = provider.GetService<TestNoInterfaceService>();
        Assert.NotNull(noInterfaceService);
    }

}

