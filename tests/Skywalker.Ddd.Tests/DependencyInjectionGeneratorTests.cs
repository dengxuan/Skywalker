using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.DynamicProxies;

namespace Skywalker.Ddd.Tests;

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
public class TestEmailService : ITestEmailService
{
    public string SendEmail(string to, string subject) => $"Sent to {to}: {subject}";
}

// Scoped service
public class TestUserRepository : ITestUserRepository
{
    public object GetUser(int id) => new { Id = id, Name = "Test" };
}

// Singleton service
public class TestCacheService : ITestCacheService
{
    public object Get(string key) => $"cached:{key}";
}

// Service with multiple interfaces
public class TestMultiService : ITestEmailService, ITestNotificationService
{
    public string SendEmail(string to, string subject) => "multi email";
    public void Notify(string message) { }
}


// Service with ReplaceService attribute
[ReplaceService]
public class TestReplacementService : ITestCacheService
{
    public object Get(string key) => $"replacement:{key}";
}

// Service with ExposeServices attribute
[ExposeServices(typeof(ITestEmailService), IncludeSelf = true)]
public class TestExposedService : ITestEmailService, ITestNotificationService
{
    public string SendEmail(string to, string subject) => "exposed";
    public void Notify(string message) { }
}

// Service without interface - should register itself
public class TestNoInterfaceService
{
    public string DoSomething() => "done";
}

#endregion

#region Test Interceptors

// Simple logging interceptor for testing
public class TestLoggingInterceptor : IInterceptor
{
    public static readonly List<string> Log = new();
    private static readonly object _lock = new();

    public async Task InterceptAsync(IMethodInvocation invocation)
    {
        lock (_lock) { Log.Add($"Before: {invocation.MethodName}"); }
        await invocation.ProceedAsync();
        lock (_lock) { Log.Add($"After: {invocation.MethodName}"); }
    }
}

// Timing interceptor for testing
public class TestTimingInterceptor : IInterceptor
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

// Intercepted service - no longer needs marker interface
public class TestOrderService : ITestOrderService
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
/// Tests for service registration and interception.
/// </summary>
public class DependencyInjectionGeneratorTests
{
    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSkywalker(typeof(DependencyInjectionGeneratorTests).Assembly);

        // 显式注册测试服务（不再使用 marker interface）
        services.TryAddTransient<ITestEmailService, TestEmailService>();
        services.TryAddScoped<ITestUserRepository, TestUserRepository>();
        services.TryAddSingleton<ITestCacheService, TestCacheService>();
        services.TryAddScoped<ITestOrderService, TestOrderService>();
        services.AddTransient<IInterceptor, TestLoggingInterceptor>();

        // 启用拦截（测试服务在 AddSkywalker 之后注册，需再次扫描）
        services.AddInterceptedServices();

        return services.BuildServiceProvider();
    }

    [Fact]
    public void ServiceRegistrar_RegistersTransientService()
    {
        using var provider = BuildProvider();
        var service1 = provider.GetService<ITestEmailService>();
        var service2 = provider.GetService<ITestEmailService>();

        Assert.NotNull(service1);
        Assert.NotNull(service2);
        Assert.NotSame(service1, service2); // Transient = new instance each time
    }

    [Fact]
    public void ServiceRegistrar_RegistersScopedService()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service1 = scope.ServiceProvider.GetService<ITestUserRepository>();
        var service2 = scope.ServiceProvider.GetService<ITestUserRepository>();

        Assert.NotNull(service1);
        Assert.NotNull(service2);
        Assert.Same(service1, service2); // Same within scope
    }

    [Fact]
    public void ServiceRegistrar_RegistersSingletonService()
    {
        using var provider = BuildProvider();
        var service1 = provider.GetService<ITestCacheService>();
        using var scope = provider.CreateScope();
        var service2 = scope.ServiceProvider.GetService<ITestCacheService>();

        Assert.NotNull(service1);
        Assert.NotNull(service2);
        Assert.Same(service1, service2); // Same instance globally
    }

    [Fact]
    public async Task Interceptor_InterceptsServiceMethods()
    {
        // Arrange — 记录当前 Log 长度，不依赖 Clear()（避免并行测试竞争）
        var logCountBefore = TestLoggingInterceptor.Log.Count;

        using var provider = BuildProvider();

        // Act - 获取代理服务并调用方法
        var service = provider.GetRequiredService<ITestOrderService>();
        var result = await service.CreateOrderAsync("Product", 5);

        // Assert - 验证拦截器被调用（检查 logCountBefore 之后新增的条目）
        Assert.Equal("Order: Product x 5", result);
        var newEntries = TestLoggingInterceptor.Log.Skip(logCountBefore).ToList();
        Assert.Contains("Before: CreateOrderAsync", newEntries);
        Assert.Contains("After: CreateOrderAsync", newEntries);
    }
}

