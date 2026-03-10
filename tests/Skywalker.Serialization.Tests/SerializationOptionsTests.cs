using Skywalker.Serialization;
using Xunit;

namespace Skywalker.Serialization.Tests;

public class SerializationOptionsTests
{
    [Fact]
    public void SerializationOptions_DefaultDateTimeFormat_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var options = new SerializationOptions();

        // Assert
        Assert.Equal("yyyy-MM-dd HH:mm:ss", options.DefaultDateTimeFormat);
    }

    [Fact]
    public void SerializationOptions_DefaultDateTimeFormat_ShouldBeSettable()
    {
        // Arrange
        var options = new SerializationOptions();

        // Act
        options.DefaultDateTimeFormat = "yyyy/MM/dd";

        // Assert
        Assert.Equal("yyyy/MM/dd", options.DefaultDateTimeFormat);
    }

    [Fact]
    public void SerializationOptions_DefaultDateTimeFormat_ShouldSupportISOFormat()
    {
        // Arrange
        var options = new SerializationOptions();

        // Act
        options.DefaultDateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

        // Assert
        Assert.Equal("yyyy-MM-ddTHH:mm:ss.fffZ", options.DefaultDateTimeFormat);
    }
}

