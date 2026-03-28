using Skywalker.EventBus;
using Skywalker.EventBus.Abstractions;

namespace Skywalker.EventBus.Tests;

public class NullEventBusTests
{
    [Fact]
    public void Instance_ShouldBeSingleton()
    {
        Assert.Same(NullEventBus.Instance, NullEventBus.Instance);
    }

    [Fact]
    public async Task PublishAsync_Generic_ShouldComplete()
    {
        await NullEventBus.Instance.PublishAsync(new TestEvent());
    }

    [Fact]
    public async Task PublishAsync_ByType_ShouldComplete()
    {
        await NullEventBus.Instance.PublishAsync(typeof(TestEvent), new TestEvent());
    }

    private class TestEvent { }
}

public class GenericEventNameAttributeTests
{
    [Fact]
    public void GetName_WithPrefixAndPostfix_ShouldFormat()
    {
        var attr = new GenericEventNameAttribute
        {
            Prefix = "Pre.",
            Postfix = ".Post"
        };

        var name = attr.GetName(typeof(GenericEvent<SimpleEvent>));
        Assert.StartsWith("Pre.", name);
        Assert.EndsWith(".Post", name);
    }

    [Fact]
    public void GetName_NonGenericType_ShouldThrow()
    {
        var attr = new GenericEventNameAttribute();
        Assert.Throws<Exception>(() => attr.GetName(typeof(SimpleEvent)));
    }

    [EventName("CustomName")]
    private class SimpleEvent { }

    private class GenericEvent<T> { }
}
