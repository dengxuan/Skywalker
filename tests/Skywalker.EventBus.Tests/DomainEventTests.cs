using Microsoft.Extensions.DependencyInjection;
using Skywalker.EventBus.Abstractions;
using Skywalker.EventBus.Local;

namespace Skywalker.EventBus.Tests;

public class DomainEventTests
{
    [Fact]
    public void DomainEventBase_SetsOccurredOn()
    {
        // Arrange & Act
        var before = DateTimeOffset.UtcNow;
        var domainEvent = new TestDomainEvent();
        var after = DateTimeOffset.UtcNow;

        // Assert
        Assert.InRange(domainEvent.OccurredOn, before, after);
    }

    [Fact]
    public void DomainEventCollection_AddAndClear()
    {
        // Arrange
        var collection = new DomainEventCollection();
        var event1 = new TestDomainEvent();
        var event2 = new TestDomainEvent();

        // Act
        collection.Add(event1);
        collection.Add(event2);

        // Assert
        Assert.Equal(2, collection.Count);

        // Act
        collection.Clear();

        // Assert
        Assert.Empty(collection);
    }

    [Fact]
    public async Task DomainEventDispatcher_DispatchesEvents()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEventBusLocal(options =>
        {
            options.AddEventHandler<TestDomainEventHandler>();
        });

        await using var provider = services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IDomainEventDispatcher>();
        TestDomainEventHandler.ReceivedEvents.Clear();

        var entity = new TestEntity();
        entity.RaiseEvent(new TestDomainEvent { Value = "Test" });

        // Act
        await dispatcher.DispatchEventsAsync([entity]);

        // Wait for channel processing with timeout (CI environments may be slower)
        var timeout = DateTime.UtcNow.AddSeconds(5);
        while (TestDomainEventHandler.ReceivedEvents.Count == 0 && DateTime.UtcNow < timeout)
        {
            await Task.Delay(50);
        }

        // Assert
        Assert.Single(TestDomainEventHandler.ReceivedEvents);
        Assert.Equal("Test", TestDomainEventHandler.ReceivedEvents[0].Value);
        Assert.Empty(entity.DomainEvents);
    }

    public class TestDomainEvent : DomainEventBase
    {
        public string Value { get; set; } = string.Empty;
    }

    public class TestDomainEventHandler : IEventHandler<TestDomainEvent>
    {
        public static List<TestDomainEvent> ReceivedEvents { get; } = [];

        public Task HandleEventAsync(TestDomainEvent eventData)
        {
            ReceivedEvents.Add(eventData);
            return Task.CompletedTask;
        }
    }

    public class TestEntity : IHasDomainEvents
    {
        private readonly DomainEventCollection _domainEvents = new();

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

        public void RaiseEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}

