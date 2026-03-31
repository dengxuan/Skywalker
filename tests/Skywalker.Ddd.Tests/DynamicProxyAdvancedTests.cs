using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.DynamicProxies;

namespace Skywalker.Ddd.Tests;

#region Complex Service Interfaces and Implementations

/// <summary>
/// 包含各种复杂参数类型的服务接口：数组、泛型、nullable、多重泛型、重载。
/// </summary>
public interface IComplexParamService : IInterceptable
{
    // 数组参数
    Task<int> ProcessArrayAsync(string[] items);
    Task<int> ProcessIntArrayAsync(int[] values);

    // 多维/嵌套数组
    Task<string> ProcessNestedArrayAsync(string[][] matrix);

    // 泛型集合参数
    Task<string> ProcessListAsync(List<string> items);
    Task<int> ProcessDictionaryAsync(Dictionary<string, int> map);

    // nullable 参数
    Task<string> HandleNullableAsync(string? name, int? count);

    // 混合复杂参数
    Task<string> MixedComplexAsync(string[] tags, Dictionary<string, List<int>> data, bool? flag);

    // 无参数方法
    Task<string> NoParamsAsync();

    // 同步方法
    int SyncMethod(string input);

    // 同名方法重载
    Task<string> OverloadedAsync(string value);
    Task<string> OverloadedAsync(int value);
    Task<string> OverloadedAsync(string key, string[] values);
}

/// <summary>
/// 包含泛型方法的服务接口。
/// </summary>
public interface IGenericMethodService : IInterceptable
{
    Task<T> GetAsync<T>(string key) where T : class;
    Task<TResult> TransformAsync<TInput, TResult>(TInput input) where TInput : class where TResult : class;
    Task<List<T>> GetListAsync<T>(string[] keys) where T : class;
}

public class ComplexParamService : IComplexParamService
{
    public Task<int> ProcessArrayAsync(string[] items) => Task.FromResult(items.Length);
    public Task<int> ProcessIntArrayAsync(int[] values) => Task.FromResult(values.Sum());
    public Task<string> ProcessNestedArrayAsync(string[][] matrix) => Task.FromResult($"rows:{matrix.Length}");
    public Task<string> ProcessListAsync(List<string> items) => Task.FromResult(string.Join(",", items));
    public Task<int> ProcessDictionaryAsync(Dictionary<string, int> map) => Task.FromResult(map.Count);
    public Task<string> HandleNullableAsync(string? name, int? count) => Task.FromResult($"{name ?? "null"}:{count?.ToString() ?? "null"}");
    public Task<string> MixedComplexAsync(string[] tags, Dictionary<string, List<int>> data, bool? flag)
        => Task.FromResult($"tags:{tags.Length},data:{data.Count},flag:{flag}");
    public Task<string> NoParamsAsync() => Task.FromResult("ok");
    public int SyncMethod(string input) => input.Length;
    public Task<string> OverloadedAsync(string value) => Task.FromResult($"str:{value}");
    public Task<string> OverloadedAsync(int value) => Task.FromResult($"int:{value}");
    public Task<string> OverloadedAsync(string key, string[] values) => Task.FromResult($"{key}:[{string.Join(",", values)}]");
}

public class GenericMethodService : IGenericMethodService
{
    public Task<T> GetAsync<T>(string key) where T : class => Task.FromResult(default(T)!);
    public Task<TResult> TransformAsync<TInput, TResult>(TInput input) where TInput : class where TResult : class
        => Task.FromResult(default(TResult)!);
    public Task<List<T>> GetListAsync<T>(string[] keys) where T : class
        => Task.FromResult(new List<T>());
}

#endregion

/// <summary>
/// 测试 DynamicProxies SourceGenerator 对复杂参数类型的处理。
/// </summary>
public class DynamicProxyAdvancedTests
{
    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddSkywalker(typeof(DynamicProxyAdvancedTests).Assembly);

        // 显式注册测试服务
        services.TryAddScoped<IComplexParamService, ComplexParamService>();
        services.TryAddScoped<IGenericMethodService, GenericMethodService>();

        // 启用拦截
        services.AddInterceptedServices();

        return services.BuildServiceProvider();
    }

    #region Proxy Verification (Castle.DynamicProxy)

    [Fact]
    public void CastleProxy_Created_ForComplexParamService()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IComplexParamService>();
        // Castle.DynamicProxy creates runtime proxy types
        Assert.NotEqual(typeof(ComplexParamService), service.GetType());
    }

    [Fact]
    public void CastleProxy_Created_ForGenericMethodService()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IGenericMethodService>();
        Assert.NotEqual(typeof(GenericMethodService), service.GetType());
    }

    #endregion

    #region Array Parameter Tests

    [Fact]
    public async Task Proxy_ArrayParam_StringArray()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IComplexParamService>();

        var result = await service.ProcessArrayAsync(new[] { "a", "b", "c" });
        Assert.Equal(3, result);
    }

    [Fact]
    public async Task Proxy_ArrayParam_IntArray()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IComplexParamService>();

        var result = await service.ProcessIntArrayAsync(new[] { 10, 20, 30 });
        Assert.Equal(60, result);
    }

    [Fact]
    public async Task Proxy_ArrayParam_NestedArray()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IComplexParamService>();

        var result = await service.ProcessNestedArrayAsync(new[] { new[] { "a" }, new[] { "b", "c" } });
        Assert.Equal("rows:2", result);
    }

    #endregion

    #region Generic Collection Parameter Tests

    [Fact]
    public async Task Proxy_GenericParam_List()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IComplexParamService>();

        var result = await service.ProcessListAsync(new List<string> { "x", "y" });
        Assert.Equal("x,y", result);
    }

    [Fact]
    public async Task Proxy_GenericParam_Dictionary()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IComplexParamService>();

        var result = await service.ProcessDictionaryAsync(new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 });
        Assert.Equal(2, result);
    }

    #endregion

    #region Nullable Parameter Tests

    [Fact]
    public async Task Proxy_NullableParams_WithValues()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IComplexParamService>();

        var result = await service.HandleNullableAsync("test", 42);
        Assert.Equal("test:42", result);
    }

    [Fact]
    public async Task Proxy_NullableParams_WithNulls()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IComplexParamService>();

        var result = await service.HandleNullableAsync(null, null);
        Assert.Equal("null:null", result);
    }

    #endregion

    #region Mixed Complex Parameter Tests

    [Fact]
    public async Task Proxy_MixedComplexParams()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IComplexParamService>();

        var result = await service.MixedComplexAsync(
            new[] { "tag1" },
            new Dictionary<string, List<int>> { ["k"] = new List<int> { 1 } },
            true);
        Assert.Equal("tags:1,data:1,flag:True", result);
    }

    #endregion

    #region Sync Method Tests

    [Fact]
    public void Proxy_SyncMethod_Works()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IComplexParamService>();

        var result = service.SyncMethod("hello");
        Assert.Equal(5, result);
    }

    #endregion

    #region Method Overload Tests

    [Fact]
    public async Task Proxy_Overload_StringParam()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IComplexParamService>();

        var result = await service.OverloadedAsync("test");
        Assert.Equal("str:test", result);
    }

    [Fact]
    public async Task Proxy_Overload_IntParam()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IComplexParamService>();

        var result = await service.OverloadedAsync(42);
        Assert.Equal("int:42", result);
    }

    [Fact]
    public async Task Proxy_Overload_StringAndArrayParam()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IComplexParamService>();

        var result = await service.OverloadedAsync("key", new[] { "a", "b" });
        Assert.Equal("key:[a,b]", result);
    }

    #endregion

    #region No-Param Method Tests

    [Fact]
    public async Task Proxy_NoParams_Works()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IComplexParamService>();

        var result = await service.NoParamsAsync();
        Assert.Equal("ok", result);
    }

    #endregion

    #region Generic Method Tests

    [Fact]
    public async Task Proxy_GenericMethod_SingleTypeParam()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IGenericMethodService>();

        // GetAsync<T> — 泛型方法应当正确解析并缓存 MethodInfo
        // 验证代理方法可以正常调用而不抛异常（default(string)! 为 null）
        var exception = await Record.ExceptionAsync(() => service.GetAsync<string>("key"));
        Assert.Null(exception);
    }

    [Fact]
    public async Task Proxy_GenericMethod_MultipleTypeParams()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IGenericMethodService>();

        // TransformAsync<TInput, TResult> — 两个类型参数，验证不抛异常
        var exception = await Record.ExceptionAsync(() => service.TransformAsync<string, string>("input"));
        Assert.Null(exception);
    }

    [Fact]
    public async Task Proxy_GenericMethod_WithArrayParam()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IGenericMethodService>();

        // GetListAsync<T>(string[] keys) — 泛型方法 + 数组参数
        var result = await service.GetListAsync<string>(new[] { "a", "b" });
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region Pipeline Caching Tests

    [Fact]
    public async Task Pipeline_IsCached_MultipleCallsSameMethod()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IComplexParamService>();

        // 多次调用同一方法，每次参数不同——验证管道缓存不影响参数传递
        var r1 = await service.ProcessArrayAsync(new[] { "a" });
        var r2 = await service.ProcessArrayAsync(new[] { "b", "c" });
        var r3 = await service.ProcessArrayAsync(new[] { "d", "e", "f" });

        Assert.Equal(1, r1);
        Assert.Equal(2, r2);
        Assert.Equal(3, r3);
    }

    #endregion

    #region Concurrent Safety Tests

    [Fact]
    public async Task Proxy_ConcurrentCalls_AreThreadSafe()
    {
        using var provider = BuildProvider();

        // 并发调用代理方法，验证结果正确性（而非静态计数器）
        var results = new ConcurrentBag<int>();
        var tasks = Enumerable.Range(0, 50).Select(async i =>
        {
            using var scope = provider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IComplexParamService>();
            var result = await service.ProcessArrayAsync(new[] { $"item{i}" });
            results.Add(result);
        });

        await Task.WhenAll(tasks);

        // 所有 50 次并发调用都应返回正确结果
        Assert.Equal(50, results.Count);
        Assert.All(results, r => Assert.Equal(1, r));
    }

    [Fact]
    public async Task Proxy_ConcurrentGenericCalls_AreThreadSafe()
    {
        using var provider = BuildProvider();

        var tasks = Enumerable.Range(0, 20).Select(async i =>
        {
            using var scope = provider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IGenericMethodService>();
            var result = await service.GetListAsync<string>(new[] { $"key{i}" });
            Assert.NotNull(result);
        });

        await Task.WhenAll(tasks);
    }

    #endregion
}
