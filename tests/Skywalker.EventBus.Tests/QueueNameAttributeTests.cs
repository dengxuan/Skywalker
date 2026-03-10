using Skywalker.EventBus;
using Skywalker.EventBus.Abstractions;

namespace Skywalker.EventBus.Tests;

public class QueueNameAttributeTests
{
    [Fact]
    public void QueueNameAttribute_ShouldSetName()
    {
        // Arrange & Act
        var attribute = new QueueNameAttribute("TestQueue");

        // Assert
        Assert.Equal("TestQueue", attribute.Name);
    }

    [Fact]
    public void GetNameOrDefault_WithAttribute_ShouldReturnAttributeName()
    {
        // Act
        var name = QueueNameAttribute.GetNameOrDefault<TestEventWithQueueName>();

        // Assert
        Assert.Equal("CustomQueueName", name);
    }

    [Fact]
    public void GetNameOrDefault_WithoutAttribute_ShouldReturnNull()
    {
        // Act
        var name = QueueNameAttribute.GetNameOrDefault<TestEventWithoutQueueName>();

        // Assert
        Assert.Null(name);
    }

    [Fact]
    public void GetName_ShouldReturnName()
    {
        // Arrange
        var attribute = new QueueNameAttribute("MyQueue");

        // Act
        var name = attribute.GetName(typeof(TestEventWithQueueName));

        // Assert
        Assert.Equal("MyQueue", name);
    }

    [QueueName("CustomQueueName")]
    private class TestEventWithQueueName { }

    private class TestEventWithoutQueueName { }
}
