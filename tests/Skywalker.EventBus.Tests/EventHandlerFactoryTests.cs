using Microsoft.Extensions.DependencyInjection;
using Skywalker.EventBus;
using Skywalker.EventBus.Abstractions;

namespace Skywalker.EventBus.Tests;

public class EventHandlerFactoryTests
{
    [Fact]
    public void GetHandler_ShouldCreateHandler()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var factory = new EventHandlerFactory(serviceProvider);

        // Act
        var handler = factory.GetHandler(typeof(TestEventHandler));

        // Assert
        Assert.NotNull(handler);
        Assert.IsType<TestEventHandler>(handler);
    }

    [Fact]
    public void GetHandler_ShouldCreateNewInstanceEachTime()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var factory = new EventHandlerFactory(serviceProvider);

        // Act
        var handler1 = factory.GetHandler(typeof(TestEventHandler));
        var handler2 = factory.GetHandler(typeof(TestEventHandler));

        // Assert
        Assert.NotSame(handler1, handler2);
    }

    [Fact]
    public void GetHandler_WithDependency_ShouldInjectDependencies()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IDependency, TestDependency>();
        var serviceProvider = services.BuildServiceProvider();
        var factory = new EventHandlerFactory(serviceProvider);

        // Act
        var handler = factory.GetHandler(typeof(TestEventHandlerWithDependency)) as TestEventHandlerWithDependency;

        // Assert
        Assert.NotNull(handler);
        Assert.NotNull(handler.Dependency);
    }

    private class TestEvent { }

    private class TestEventHandler : IEventHandler<TestEvent>
    {
        public Task HandleEventAsync(TestEvent eventData) => Task.CompletedTask;
    }

    private interface IDependency { }

    private class TestDependency : IDependency { }

    private class TestEventHandlerWithDependency : IEventHandler<TestEvent>
    {
        public IDependency Dependency { get; }

        public TestEventHandlerWithDependency(IDependency dependency)
        {
            Dependency = dependency;
        }

        public Task HandleEventAsync(TestEvent eventData) => Task.CompletedTask;
    }
}
