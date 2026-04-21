using Skywalker.Ddd.Domain.Events;

namespace Skywalker.Ddd.Domain.Tests;

public class EntityChangedEventTests
{
    [Fact]
    public void EntityChangedEvent_SetsProperties()
    {
        var entity = new TestEntity();

        var createdEvent = new EntityCreatedEvent<TestEntity>(entity);
        var updatedEvent = new EntityUpdatedEvent<TestEntity>(entity);
        var deletedEvent = new EntityDeletedEvent<TestEntity>(entity);

        Assert.Same(entity, createdEvent.Entity);
        Assert.Equal(EntityChangeType.Created, createdEvent.ChangeType);
        Assert.Equal(EntityChangeType.Updated, updatedEvent.ChangeType);
        Assert.Equal(EntityChangeType.Deleted, deletedEvent.ChangeType);
    }

    public class TestEntity { }
}
