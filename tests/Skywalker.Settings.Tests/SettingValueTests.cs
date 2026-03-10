using Skywalker.Settings;

namespace Skywalker.Settings.Tests;

public class SettingValueTests
{
    [Fact]
    public void SettingValue_ShouldSetName()
    {
        // Arrange & Act
        var value = new SettingValue("TestName", "TestValue");

        // Assert
        Assert.Equal("TestName", value.Name);
    }

    [Fact]
    public void SettingValue_ShouldSetValue()
    {
        // Arrange & Act
        var value = new SettingValue("TestName", "TestValue");

        // Assert
        Assert.Equal("TestValue", value.Value);
    }

    [Fact]
    public void SettingValue_ShouldAllowNullValue()
    {
        // Arrange & Act
        var value = new SettingValue("TestName", null);

        // Assert
        Assert.Null(value.Value);
    }

    [Fact]
    public void SettingValue_ShouldBeSerializable()
    {
        // Arrange
        var type = typeof(SettingValue);

        // Assert
        Assert.NotNull(type.GetCustomAttributes(typeof(SerializableAttribute), false).FirstOrDefault());
    }
}

