using System.Globalization;
using Skywalker.Localization;
using Xunit;

namespace Skywalker.Localization.Tests;

public class NullStringLocalizerTests
{
    [Fact]
    public void Instance_ReturnsSameInstance()
    {
        var instance1 = NullStringLocalizer.Instance;
        var instance2 = NullStringLocalizer.Instance;

        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void Indexer_ReturnsKeyAsValue()
    {
        var localizer = NullStringLocalizer.Instance;

        var result = localizer["Hello"];

        Assert.Equal("Hello", result.Name);
        Assert.Equal("Hello", result.Value);
        Assert.True(result.ResourceNotFound);
    }

    [Fact]
    public void Indexer_WithArguments_FormatsString()
    {
        var localizer = NullStringLocalizer.Instance;

        var result = localizer["Hello {0}", "World"];

        Assert.Equal("Hello {0}", result.Name);
        Assert.Equal("Hello World", result.Value);
        Assert.True(result.ResourceNotFound);
    }

    [Fact]
    public void GetAllStrings_ReturnsEmptyEnumerable()
    {
        var localizer = NullStringLocalizer.Instance;

        var result = localizer.GetAllStrings();

        Assert.Empty(result);
    }

    [Fact]
    public void GetAllStrings_WithCulture_ReturnsEmptyEnumerable()
    {
        var localizer = NullStringLocalizer.Instance;

        var result = localizer.GetAllStrings(new CultureInfo("zh-CN"), true);

        Assert.Empty(result);
    }
}

public class NullStringLocalizerOfTTests
{
    private class TestResource { }

    [Fact]
    public void Instance_ReturnsSameInstance()
    {
        var instance1 = NullStringLocalizer<TestResource>.Instance;
        var instance2 = NullStringLocalizer<TestResource>.Instance;

        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void Indexer_ReturnsKeyAsValue()
    {
        var localizer = NullStringLocalizer<TestResource>.Instance;

        var result = localizer["Hello"];

        Assert.Equal("Hello", result.Name);
        Assert.Equal("Hello", result.Value);
        Assert.True(result.ResourceNotFound);
    }

    [Fact]
    public void Indexer_WithArguments_FormatsString()
    {
        var localizer = NullStringLocalizer<TestResource>.Instance;

        var result = localizer["Count: {0}", 42];

        Assert.Equal("Count: {0}", result.Name);
        Assert.Equal("Count: 42", result.Value);
        Assert.True(result.ResourceNotFound);
    }

    [Fact]
    public void GetAllStrings_ReturnsEmptyEnumerable()
    {
        var localizer = NullStringLocalizer<TestResource>.Instance;

        var result = localizer.GetAllStrings();

        Assert.Empty(result);
    }

    [Fact]
    public void GetAllStrings_WithIncludeParentCulturesFalse_ReturnsEmptyEnumerable()
    {
        var localizer = NullStringLocalizer<TestResource>.Instance;

        var result = localizer.GetAllStrings(includeParentCultures: false);

        Assert.Empty(result);
    }

    [Fact]
    public void DifferentResourceTypes_HaveDifferentInstances()
    {
        var localizer1 = NullStringLocalizer<TestResource>.Instance;
        var localizer2 = NullStringLocalizer<AnotherResource>.Instance;

        Assert.NotSame(localizer1, localizer2);
    }

    private class AnotherResource { }
}

