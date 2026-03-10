using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Uow.Tests;

public class UnitOfWorkTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<UnitOfWorkDefaultOptions> _options;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWorkTests()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
        _options = Options.Create(new UnitOfWorkDefaultOptions());
        _logger = Substitute.For<ILogger<UnitOfWork>>();
    }

    private UnitOfWork CreateUnitOfWork()
    {
        return new UnitOfWork(_serviceProvider, _options, _logger);
    }

    [Fact]
    public void Constructor_ShouldCreateUnitOfWorkWithNewGuid()
    {
        // Act
        var uow = CreateUnitOfWork();

        // Assert
        Assert.NotEqual(Guid.Empty, uow.Id);
        Assert.False(uow.IsDisposed);
        Assert.False(uow.IsCompleted);
        Assert.False(uow.IsReserved);
    }

    [Fact]
    public void Initialize_ShouldSetOptions()
    {
        // Arrange
        var uow = CreateUnitOfWork();
        var options = new UnitOfWorkOptions { IsTransactional = true };

        // Act
        uow.Initialize(options);

        // Assert
        Assert.NotNull(uow.Options);
        Assert.False(uow.IsReserved);
    }

    [Fact]
    public void Initialize_ShouldThrowIfAlreadyInitialized()
    {
        // Arrange
        var uow = CreateUnitOfWork();
        uow.Initialize(new UnitOfWorkOptions());

        // Act & Assert
        var ex = Assert.ThrowsAny<Exception>(() => uow.Initialize(new UnitOfWorkOptions()));
        Assert.Contains("already initialized", ex.Message);
    }

    [Fact]
    public void Reserve_ShouldSetReservationName()
    {
        // Arrange
        var uow = CreateUnitOfWork();

        // Act
        uow.Reserve("TestReservation");

        // Assert
        Assert.Equal("TestReservation", uow.ReservationName);
        Assert.True(uow.IsReserved);
    }

    [Fact]
    public void SetOuter_ShouldSetOuterUnitOfWork()
    {
        // Arrange
        var uow = CreateUnitOfWork();
        var outerUow = CreateUnitOfWork();

        // Act
        uow.SetOuter(outerUow);

        // Assert
        Assert.Same(outerUow, uow.Outer);
    }

    [Fact]
    public async Task CompleteAsync_ShouldSetIsCompleted()
    {
        // Arrange
        var uow = CreateUnitOfWork();
        uow.Initialize(new UnitOfWorkOptions());

        // Act
        await uow.CompleteAsync();

        // Assert
        Assert.True(uow.IsCompleted);
    }

    [Fact]
    public async Task CompleteAsync_ShouldThrowIfCalledTwice()
    {
        // Arrange
        var uow = CreateUnitOfWork();
        uow.Initialize(new UnitOfWorkOptions());
        await uow.CompleteAsync();

        // Act & Assert
        var ex = await Assert.ThrowsAnyAsync<Exception>(async () => await uow.CompleteAsync());
        Assert.Contains("Complete is called before", ex.Message);
    }

    [Fact]
    public void Dispose_ShouldSetIsDisposed()
    {
        // Arrange
        var uow = CreateUnitOfWork();
        uow.Initialize(new UnitOfWorkOptions());

        // Act
        uow.Dispose();

        // Assert
        Assert.True(uow.IsDisposed);
    }

    [Fact]
    public void Dispose_ShouldBeIdempotent()
    {
        // Arrange
        var uow = CreateUnitOfWork();
        uow.Initialize(new UnitOfWorkOptions());

        // Act
        uow.Dispose();
        uow.Dispose(); // 第二次调用不应该抛异常

        // Assert
        Assert.True(uow.IsDisposed);
    }

    [Fact]
    public void OnCompleted_ShouldRegisterHandler()
    {
        // Arrange
        var uow = CreateUnitOfWork();
        var handlerCalled = false;

        // Act
        uow.OnCompleted(() => { handlerCalled = true; return Task.CompletedTask; });
        uow.Initialize(new UnitOfWorkOptions());

        // Assert (handler not called yet)
        Assert.False(handlerCalled);
    }
}

