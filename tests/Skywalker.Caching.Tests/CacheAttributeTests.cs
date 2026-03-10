using Skywalker.Caching.Abstractions;

namespace Skywalker.Caching.Tests;

public class CacheAttributeTests
{
    [Fact]
    public void CacheAttribute_ShouldSetKey()
    {
        // Arrange & Act
        var attr = new CacheAttribute("test-key");

        // Assert
        Assert.Equal("test-key", attr.Key);
    }

    [Fact]
    public void CacheAttribute_ShouldSetExpiry()
    {
        // Arrange & Act
        var attr = new CacheAttribute("test-key") { Expiry = 3600 };

        // Assert
        Assert.Equal(3600, attr.Expiry);
    }

    [Fact]
    public void CacheAttribute_DefaultExpiry_ShouldBeZero()
    {
        // Arrange & Act
        var attr = new CacheAttribute("test-key");

        // Assert
        Assert.Equal(0, attr.Expiry);
    }

    [Cache("my-cache")]
    private class TestCacheClass { }

    [Fact]
    public void CacheAttribute_ShouldBeApplicableToClass()
    {
        // Arrange
        var type = typeof(TestCacheClass);

        // Act
        var attrs = type.GetCustomAttributes(typeof(CacheAttribute), false);

        // Assert
        Assert.Single(attrs);
        var attr = (CacheAttribute)attrs[0];
        Assert.Equal("my-cache", attr.Key);
    }
}

