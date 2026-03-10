using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Uow.Tests;

public class UnitOfWorkManagerTests
{
    private readonly IAmbientUnitOfWork _ambientUnitOfWork;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<UnitOfWorkManager> _logger;
    private readonly UnitOfWorkManager _manager;

    public UnitOfWorkManagerTests()
    {
        _ambientUnitOfWork = new AmbientUnitOfWork();
        _serviceScopeFactory = CreateServiceScopeFactory();
        _logger = Substitute.For<ILogger<UnitOfWorkManager>>();
        _manager = new UnitOfWorkManager(_ambientUnitOfWork, _serviceScopeFactory, _logger);
    }

    private IServiceScopeFactory CreateServiceScopeFactory()
    {
        var scopeFactory = Substitute.For<IServiceScopeFactory>();
        var scope = Substitute.For<IServiceScope>();
        var serviceProvider = Substitute.For<IServiceProvider>();

        var options = Options.Create(new UnitOfWorkDefaultOptions());
        var logger = Substitute.For<ILogger<UnitOfWork>>();

        serviceProvider.GetService(typeof(IUnitOfWork))
            .Returns(_ => new UnitOfWork(serviceProvider, options, logger));

        scope.ServiceProvider.Returns(serviceProvider);
        scopeFactory.CreateScope().Returns(scope);

        return scopeFactory;
    }

    [Fact]
    public void Current_ShouldBeNullWhenNoUnitOfWorkStarted()
    {
        // Assert
        Assert.Null(_manager.Current);
    }

    [Fact]
    public void Begin_ShouldCreateNewUnitOfWork()
    {
        // Act
        using var uow = _manager.Begin(new UnitOfWorkOptions());

        // Assert
        Assert.NotNull(uow);
        Assert.NotEqual(Guid.Empty, uow.Id);
    }

    [Fact]
    public void Begin_ShouldSetCurrentUnitOfWork()
    {
        // Act
        using var uow = _manager.Begin(new UnitOfWorkOptions());

        // Assert
        Assert.NotNull(_manager.Current);
        Assert.Equal(uow.Id, _manager.Current!.Id);
    }

    [Fact]
    public void Begin_WithExistingUow_ShouldReturnChildUnitOfWork()
    {
        // Arrange
        using var outerUow = _manager.Begin(new UnitOfWorkOptions());

        // Act
        using var innerUow = _manager.Begin(new UnitOfWorkOptions());

        // Assert - 内层 UoW 是 ChildUnitOfWork，共享父级的 Id
        Assert.Equal(outerUow.Id, innerUow.Id);
    }

    [Fact]
    public void Begin_WithRequiresNew_ShouldCreateNewUnitOfWork()
    {
        // Arrange
        using var outerUow = _manager.Begin(new UnitOfWorkOptions());

        // Act
        using var innerUow = _manager.Begin(new UnitOfWorkOptions(), requiresNew: true);

        // Assert - requiresNew=true 时应该创建新的 UoW
        Assert.NotEqual(outerUow.Id, innerUow.Id);
    }

    [Fact]
    public void Reserve_ShouldCreateReservedUnitOfWork()
    {
        // Act
        using var uow = _manager.Reserve("TestReservation");

        // Assert
        Assert.NotNull(uow);
        Assert.True(uow.IsReserved);
        Assert.Equal("TestReservation", uow.ReservationName);
    }

    [Fact]
    public void TryBeginReserved_ShouldReturnFalseWhenNoReservedUow()
    {
        // Act
        var result = _manager.TryBeginReserved("NonExistent", new UnitOfWorkOptions());

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TryBeginReserved_ShouldReturnTrueWhenReservedUowExists()
    {
        // Arrange
        using var reservedUow = _manager.Reserve("TestReservation");

        // Act
        var result = _manager.TryBeginReserved("TestReservation", new UnitOfWorkOptions());

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Current_ShouldSkipDisposedUnitOfWork()
    {
        // Arrange
        var uow = _manager.Begin(new UnitOfWorkOptions());
        uow.Dispose();

        // Assert
        Assert.Null(_manager.Current);
    }

    [Fact]
    public async Task Current_ShouldSkipCompletedUnitOfWork()
    {
        // Arrange
        var uow = _manager.Begin(new UnitOfWorkOptions());
        await uow.CompleteAsync();

        // Assert
        Assert.Null(_manager.Current);
    }
}

