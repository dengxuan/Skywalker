using System.Text;
using Skywalker.Caching;

namespace Skywalker.Caching.Tests;

public class CachingOptionsTests
{
    [Fact]
    public void CachingOptions_DefaultEncoding_ShouldBeUTF8()
    {
        // Arrange & Act
        var options = new CachingOptions();

        // Assert
        Assert.Same(Encoding.UTF8, options.Encoding);
    }

    [Fact]
    public void CachingOptions_Encoding_ShouldBeSettable()
    {
        // Arrange
        var options = new CachingOptions();

        // Act
        options.Encoding = Encoding.ASCII;

        // Assert
        Assert.Same(Encoding.ASCII, options.Encoding);
    }

    [Fact]
    public void CachingOptions_Encoding_ShouldSupportUnicode()
    {
        // Arrange
        var options = new CachingOptions();

        // Act
        options.Encoding = Encoding.Unicode;

        // Assert
        Assert.Same(Encoding.Unicode, options.Encoding);
    }
}

