using NSubstitute;
using Skywalker.Caching.Abstractions;

namespace Skywalker.Caching.Tests;

public class CachingProviderTests
{
    private class TestCachingProvider : CachingProvider
    {
        public int CreateCount { get; private set; }

        protected override ICaching CreateCacheImplementation(string name)
        {
            CreateCount++;
            var cache = Substitute.For<ICaching>();
            cache.Name.Returns(name);
            return cache;
        }
    }

    [Fact]
    public void GetCaching_ShouldReturnCache()
    {
        // Arrange
        var provider = new TestCachingProvider();

        // Act
        var cache = provider.GetCaching("test-cache");

        // Assert
        Assert.NotNull(cache);
        Assert.Equal("test-cache", cache.Name);
    }

    [Fact]
    public void GetCaching_ShouldReturnSameInstance_ForSameName()
    {
        // Arrange
        var provider = new TestCachingProvider();

        // Act
        var cache1 = provider.GetCaching("test-cache");
        var cache2 = provider.GetCaching("test-cache");

        // Assert
        Assert.Same(cache1, cache2);
        Assert.Equal(1, provider.CreateCount);
    }

    [Fact]
    public void GetCaching_ShouldReturnDifferentInstances_ForDifferentNames()
    {
        // Arrange
        var provider = new TestCachingProvider();

        // Act
        var cache1 = provider.GetCaching("cache-1");
        var cache2 = provider.GetCaching("cache-2");

        // Assert
        Assert.NotSame(cache1, cache2);
        Assert.Equal("cache-1", cache1.Name);
        Assert.Equal("cache-2", cache2.Name);
        Assert.Equal(2, provider.CreateCount);
    }

    [Fact]
    public void GetCaching_ShouldThrowArgumentNullException_WhenNameIsNull()
    {
        // Arrange
        var provider = new TestCachingProvider();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => provider.GetCaching(null!));
    }

    [Fact]
    public void GetCaching_ShouldThrowArgumentNullException_WhenNameIsEmpty()
    {
        // Arrange
        var provider = new TestCachingProvider();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => provider.GetCaching(string.Empty));
    }
}

