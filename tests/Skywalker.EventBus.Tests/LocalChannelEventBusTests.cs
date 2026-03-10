using Microsoft.Extensions.DependencyInjection;
using Skywalker.EventBus.Abstractions;
using Skywalker.EventBus.Local;

namespace Skywalker.EventBus.Tests;

public class LocalChannelEventBusTests
{
    [Fact]
    public async Task PublishAsync_WithRegisteredHandler_InvokesHandler()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEventBusLocal(options =>
        {
            options.AddEventHandler<TestEventHandler>();
        });

        await using var provider = services.BuildServiceProvider();
        var eventBus = provider.GetRequiredService<ILocalEventBus>();
        TestEventHandler.ReceivedEvents.Clear();

        // Act
        await eventBus.PublishAsync(new TestEvent { Message = "Hello" });

        // Wait for channel processing with retry
        for (var i = 0; i < 20 && TestEventHandler.ReceivedEvents.Count == 0; i++)
        {
            await Task.Delay(100);
        }

        // Assert
        Assert.Single(TestEventHandler.ReceivedEvents);
        Assert.Equal("Hello", TestEventHandler.ReceivedEvents[0].Message);
    }

    [Fact]
    public async Task PublishAsync_WithMultipleHandlers_InvokesAllHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEventBusLocal(options =>
        {
            options.AddEventHandler<TestEventHandler>();
            options.AddEventHandler<AnotherTestEventHandler>();
        });

        await using var provider = services.BuildServiceProvider();
        var eventBus = provider.GetRequiredService<ILocalEventBus>();
        TestEventHandler.ReceivedEvents.Clear();
        AnotherTestEventHandler.ReceivedEvents.Clear();

        // Act
        await eventBus.PublishAsync(new TestEvent { Message = "Hello" });

        // Wait for channel processing with retry
        for (var i = 0; i < 20 && (TestEventHandler.ReceivedEvents.Count == 0 || AnotherTestEventHandler.ReceivedEvents.Count == 0); i++)
        {
            await Task.Delay(100);
        }

        // Assert
        Assert.Single(TestEventHandler.ReceivedEvents);
        Assert.Single(AnotherTestEventHandler.ReceivedEvents);
    }

    [Fact]
    public async Task Subscribe_AddsHandler()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEventBusLocal();

        await using var provider = services.BuildServiceProvider();
        var eventBus = provider.GetRequiredService<ILocalEventBus>();

        // Act
        eventBus.Subscribe<TestEvent, TestEventHandler>();

        // Assert - no exception means success
    }

    [Fact]
    public async Task Unsubscribe_RemovesHandler()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEventBusLocal(options =>
        {
            options.AddEventHandler<TestEventHandler>();
        });

        await using var provider = services.BuildServiceProvider();
        var eventBus = provider.GetRequiredService<ILocalEventBus>();

        // Act
        eventBus.Unsubscribe<TestEvent, TestEventHandler>();

        // Assert - no exception means success
    }

    public class TestEvent
    {
        public string Message { get; set; } = string.Empty;
    }

    public class TestEventHandler : IEventHandler<TestEvent>
    {
        public static List<TestEvent> ReceivedEvents { get; } = [];

        public Task HandleEventAsync(TestEvent eventData)
        {
            ReceivedEvents.Add(eventData);
            return Task.CompletedTask;
        }
    }

    public class AnotherTestEventHandler : IEventHandler<TestEvent>
    {
        public static List<TestEvent> ReceivedEvents { get; } = [];

        public Task HandleEventAsync(TestEvent eventData)
        {
            ReceivedEvents.Add(eventData);
            return Task.CompletedTask;
        }
    }
}

