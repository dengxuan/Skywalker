using Skywalker.EventBus;
using Skywalker.EventBus.Abstractions;

namespace Skywalker.EventBus.Tests;

public class EventNameAttributeTests
{
    [Fact]
    public void EventNameAttribute_ShouldSetName()
    {
        // Arrange & Act
        var attribute = new EventNameAttribute("TestEvent");

        // Assert
        Assert.Equal("TestEvent", attribute.Name);
    }

    [Fact]
    public void GetNameOrDefault_WithAttribute_ShouldReturnAttributeName()
    {
        // Act
        var name = EventNameAttribute.GetNameOrDefault<TestEventWithName>();

        // Assert
        Assert.Equal("CustomEventName", name);
    }

    [Fact]
    public void GetNameOrDefault_WithoutAttribute_ShouldReturnFullName()
    {
        // Act
        var name = EventNameAttribute.GetNameOrDefault<TestEventWithoutName>();

        // Assert
        Assert.Equal(typeof(TestEventWithoutName).FullName, name);
    }

    [Fact]
    public void GetName_ShouldReturnName()
    {
        // Arrange
        var attribute = new EventNameAttribute("MyEvent");

        // Act
        var name = attribute.GetName(typeof(TestEventWithName));

        // Assert
        Assert.Equal("MyEvent", name);
    }

    [EventName("CustomEventName")]
    private class TestEventWithName { }

    private class TestEventWithoutName { }
}
