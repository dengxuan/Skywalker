using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Skywalker.Extensions.DynamicProxies;

namespace Skywalker.Extensions.DynamicProxies.Tests;

public class InterceptorChainBuilderTests
{
    [Fact]
    public async Task Build_WithNoMiddleware_CallsTarget()
    {
        var sp = new ServiceCollection().BuildServiceProvider();
        var builder = new InterceptorChainBuilder(sp);
        var called = false;

        var pipeline = builder.Build(ctx =>
        {
            called = true;
            return Task.CompletedTask;
        });

        var method = typeof(string).GetMethod("ToString", Type.EmptyTypes)!;
        var context = new InterceptorContext("target", method, Array.Empty<object?>());
        await pipeline(context);

        Assert.True(called);
    }

    [Fact]
    public async Task Build_WithMiddleware_ExecutesInOrder()
    {
        var sp = new ServiceCollection().BuildServiceProvider();
        var builder = new InterceptorChainBuilder(sp);
        var order = new List<string>();

        builder.Use(next => async ctx =>
        {
            order.Add("before-1");
            await next(ctx);
            order.Add("after-1");
        });

        builder.Use(next => async ctx =>
        {
            order.Add("before-2");
            await next(ctx);
            order.Add("after-2");
        });

        var pipeline = builder.Build(ctx =>
        {
            order.Add("target");
            return Task.CompletedTask;
        });

        var method = typeof(string).GetMethod("ToString", Type.EmptyTypes)!;
        var context = new InterceptorContext("target", method, Array.Empty<object?>());
        await pipeline(context);

        Assert.Equal(new[] { "before-1", "before-2", "target", "after-2", "after-1" }, order);
    }

    [Fact]
    public async Task Build_MiddlewareCanShortCircuit()
    {
        var sp = new ServiceCollection().BuildServiceProvider();
        var builder = new InterceptorChainBuilder(sp);
        var targetCalled = false;

        builder.Use(next => ctx =>
        {
            // Don't call next
            return Task.CompletedTask;
        });

        var pipeline = builder.Build(ctx =>
        {
            targetCalled = true;
            return Task.CompletedTask;
        });

        var method = typeof(string).GetMethod("ToString", Type.EmptyTypes)!;
        var context = new InterceptorContext("target", method, Array.Empty<object?>());
        await pipeline(context);

        Assert.False(targetCalled);
    }

    [Fact]
    public void ServiceProvider_IsExposed()
    {
        var sp = new ServiceCollection().BuildServiceProvider();
        var builder = new InterceptorChainBuilder(sp);

        Assert.Same(sp, builder.ServiceProvider);
    }
}

public class InterceptorContextTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var target = new object();
        var method = typeof(object).GetMethod("ToString")!;
        var args = new object?[] { "arg1", 42 };

        var context = new InterceptorContext(target, method, args);

        Assert.Same(target, context.Target);
        Assert.Same(method, context.Method);
        Assert.Equal("ToString", context.MethodName);
        Assert.Same(args, context.Arguments);
        Assert.Equal(typeof(string), context.ReturnType);
        Assert.Null(context.ReturnValue);
    }

    [Fact]
    public void ReturnValue_CanBeSetAndRead()
    {
        var method = typeof(object).GetMethod("ToString")!;
        var context = new InterceptorContext("target", method, Array.Empty<object?>());

        context.ReturnValue = "result";

        Assert.Equal("result", context.ReturnValue);
    }

    [Fact]
    public async Task ProceedAsync_ThrowsWhenNoProceedAction()
    {
        var method = typeof(object).GetMethod("ToString")!;
        var context = new InterceptorContext("target", method, Array.Empty<object?>());

        // Public constructor does not set _proceedAction
        await Assert.ThrowsAsync<InvalidOperationException>(() => context.ProceedAsync());
    }
}

public class InterceptorChainBuilderExtensionsTests
{
    [Fact]
    public async Task UseInterceptor_WithInstance_AddsToChain()
    {
        var sp = new ServiceCollection().BuildServiceProvider();
        var builder = new InterceptorChainBuilder(sp);
        var order = new List<string>();

        var interceptor = new TestInterceptor(order);
        builder.UseInterceptor(interceptor);

        var pipeline = builder.Build(ctx =>
        {
            order.Add("target");
            return Task.CompletedTask;
        });

        var method = typeof(string).GetMethod("ToString", Type.EmptyTypes)!;
        var context = new InterceptorContext("target", method, Array.Empty<object?>());
        await pipeline(context);

        Assert.Equal(new[] { "before-intercept", "target", "after-intercept" }, order);
    }

    [Fact]
    public async Task UseInterceptor_WithType_ResolvesFromDI()
    {
        var order = new List<string>();
        var services = new ServiceCollection();
        services.AddSingleton(order);
        services.AddTransient<TestInterceptor>();
        var sp = services.BuildServiceProvider();

        var builder = new InterceptorChainBuilder(sp);
        builder.UseInterceptor<TestInterceptor>();

        var pipeline = builder.Build(ctx =>
        {
            order.Add("target");
            return Task.CompletedTask;
        });

        var method = typeof(string).GetMethod("ToString", Type.EmptyTypes)!;
        var context = new InterceptorContext("target", method, Array.Empty<object?>());
        await pipeline(context);

        Assert.Equal(new[] { "before-intercept", "target", "after-intercept" }, order);
    }

    [Fact]
    public async Task UseInterceptors_MultipleInstances_ChainInOrder()
    {
        var sp = new ServiceCollection().BuildServiceProvider();
        var builder = new InterceptorChainBuilder(sp);
        var order = new List<string>();

        var interceptor1 = new NamedInterceptor("A", order);
        var interceptor2 = new NamedInterceptor("B", order);
        builder.UseInterceptors(new IInterceptor[] { interceptor1, interceptor2 });

        var pipeline = builder.Build(ctx =>
        {
            order.Add("target");
            return Task.CompletedTask;
        });

        var method = typeof(string).GetMethod("ToString", Type.EmptyTypes)!;
        var context = new InterceptorContext("target", method, Array.Empty<object?>());
        await pipeline(context);

        Assert.Equal(new[] { "before-A", "before-B", "target", "after-B", "after-A" }, order);
    }
}

internal class TestInterceptor : IInterceptor
{
    private readonly List<string> _order;

    public TestInterceptor(List<string> order)
    {
        _order = order;
    }

    public async Task InterceptAsync(IMethodInvocation invocation)
    {
        _order.Add("before-intercept");
        await invocation.ProceedAsync();
        _order.Add("after-intercept");
    }
}

internal class NamedInterceptor : IInterceptor
{
    private readonly string _name;
    private readonly List<string> _order;

    public NamedInterceptor(string name, List<string> order)
    {
        _name = name;
        _order = order;
    }

    public async Task InterceptAsync(IMethodInvocation invocation)
    {
        _order.Add($"before-{_name}");
        await invocation.ProceedAsync();
        _order.Add($"after-{_name}");
    }
}
