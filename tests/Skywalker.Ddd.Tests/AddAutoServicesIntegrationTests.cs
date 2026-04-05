using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Skywalker.Ddd.Tests;

/// <summary>
/// Integration tests for service registration via AddSkywalker() and explicit registration.
/// </summary>
public class AddAutoServicesIntegrationTests
{
    [Fact]
    public void AddSkywalker_RegistersTestProjectServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSkywalker(typeof(AddAutoServicesIntegrationTests).Assembly);

        // 显式注册测试服务（不再使用 marker interface）
        services.TryAddTransient<ITestEmailService, TestEmailService>();
        services.TryAddScoped<ITestUserRepository, TestUserRepository>();
        services.TryAddSingleton<ITestCacheService, TestCacheService>();

        using var provider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(provider.GetService<ITestEmailService>());
        Assert.NotNull(provider.GetService<ITestUserRepository>());
        Assert.NotNull(provider.GetService<ITestCacheService>());
    }

    [Fact]
    public void AddSkywalker_RegistersServicesWithCorrectLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSkywalker(typeof(AddAutoServicesIntegrationTests).Assembly);

        // 显式注册
        services.TryAddTransient<ITestEmailService, TestEmailService>();
        services.TryAddScoped<ITestUserRepository, TestUserRepository>();
        services.TryAddSingleton<ITestCacheService, TestCacheService>();

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
    public void AddSkywalker_ServicesAreFunctional()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSkywalker(typeof(AddAutoServicesIntegrationTests).Assembly);
        services.TryAddTransient<ITestEmailService, TestEmailService>();

        using var provider = services.BuildServiceProvider();

        // Act & Assert
        var emailService = provider.GetRequiredService<ITestEmailService>();
        var result = emailService.SendEmail("test@example.com", "Hello");
        Assert.Contains("test@example.com", result);
    }

    [Fact]
    public void AddSkywalker_CreatesPartManagerWithParts()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var builder = services.AddSkywalker(typeof(AddAutoServicesIntegrationTests).Assembly);

        // Assert - PartManager should discover assemblies
        Assert.NotEmpty(builder.PartManager.ApplicationParts);
    }
}

