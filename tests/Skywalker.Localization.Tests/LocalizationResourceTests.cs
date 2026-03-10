using Skywalker.Localization;
using Xunit;

namespace Skywalker.Localization.Tests;

// Dummy resource types for testing
public class TestResource { }
public class BaseResource { }
public class AnotherBaseResource { }

public class LocalizationResourceTests
{
    [Fact]
    public void Constructor_SetsResourceType()
    {
        var resource = new LocalizationResource(typeof(TestResource));

        Assert.Equal(typeof(TestResource), resource.ResourceType);
        Assert.Equal(typeof(TestResource).FullName, resource.ResourceName);
    }

    [Fact]
    public void Constructor_SetsDefaultCultureName()
    {
        var resource = new LocalizationResource(typeof(TestResource), "en");

        Assert.Equal("en", resource.DefaultCultureName);
    }

    [Fact]
    public void Constructor_InitializesEmptyLists()
    {
        var resource = new LocalizationResource(typeof(TestResource));

        Assert.NotNull(resource.BaseResourceTypes);
        Assert.Empty(resource.BaseResourceTypes);
        Assert.NotNull(resource.Contributors);
        Assert.Empty(resource.Contributors);
    }

    [Fact]
    public void AddBaseTypes_Generic_AddsType()
    {
        var resource = new LocalizationResource(typeof(TestResource));

        resource.AddBaseTypes<BaseResource>();

        Assert.Single(resource.BaseResourceTypes);
        Assert.Contains(typeof(BaseResource), resource.BaseResourceTypes);
    }

    [Fact]
    public void AddBaseTypes_Multiple_AddsAllTypes()
    {
        var resource = new LocalizationResource(typeof(TestResource));

        resource.AddBaseTypes(typeof(BaseResource), typeof(AnotherBaseResource));

        Assert.Equal(2, resource.BaseResourceTypes.Count);
        Assert.Contains(typeof(BaseResource), resource.BaseResourceTypes);
        Assert.Contains(typeof(AnotherBaseResource), resource.BaseResourceTypes);
    }

    [Fact]
    public void AddBaseTypes_DoesNotAddDuplicates()
    {
        var resource = new LocalizationResource(typeof(TestResource));

        resource.AddBaseTypes<BaseResource>();
        resource.AddBaseTypes<BaseResource>();

        Assert.Single(resource.BaseResourceTypes);
    }

    [Fact]
    public void AddBaseTypes_ReturnsResourceForChaining()
    {
        var resource = new LocalizationResource(typeof(TestResource));

        var result = resource.AddBaseTypes<BaseResource>();

        Assert.Same(resource, result);
    }

    [Fact]
    public void Constructor_ThrowsOnNullResourceType()
    {
        Assert.Throws<ArgumentNullException>(() => new LocalizationResource(null!));
    }
}

