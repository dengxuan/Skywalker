using Xunit;

namespace Skywalker.Extensions.Universal.Tests;

public class TestTarget
{
    public string Name { get; set; } = "default";
    public int Age { get; set; }
    public string ReadOnly { get; } = "readonly";
}

public class ObjectHelperTests
{
    [Fact]
    public void TrySetProperty_SetsWritableProperty()
    {
        var obj = new TestTarget();
        ObjectHelper.TrySetProperty(obj, x => x.Name, () => "updated");

        Assert.Equal("updated", obj.Name);
    }

    [Fact]
    public void TrySetProperty_WithObjectParam_SetsProperty()
    {
        var obj = new TestTarget();
        ObjectHelper.TrySetProperty<TestTarget, int>(obj, x => x.Age, o => 42);

        Assert.Equal(42, obj.Age);
    }

    [Fact]
    public void TrySetProperty_SetsValueType()
    {
        var obj = new TestTarget();
        ObjectHelper.TrySetProperty(obj, x => x.Age, () => 30);

        Assert.Equal(30, obj.Age);
    }
}
