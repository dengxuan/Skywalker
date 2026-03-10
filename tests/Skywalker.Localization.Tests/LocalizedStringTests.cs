using Skywalker.Localization;
using Xunit;

namespace Skywalker.Localization.Tests;

public class LocalizedStringTests
{
    [Fact]
    public void Constructor_SetsNameAndValue()
    {
        var localizedString = new LocalizedString("Hello", "你好");

        Assert.Equal("Hello", localizedString.Name);
        Assert.Equal("你好", localizedString.Value);
        Assert.False(localizedString.ResourceNotFound);
        Assert.Null(localizedString.SearchedLocation);
    }

    [Fact]
    public void Constructor_WithResourceNotFound_SetsFlag()
    {
        var localizedString = new LocalizedString("Hello", "Hello", resourceNotFound: true, "TestResource");

        Assert.Equal("Hello", localizedString.Name);
        Assert.Equal("Hello", localizedString.Value);
        Assert.True(localizedString.ResourceNotFound);
        Assert.Equal("TestResource", localizedString.SearchedLocation);
    }

    [Fact]
    public void ImplicitConversion_ReturnsValue()
    {
        var localizedString = new LocalizedString("Hello", "你好");
        string value = localizedString;

        Assert.Equal("你好", value);
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var localizedString = new LocalizedString("Hello", "你好");

        Assert.Equal("你好", localizedString.ToString());
    }

    [Fact]
    public void Constructor_ThrowsOnNullName()
    {
        Assert.Throws<ArgumentNullException>(() => new LocalizedString(null!, "value"));
    }

    [Fact]
    public void Constructor_ThrowsOnNullValue()
    {
        Assert.Throws<ArgumentNullException>(() => new LocalizedString("name", null!));
    }
}

