using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.Extensions.DynamicProxies;
using Xunit;

namespace Skywalker.Ddd.Uow.Tests;

public class UnitOfWorkInterceptorTests
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<UnitOfWorkInterceptor> _logger;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly UnitOfWorkInterceptor _interceptor;

    public UnitOfWorkInterceptorTests()
    {
        _logger = Substitute.For<ILogger<UnitOfWorkInterceptor>>();
        _unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
        _serviceScopeFactory = CreateScopeFactory(transactional: null);
        _interceptor = new UnitOfWorkInterceptor(_serviceScopeFactory, _logger);
    }

    private IServiceScopeFactory CreateScopeFactory(bool? transactional)
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
        serviceProvider.GetService(typeof(IUnitOfWorkManager)).Returns(_unitOfWorkManager);

        scope.ServiceProvider.Returns(serviceProvider);
        factory.CreateScope().Returns(scope);
        return factory;
    }

    [Fact]
    public async Task InterceptAsync_NonUowMethod_ShouldProceedWithoutUow()
    {
        // Arrange — method with [UnitOfWork(IsDisabled = true)]
        var invocation = CreateInvocation(nameof(DisabledUowMethod));

        // Act
        await _interceptor.InterceptAsync(invocation);

        // Assert
        await invocation.Received(1).ProceedAsync();
        _unitOfWorkManager.DidNotReceive().TryBeginReserved(Arg.Any<string>(), Arg.Any<UnitOfWorkOptions>());
        _unitOfWorkManager.DidNotReceive().Begin(Arg.Any<UnitOfWorkOptions>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task InterceptAsync_UowMethod_TryBeginReservedSucceeds_ShouldNotBeginNew()
    {
        // Arrange
        _unitOfWorkManager.TryBeginReserved(Arg.Any<string>(), Arg.Any<UnitOfWorkOptions>()).Returns(true);
        var invocation = CreateInvocation(nameof(CreateOrderMethod));

        // Act
        await _interceptor.InterceptAsync(invocation);

        // Assert
        await invocation.Received(1).ProceedAsync();
        _unitOfWorkManager.Received(1).TryBeginReserved(UnitOfWork.UnitOfWorkReservationName, Arg.Any<UnitOfWorkOptions>());
        _unitOfWorkManager.DidNotReceive().Begin(Arg.Any<UnitOfWorkOptions>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task InterceptAsync_UowMethod_NoReserved_ShouldBeginAndComplete()
    {
        // Arrange
        var uow = Substitute.For<IUnitOfWork>();
        _unitOfWorkManager.TryBeginReserved(Arg.Any<string>(), Arg.Any<UnitOfWorkOptions>()).Returns(false);
        _unitOfWorkManager.Begin(Arg.Any<UnitOfWorkOptions>(), Arg.Any<bool>()).Returns(uow);
        var invocation = CreateInvocation(nameof(CreateOrderMethod));

        // Act
        await _interceptor.InterceptAsync(invocation);

        // Assert
        await invocation.Received(1).ProceedAsync();
        await uow.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InterceptAsync_UowMethod_Exception_ShouldRollback()
    {
        // Arrange
        var uow = Substitute.For<IUnitOfWork>();
        _unitOfWorkManager.TryBeginReserved(Arg.Any<string>(), Arg.Any<UnitOfWorkOptions>()).Returns(false);
        _unitOfWorkManager.Begin(Arg.Any<UnitOfWorkOptions>(), Arg.Any<bool>()).Returns(uow);
        var invocation = CreateInvocation(nameof(CreateOrderMethod));
        invocation.ProceedAsync().Returns(_ => throw new InvalidOperationException("test"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _interceptor.InterceptAsync(invocation));
        await uow.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        await uow.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InterceptAsync_GetMethod_ShouldSetNonTransactional()
    {
        // Arrange — "GetOrders" starts with "Get" → non-transactional
        UnitOfWorkOptions? capturedOptions = null;
        _unitOfWorkManager.TryBeginReserved(Arg.Any<string>(), Arg.Any<UnitOfWorkOptions>()).Returns(false);
        _unitOfWorkManager.Begin(Arg.Any<UnitOfWorkOptions>(), Arg.Any<bool>())
            .Returns(ci =>
            {
                capturedOptions = ci.Arg<UnitOfWorkOptions>();
                return Substitute.For<IUnitOfWork>();
            });
        var invocation = CreateInvocation(nameof(GetOrdersMethod));

        // Act
        await _interceptor.InterceptAsync(invocation);

        // Assert
        Assert.NotNull(capturedOptions);
        Assert.False(capturedOptions!.IsTransactional);
    }

    [Fact]
    public async Task InterceptAsync_FindMethod_ShouldSetNonTransactional()
    {
        // Arrange — "FindById" starts with "Find" → non-transactional
        UnitOfWorkOptions? capturedOptions = null;
        _unitOfWorkManager.TryBeginReserved(Arg.Any<string>(), Arg.Any<UnitOfWorkOptions>()).Returns(false);
        _unitOfWorkManager.Begin(Arg.Any<UnitOfWorkOptions>(), Arg.Any<bool>())
            .Returns(ci =>
            {
                capturedOptions = ci.Arg<UnitOfWorkOptions>();
                return Substitute.For<IUnitOfWork>();
            });
        var invocation = CreateInvocation(nameof(FindByIdMethod));

        // Act
        await _interceptor.InterceptAsync(invocation);

        // Assert
        Assert.NotNull(capturedOptions);
        Assert.False(capturedOptions!.IsTransactional);
    }

    [Fact]
    public async Task InterceptAsync_WithTransactionalAttribute_ShouldUseAttributeValue()
    {
        // Arrange
        UnitOfWorkOptions? capturedOptions = null;
        _unitOfWorkManager.TryBeginReserved(Arg.Any<string>(), Arg.Any<UnitOfWorkOptions>()).Returns(false);
        _unitOfWorkManager.Begin(Arg.Any<UnitOfWorkOptions>(), Arg.Any<bool>())
            .Returns(ci =>
            {
                capturedOptions = ci.Arg<UnitOfWorkOptions>();
                return Substitute.For<IUnitOfWork>();
            });
        var invocation = CreateInvocation(nameof(ExplicitTransactionalMethod));

        // Act
        await _interceptor.InterceptAsync(invocation);

        // Assert
        Assert.NotNull(capturedOptions);
        Assert.True(capturedOptions!.IsTransactional);
    }

    [Fact]
    public async Task InterceptAsync_WithBehaviourProvider_ShouldUseProviderValue()
    {
        // Arrange
        var scopeFactory = CreateScopeFactory(transactional: false);
        var interceptor = new UnitOfWorkInterceptor(scopeFactory, _logger);

        UnitOfWorkOptions? capturedOptions = null;
        _unitOfWorkManager.TryBeginReserved(Arg.Any<string>(), Arg.Any<UnitOfWorkOptions>()).Returns(false);
        _unitOfWorkManager.Begin(Arg.Any<UnitOfWorkOptions>(), Arg.Any<bool>())
            .Returns(ci =>
            {
                capturedOptions = ci.Arg<UnitOfWorkOptions>();
                return Substitute.For<IUnitOfWork>();
            });
        var invocation = CreateInvocation(nameof(CreateOrderMethod));

        // Act
        await interceptor.InterceptAsync(invocation);

        // Assert
        Assert.NotNull(capturedOptions);
        Assert.False(capturedOptions!.IsTransactional);
    }

    #region Helper methods

    private static IMethodInvocation CreateInvocation(string methodName)
    {
        var invocation = Substitute.For<IMethodInvocation>();
        var method = typeof(UnitOfWorkInterceptorTests).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static)!;
        invocation.Method.Returns(method);
        invocation.ProceedAsync().Returns(Task.CompletedTask);
        return invocation;
    }

    [UnitOfWork(IsDisabled = true)]
    private static void DisabledUowMethod() { }

    [UnitOfWork]
    private static void CreateOrderMethod() { }

    [UnitOfWork]
    private static void GetOrdersMethod() { }

    [UnitOfWork]
    private static void FindByIdMethod() { }

    [UnitOfWork(true)]
    private static void ExplicitTransactionalMethod() { }

    #endregion
}
