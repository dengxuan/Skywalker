using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Skywalker.Ddd.AspNetCore.Uow;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;
using Xunit;

namespace Skywalker.Ddd.AspNetCore.Tests;

public class UnitOfWorkMiddlewareTests
{
    private readonly ILogger<UnitOfWorkMiddleware> _logger;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public UnitOfWorkMiddlewareTests()
    {
        _logger = Substitute.For<ILogger<UnitOfWorkMiddleware>>();
        _unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
        _serviceScopeFactory = CreateServiceScopeFactory(transactional: null, httpMethod: null);
    }

    private static IServiceScopeFactory CreateServiceScopeFactory(bool? transactional, string? httpMethod, HttpContext? httpContext = null)
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        var scope = Substitute.For<IServiceScope>();
        var factory = Substitute.For<IServiceScopeFactory>();

        serviceProvider.GetService(typeof(IOptions<UnitOfWorkDefaultOptions>))
            .Returns(Options.Create(new UnitOfWorkDefaultOptions()));
        var behaviourProvider = Substitute.For<IUnitOfWorkTransactionBehaviourProvider>();
        behaviourProvider.IsTransactional.Returns(transactional);
        serviceProvider.GetService(typeof(IUnitOfWorkTransactionBehaviourProvider))
            .Returns(behaviourProvider);

        if (httpContext != null)
        {
            var accessor = Substitute.For<IHttpContextAccessor>();
            accessor.HttpContext.Returns(httpContext);
            serviceProvider.GetService(typeof(IHttpContextAccessor)).Returns(accessor);
        }

        scope.ServiceProvider.Returns(serviceProvider);
        factory.CreateScope().Returns(scope);
        return factory;
    }

    [Fact]
    public async Task InvokeAsync_NullEndpoint_ShouldSkipUow()
    {
        // Arrange
        var nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };
        var middleware = new UnitOfWorkMiddleware(next, _serviceScopeFactory, _logger);
        var context = new DefaultHttpContext(); // no endpoint

        // Act
        await middleware.InvokeAsync(context, _unitOfWorkManager);

        // Assert
        Assert.True(nextCalled);
        _unitOfWorkManager.DidNotReceive().Reserve(Arg.Any<string>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task InvokeAsync_NoActionDescriptor_ShouldSkipUow()
    {
        // Arrange
        var nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };
        var middleware = new UnitOfWorkMiddleware(next, _serviceScopeFactory, _logger);
        var context = CreateHttpContextWithEndpoint(hasActionDescriptor: false);

        // Act
        await middleware.InvokeAsync(context, _unitOfWorkManager);

        // Assert
        Assert.True(nextCalled);
        _unitOfWorkManager.DidNotReceive().Reserve(Arg.Any<string>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task InvokeAsync_DisabledUow_ShouldSkipUow()
    {
        // Arrange
        var nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };
        var middleware = new UnitOfWorkMiddleware(next, _serviceScopeFactory, _logger);
        var context = CreateHttpContextWithEndpoint(
            hasActionDescriptor: true,
            methodInfo: typeof(UnitOfWorkMiddlewareTests).GetMethod(nameof(DisabledUowMethod), BindingFlags.NonPublic | BindingFlags.Static)!);

        // Act
        await middleware.InvokeAsync(context, _unitOfWorkManager);

        // Assert
        Assert.True(nextCalled);
        _unitOfWorkManager.DidNotReceive().Reserve(Arg.Any<string>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task InvokeAsync_ValidAction_ShouldReserveAndComplete()
    {
        // Arrange
        var uow = Substitute.For<IUnitOfWork>();
        uow.IsReserved.Returns(false); // interceptor claimed it
        _unitOfWorkManager.Reserve(Arg.Any<string>(), Arg.Any<bool>()).Returns(uow);

        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new UnitOfWorkMiddleware(next, _serviceScopeFactory, _logger);
        var context = CreateHttpContextWithEndpoint(
            hasActionDescriptor: true,
            methodInfo: typeof(UnitOfWorkMiddlewareTests).GetMethod(nameof(UowEnabledMethod), BindingFlags.NonPublic | BindingFlags.Static)!);

        // Act
        await middleware.InvokeAsync(context, _unitOfWorkManager);

        // Assert
        _unitOfWorkManager.Received(1).Reserve(UnitOfWork.UnitOfWorkReservationName, Arg.Any<bool>());
        await uow.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
        uow.DidNotReceive().Initialize(Arg.Any<UnitOfWorkOptions>());
    }

    [Fact]
    public async Task InvokeAsync_ReservedNotClaimed_ShouldInitializeWithFallbackOptions()
    {
        // Arrange
        var uow = Substitute.For<IUnitOfWork>();
        uow.IsReserved.Returns(true); // no interceptor claimed it
        _unitOfWorkManager.Reserve(Arg.Any<string>(), Arg.Any<bool>()).Returns(uow);

        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new UnitOfWorkMiddleware(next, _serviceScopeFactory, _logger);
        var context = CreateHttpContextWithEndpoint(
            hasActionDescriptor: true,
            methodInfo: typeof(UnitOfWorkMiddlewareTests).GetMethod(nameof(UowEnabledMethod), BindingFlags.NonPublic | BindingFlags.Static)!);

        // Act
        await middleware.InvokeAsync(context, _unitOfWorkManager);

        // Assert
        uow.Received(1).Initialize(Arg.Any<UnitOfWorkOptions>());
        await uow.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InvokeAsync_ExceptionThrown_ShouldRollback()
    {
        // Arrange
        var uow = Substitute.For<IUnitOfWork>();
        uow.IsReserved.Returns(false);
        _unitOfWorkManager.Reserve(Arg.Any<string>(), Arg.Any<bool>()).Returns(uow);

        RequestDelegate next = _ => throw new InvalidOperationException("test error");
        var middleware = new UnitOfWorkMiddleware(next, _serviceScopeFactory, _logger);
        var context = CreateHttpContextWithEndpoint(
            hasActionDescriptor: true,
            methodInfo: typeof(UnitOfWorkMiddlewareTests).GetMethod(nameof(UowEnabledMethod), BindingFlags.NonPublic | BindingFlags.Static)!);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            middleware.InvokeAsync(context, _unitOfWorkManager));
        await uow.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        await uow.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("GET", false)]
    [InlineData("HEAD", false)]
    [InlineData("OPTIONS", false)]
    [InlineData("POST", true)]
    [InlineData("PUT", true)]
    [InlineData("PATCH", true)]
    [InlineData("DELETE", true)]
    public async Task InvokeAsync_HttpMethod_ShouldDetermineTransactional(string httpMethod, bool expectedTransactional)
    {
        // Arrange — httpContext with the given httpMethod for CreateOptions to read
        var httpContext = CreateHttpContextWithEndpoint(
            hasActionDescriptor: true,
            methodInfo: typeof(UnitOfWorkMiddlewareTests).GetMethod(nameof(UowEnabledMethod), BindingFlags.NonPublic | BindingFlags.Static)!,
            httpMethod: httpMethod);

        var scopeFactory = CreateServiceScopeFactory(transactional: null, httpMethod: null, httpContext: httpContext);

        UnitOfWorkOptions? capturedOptions = null;
        var uow = Substitute.For<IUnitOfWork>();
        uow.IsReserved.Returns(true); // force fallback initialization
        uow.When(u => u.Initialize(Arg.Any<UnitOfWorkOptions>()))
            .Do(ci => capturedOptions = ci.Arg<UnitOfWorkOptions>());
        _unitOfWorkManager.Reserve(Arg.Any<string>(), Arg.Any<bool>()).Returns(uow);

        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new UnitOfWorkMiddleware(next, scopeFactory, _logger);

        // Act
        await middleware.InvokeAsync(httpContext, _unitOfWorkManager);

        // Assert
        Assert.NotNull(capturedOptions);
        Assert.Equal(expectedTransactional, capturedOptions!.IsTransactional);
    }

    [Fact]
    public async Task InvokeAsync_WithTransactionalAttribute_ShouldUseAttributeOptions()
    {
        // Arrange
        UnitOfWorkOptions? capturedOptions = null;
        var uow = Substitute.For<IUnitOfWork>();
        uow.IsReserved.Returns(true);
        uow.When(u => u.Initialize(Arg.Any<UnitOfWorkOptions>()))
            .Do(ci => capturedOptions = ci.Arg<UnitOfWorkOptions>());
        _unitOfWorkManager.Reserve(Arg.Any<string>(), Arg.Any<bool>()).Returns(uow);

        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new UnitOfWorkMiddleware(next, _serviceScopeFactory, _logger);
        var context = CreateHttpContextWithEndpoint(
            hasActionDescriptor: true,
            methodInfo: typeof(UnitOfWorkMiddlewareTests).GetMethod(nameof(TransactionalMethod), BindingFlags.NonPublic | BindingFlags.Static)!);

        // Act
        await middleware.InvokeAsync(context, _unitOfWorkManager);

        // Assert
        Assert.NotNull(capturedOptions);
        Assert.True(capturedOptions!.IsTransactional);
    }

    [Fact]
    public async Task InvokeAsync_WithBehaviourProvider_ShouldUseProviderValue()
    {
        // Arrange
        var scopeFactory = CreateServiceScopeFactory(transactional: true, httpMethod: null);

        UnitOfWorkOptions? capturedOptions = null;
        var uow = Substitute.For<IUnitOfWork>();
        uow.IsReserved.Returns(true);
        uow.When(u => u.Initialize(Arg.Any<UnitOfWorkOptions>()))
            .Do(ci => capturedOptions = ci.Arg<UnitOfWorkOptions>());
        _unitOfWorkManager.Reserve(Arg.Any<string>(), Arg.Any<bool>()).Returns(uow);

        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new UnitOfWorkMiddleware(next, scopeFactory, _logger);
        var context = CreateHttpContextWithEndpoint(
            hasActionDescriptor: true,
            methodInfo: typeof(UnitOfWorkMiddlewareTests).GetMethod(nameof(UowEnabledMethod), BindingFlags.NonPublic | BindingFlags.Static)!);

        // Act
        await middleware.InvokeAsync(context, _unitOfWorkManager);

        // Assert
        Assert.NotNull(capturedOptions);
        Assert.True(capturedOptions!.IsTransactional);
    }

    #region Helper methods

    private static HttpContext CreateHttpContextWithEndpoint(
        bool hasActionDescriptor,
        MethodInfo? methodInfo = null,
        string? httpMethod = null)
    {
        var context = new DefaultHttpContext();

        if (httpMethod != null)
        {
            context.Request.Method = httpMethod;
        }

        var metadata = new List<object>();
        if (hasActionDescriptor)
        {
            var actionDescriptor = new ControllerActionDescriptor
            {
                MethodInfo = methodInfo ?? typeof(UnitOfWorkMiddlewareTests).GetMethod(nameof(UowEnabledMethod), BindingFlags.NonPublic | BindingFlags.Static)!
            };
            metadata.Add(actionDescriptor);
        }

        var endpoint = new Endpoint(_ => Task.CompletedTask, new EndpointMetadataCollection(metadata), "test");
        context.SetEndpoint(endpoint);

        return context;
    }

    [UnitOfWork(IsDisabled = true)]
    private static void DisabledUowMethod() { }

    [UnitOfWork]
    private static void UowEnabledMethod() { }

    [UnitOfWork(true)]
    private static void TransactionalMethod() { }

    #endregion
}
