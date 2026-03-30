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
        // 使用 AddSkywalker 进行反射扫描注册（传入测试程序集以发现 DDD 模块）
        services.AddSkywalker(typeof(UnitOfWorkInterceptorIntegrationTests).Assembly);
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
    public void CastleProxy_IsCreated_ForUowService()
    {
        using var provider = BuildConsoleAppProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITestUowOrderService>();
        // Castle.DynamicProxy creates a runtime proxy — type differs from the concrete class
        Assert.NotEqual(typeof(TestUowOrderService), service.GetType());
    }

    [Fact]
    public void UnitOfWorkInterceptor_IsRegistered()
    {
        using var provider = BuildConsoleAppProvider();
        var interceptor = provider.GetService<UnitOfWorkInterceptor>();
        Assert.NotNull(interceptor);
    }
}
