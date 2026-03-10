using NSubstitute;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Uow.Tests;

public class AmbientUnitOfWorkTests
{
    [Fact]
    public void UnitOfWork_ShouldBeNullByDefault()
    {
        // Arrange
        var ambientUow = new AmbientUnitOfWork();

        // Assert
        Assert.Null(ambientUow.UnitOfWork);
    }

    [Fact]
    public void SetUnitOfWork_ShouldSetCurrentUnitOfWork()
    {
        // Arrange
        var ambientUow = new AmbientUnitOfWork();
        var uow = Substitute.For<IUnitOfWork>();

        // Act
        ambientUow.SetUnitOfWork(uow);

        // Assert
        Assert.Same(uow, ambientUow.UnitOfWork);
    }

    [Fact]
    public void SetUnitOfWork_ShouldAllowSettingNull()
    {
        // Arrange
        var ambientUow = new AmbientUnitOfWork();
        var uow = Substitute.For<IUnitOfWork>();
        ambientUow.SetUnitOfWork(uow);

        // Act
        ambientUow.SetUnitOfWork(null);

        // Assert
        Assert.Null(ambientUow.UnitOfWork);
    }

    [Fact]
    public async Task UnitOfWork_ShouldFlowAcrossAsyncContext()
    {
        // Arrange
        var ambientUow = new AmbientUnitOfWork();
        var uow = Substitute.For<IUnitOfWork>();
        ambientUow.SetUnitOfWork(uow);

        // Act & Assert - AsyncLocal 应该在异步上下文中流转
        await Task.Run(() =>
        {
            Assert.Same(uow, ambientUow.UnitOfWork);
        });
    }

    [Fact]
    public async Task UnitOfWork_ShouldBeIsolatedBetweenParallelTasks()
    {
        // Arrange
        var ambientUow = new AmbientUnitOfWork();
        var uow1 = Substitute.For<IUnitOfWork>();
        var uow2 = Substitute.For<IUnitOfWork>();
        uow1.Id.Returns(Guid.NewGuid());
        uow2.Id.Returns(Guid.NewGuid());

        // Act
        var task1 = Task.Run(() =>
        {
            ambientUow.SetUnitOfWork(uow1);
            Thread.Sleep(50); // 确保 task2 有时间设置自己的值
            return ambientUow.UnitOfWork;
        });

        var task2 = Task.Run(() =>
        {
            ambientUow.SetUnitOfWork(uow2);
            Thread.Sleep(50);
            return ambientUow.UnitOfWork;
        });

        var results = await Task.WhenAll(task1, task2);

        // Assert - 每个任务应该看到自己设置的值
        Assert.Same(uow1, results[0]);
        Assert.Same(uow2, results[1]);
    }
}

