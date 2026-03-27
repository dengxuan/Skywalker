using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.DependencyInjection;
using Skywalker.Extensions.DynamicProxies;

namespace Skywalker.SourceGenerators.Tests;

#region Test Service Interfaces and Implementations

public interface ITestEmailService
{
    string SendEmail(string to, string subject);
}

public interface ITestUserRepository
{
    object GetUser(int id);
}

public interface ITestCacheService
{
    object Get(string key);
}

public interface ITestNotificationService
{
    void Notify(string message);
}

// Transient service
public class TestEmailService : ITestEmailService, ITransientDependency
{
    public string SendEmail(string to, string subject) => $"Sent to {to}: {subject}";
}

// Scoped service
public class TestUserRepository : ITestUserRepository, IScopedDependency
{
    public object GetUser(int id) => new { Id = id, Name = "Test" };
}

// Singleton service
public class TestCacheService : ITestCacheService, ISingletonDependency
{
    public object Get(string key) => $"cached:{key}";
}

// Service with multiple interfaces
public class TestMultiService : ITestEmailService, ITestNotificationService, ITransientDependency
{
    public string SendEmail(string to, string subject) => "multi email";
    public void Notify(string message) { }
}

// Service that should NOT be registered
[DisableAutoRegistration]
public class TestDisabledService : ITestEmailService, ITransientDependency
{
    public string SendEmail(string to, string subject) => "disabled";
}

// Service with ReplaceService attribute
[ReplaceService]
public class TestReplacementService : ITestCacheService, ISingletonDependency
{
    public object Get(string key) => $"replacement:{key}";
}

// Service with ExposeServices attribute
[ExposeServices(typeof(ITestEmailService), IncludeSelf = true)]
public class TestExposedService : ITestEmailService, ITestNotificationService, IScopedDependency
{
    public string SendEmail(string to, string subject) => "exposed";
    public void Notify(string message) { }
}

// Service without interface - should register itself
public class TestNoInterfaceService : ITransientDependency
{
    public string DoSomething() => "done";
}

#endregion

#region Test Interceptors

// Simple logging interceptor for testing
public class TestLoggingInterceptor : IInterceptor, ITransientDependency
{
    public static readonly List<string> Log = new();

    public async Task InterceptAsync(IMethodInvocation invocation)
    {
        Log.Add($"Before: {invocation.MethodName}");
        await invocation.ProceedAsync();
        Log.Add($"After: {invocation.MethodName}");
    }
}

// Timing interceptor for testing
public class TestTimingInterceptor : IInterceptor, ITransientDependency
{
    public static DateTime? StartTime { get; private set; }
    public static DateTime? EndTime { get; private set; }

    public async Task InterceptAsync(IMethodInvocation invocation)
    {
        StartTime = DateTime.UtcNow;
        await invocation.ProceedAsync();
        EndTime = DateTime.UtcNow;
    }
}

// Service interface for intercepted service - inherits IInterceptable to enable proxy generation
public interface ITestOrderService : IInterceptable
{
    Task<string> CreateOrderAsync(string product, int quantity);
    string GetOrderStatus(string orderId);
}

// Intercepted service - no longer needs [Intercept] attribute
public class TestOrderService : ITestOrderService, IScopedDependency
{
    public Task<string> CreateOrderAsync(string product, int quantity)
    {
        return Task.FromResult($"Order: {product} x {quantity}");
    }

    public string GetOrderStatus(string orderId)
    {
        return $"Status: {orderId}";
    }
}

#endregion

/// <summary>
/// Tests for the DependencyInjectionGenerator Source Generator.
/// </summary>
public class DependencyInjectionGeneratorTests
{
    private const string GeneratedClassName = "SkywalkerSourceGeneratorsTestsAutoServiceExtensions";

    [Fact]
    public void Generator_GeneratesAddAutoServicesMethod()
    {
        // The fact that this test compiles proves the generator works
        var assembly = typeof(DependencyInjectionGeneratorTests).Assembly;

        // Find the generated class
        var extensionClass = assembly.GetTypes()
            .FirstOrDefault(t => t.Name == GeneratedClassName);

        Assert.NotNull(extensionClass);

        // AddAutoServices is internal (非公开)
        var methodInfo = extensionClass.GetMethod("AddAutoServices", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(methodInfo);
        Assert.True(methodInfo.IsStatic);
        Assert.True(methodInfo.IsAssembly); // internal 方法
    }

    [Fact]
    public void Generator_RegistersTransientService()
    {
        // Arrange
        var services = new ServiceCollection();
        AddAutoServices(services);
        var provider = services.BuildServiceProvider();

        // Act
        var service1 = provider.GetService<ITestEmailService>();
        var service2 = provider.GetService<ITestEmailService>();

        // Assert
        Assert.NotNull(service1);
        Assert.NotNull(service2);
        Assert.NotSame(service1, service2); // Transient = new instance each time
    }

    [Fact]
    public void Generator_RegistersScopedService()
    {
        // Arrange
        var services = new ServiceCollection();
        AddAutoServices(services);
        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var service1 = scope.ServiceProvider.GetService<ITestUserRepository>();
        var service2 = scope.ServiceProvider.GetService<ITestUserRepository>();

        // Assert
        Assert.NotNull(service1);
        Assert.NotNull(service2);
        Assert.Same(service1, service2); // Same within scope
    }

    [Fact]
    public void Generator_RegistersSingletonService()
    {
        // Arrange
        var services = new ServiceCollection();
        AddAutoServices(services);
        var provider = services.BuildServiceProvider();

        // Act
        var service1 = provider.GetService<ITestCacheService>();
        using var scope = provider.CreateScope();
        var service2 = scope.ServiceProvider.GetService<ITestCacheService>();

        // Assert
        Assert.NotNull(service1);
        Assert.NotNull(service2);
        Assert.Same(service1, service2); // Same instance globally
    }

    [Fact]
    public async Task Interceptor_InterceptsServiceMethods()
    {
        // Arrange
        TestLoggingInterceptor.Log.Clear();

        var services = new ServiceCollection();

        // AddAutoServices 自动注册所有 DI 服务 + 调用 AddProxyServices 注册代理
        AddAutoServices(services);

        var provider = services.BuildServiceProvider();

        // Act - 获取代理服务并调用方法
        var service = provider.GetRequiredService<ITestOrderService>();
        var result = await service.CreateOrderAsync("Product", 5);

        // Assert - 验证拦截器被调用
        Assert.Equal("Order: Product x 5", result);
        Assert.Contains("Before: CreateOrderAsync", TestLoggingInterceptor.Log);
        Assert.Contains("After: CreateOrderAsync", TestLoggingInterceptor.Log);
    }

    private static void AddAutoServices(IServiceCollection services)
    {
        // Use reflection to call the generated method (internal 方法)
        var assembly = typeof(DependencyInjectionGeneratorTests).Assembly;
        var extensionClass = assembly.GetTypes()
            .First(t => t.Name == GeneratedClassName);
        var method = extensionClass.GetMethod("AddAutoServices", BindingFlags.NonPublic | BindingFlags.Static)!;
        method.Invoke(null, new object[] { services });
    }
}

