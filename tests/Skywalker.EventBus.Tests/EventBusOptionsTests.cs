using Skywalker.EventBus;
using Skywalker.EventBus.Abstractions;

namespace Skywalker.EventBus.Tests;

public class EventBusOptionsTests
{
    [Fact]
    public void AddEventHandler_WithValidHandler_ShouldAdd()
    {
        // Arrange
        var options = new EventBusOptions();

        // Act
        options.AddEventHandler<TestEventHandler>();

        // Assert
        Assert.Contains(typeof(TestEventHandler), options.Handlers);
    }

    [Fact]
    public void AddEventHandler_WithInvalidHandler_ShouldThrow()
    {
        // Arrange
        var options = new EventBusOptions();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => options.AddEventHandler<InvalidEventHandler>());
    }

    [Fact]
    public void Handlers_ShouldBeEmptyByDefault()
    {
        // Arrange & Act
        var options = new EventBusOptions();

        // Assert
        Assert.Empty(options.Handlers);
    }

    private class TestEvent { }

    private class TestEventHandler : IEventHandler<TestEvent>
    {
        public Task HandleEventAsync(TestEvent eventData) => Task.CompletedTask;
    }

    private class InvalidEventHandler : IEventHandler
    {
    }
}
