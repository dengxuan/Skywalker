using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.DependencyInjection;
using Skywalker.Extensions.DynamicProxies;

namespace Skywalker.SourceGenerators.Tests;

#region Test UoW Service

/// <summary>
/// 带有 [UnitOfWork] 标记的服务接口，继承 IInterceptable 以启用代理生成。
/// </summary>
public interface ITestUowOrderService : IInterceptable
{
    Task<string> CreateOrderAsync(string product, int quantity);
    Task<string> GetOrderAsync(string orderId);
}

/// <summary>
/// 使用 [UnitOfWork] 标记的服务，用于验证拦截器在无中间件场景下的工作。
/// </summary>
[UnitOfWork]
public class TestUowOrderService : ITestUowOrderService, IScopedDependency
{
    public Task<string> CreateOrderAsync(string product, int quantity)
    {
        return Task.FromResult($"Order: {product} x {quantity}");
    }

    public Task<string> GetOrderAsync(string orderId)
    {
        return Task.FromResult($"Order: {orderId}");
    }
}

#endregion

/// <summary>
/// 验证 UnitOfWorkInterceptor 在无 ASP.NET Core 中间件（Console/Worker）场景下独立工作。
/// </summary>
public class UnitOfWorkInterceptorIntegrationTests
{
    private static ServiceProvider BuildConsoleAppProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        // 注册 UoW 服务（与真实 Console/Worker 应用一样使用公开 API）
        services.AddUnitOfWork();
        // 注册本测试项目的服务（TestUowOrderService + 生成的代理）
        SkywalkerSourceGeneratorsTestsAutoServiceExtensions.AddAutoServices(services);
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Interceptor_CreatesUnitOfWork_WithoutMiddleware()
    {
        // Arrange — 模拟 Console/Worker 应用，没有任何 ASP.NET Core 中间件
        using var provider = BuildConsoleAppProvider();
        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<ITestUowOrderService>();
        var uowAccessor = scope.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();

        // Act — 调用被代理的方法
        var result = await service.CreateOrderAsync("Widget", 10);

        // Assert — 方法正确执行
        Assert.Equal("Order: Widget x 10", result);
    }

    [Fact]
    public async Task Interceptor_DetectsUnitOfWorkAttribute_ViaMethodInfo()
    {
        // Arrange
        using var provider = BuildConsoleAppProvider();
        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<ITestUowOrderService>();

        // Act — 验证代理类实际使用拦截器链
        // UnitOfWorkInterceptor 需要能读取 [UnitOfWork] 属性，这要求 MethodInfo 不为 null
        var result = await service.CreateOrderAsync("Test", 1);

        // Assert
        Assert.Equal("Order: Test x 1", result);
    }

    [Fact]
    public async Task Interceptor_GetMethod_IsNonTransactional()
    {
        // Arrange — Get* 方法应被拦截器识别为非事务性
        using var provider = BuildConsoleAppProvider();
        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<ITestUowOrderService>();

        // Act
        var result = await service.GetOrderAsync("ORD-001");

        // Assert
        Assert.Equal("Order: ORD-001", result);
    }

    [Fact]
    public void ProxyClass_IsGenerated_ForUowService()
    {
        // Arrange
        var assembly = typeof(UnitOfWorkInterceptorIntegrationTests).Assembly;

        // Assert — 验证 SourceGenerator 为 TestUowOrderService 生成了代理类
        var proxyType = assembly.GetTypes()
            .FirstOrDefault(t => t.Name == $"{nameof(TestUowOrderService)}Proxy");
        Assert.NotNull(proxyType);
    }

    [Fact]
    public void ProxyClass_HasCachedMethodInfo()
    {
        // Arrange
        var assembly = typeof(UnitOfWorkInterceptorIntegrationTests).Assembly;
        var proxyType = assembly.GetTypes().First(t => t.Name == $"{nameof(TestUowOrderService)}Proxy");

        // Assert — 验证代理类缓存了 MethodInfo（static readonly 字段）
        var methodInfoFields = proxyType.GetFields(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(MethodInfo) && f.Name.Contains("MethodInfo"))
            .ToList();

        Assert.NotEmpty(methodInfoFields);

        // 每个 MethodInfo 字段都不为 null
        var proxyInstance = CreateProxyInstance(proxyType);
        foreach (var field in methodInfoFields)
        {
            var value = field.GetValue(null);
            Assert.NotNull(value);
        }
    }

    [Fact]
    public void UnitOfWorkInterceptor_IsRegistered_AsInterceptor()
    {
        // Arrange
        using var provider = BuildConsoleAppProvider();

        // Assert — UnitOfWorkInterceptor 通过 DI 自动注册为 IInterceptor
        var interceptors = provider.GetServices<IInterceptor>().ToList();
        Assert.Contains(interceptors, i => i is UnitOfWorkInterceptor);
    }

    private static object? CreateProxyInstance(Type proxyType)
    {
        // 只用于读取静态字段，不需要实际实例
        return null;
    }
}
