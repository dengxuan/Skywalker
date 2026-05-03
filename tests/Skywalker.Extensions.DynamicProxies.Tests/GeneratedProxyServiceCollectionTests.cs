using Microsoft.Extensions.DependencyInjection;
using Skywalker.Extensions.DynamicProxies;

namespace Skywalker.Extensions.DynamicProxies.Tests;

public sealed class GeneratedProxyServiceCollectionTests
{
    [Fact]
    public async Task AddInterceptedServices_UsesGeneratedProxy_WhenMetadataExists()
    {
        var order = new List<string>();
        var services = new ServiceCollection();
        services.AddSingleton(order);
        services.AddTransient<IInterceptor, RecordingInterceptor>();
        services.AddScoped<IGeneratedOrderService, GeneratedOrderService>();

        services.AddInterceptedServices();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IGeneratedOrderService>();

        var result = await service.SubmitAsync("A-100");

        Assert.Equal("submitted:A-100", result);
        Assert.NotEqual(typeof(GeneratedOrderService), service.GetType());
        Assert.Contains("SkywalkerProxy", service.GetType().Name);
        Assert.Equal(new[] { "before:SubmitAsync:A-100", "after:SubmitAsync:submitted:A-100" }, order);
    }

    [Fact]
    public void AddInterceptedServices_DoesNotProxy_NonInterceptableService()
    {
        var services = new ServiceCollection();
        services.AddTransient<IPlainGeneratedService, PlainGeneratedService>();

        services.AddInterceptedServices();

        using var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IPlainGeneratedService>();

        Assert.IsType<PlainGeneratedService>(service);
        Assert.Equal("plain", service.GetName());
    }

    public interface IGeneratedOrderService : IInterceptable
    {
        Task<string> SubmitAsync(string number);
    }

    public sealed class GeneratedOrderService : IGeneratedOrderService
    {
        public Task<string> SubmitAsync(string number)
        {
            return Task.FromResult($"submitted:{number}");
        }
    }

    public interface IPlainGeneratedService
    {
        string GetName();
    }

    public sealed class PlainGeneratedService : IPlainGeneratedService
    {
        public string GetName() => "plain";
    }

    private sealed class RecordingInterceptor : IInterceptor
    {
        private readonly List<string> _order;

        public RecordingInterceptor(List<string> order)
        {
            _order = order;
        }

        public async Task InterceptAsync(IMethodInvocation invocation)
        {
            _order.Add($"before:{invocation.MethodName}:{invocation.Arguments[0]}");
            await invocation.ProceedAsync();
            _order.Add($"after:{invocation.MethodName}:{invocation.ReturnValue}");
        }
    }
}