using Skywalker.Extensions.Collections;
using Skywalker.Extensions.Collections.Generic;
using Xunit;

namespace Skywalker.Extensions.Universal.Tests;

public class NameValueTests
{
    [Fact]
    public void NameValue_DefaultConstructor_ShouldCreateInstance()
    {
        // Arrange & Act
        var nameValue = new NameValue();

        // Assert
        Assert.NotNull(nameValue);
        Assert.Null(nameValue.Name);
        Assert.Null(nameValue.Value);
    }

    [Fact]
    public void NameValue_WithParameters_ShouldSetValues()
    {
        // Arrange & Act
        var nameValue = new NameValue("key", "value");

        // Assert
        Assert.Equal("key", nameValue.Name);
        Assert.Equal("value", nameValue.Value);
    }

    [Fact]
    public void NameValue_Name_ShouldBeSettable()
    {
        // Arrange
        var nameValue = new NameValue();

        // Act
        nameValue.Name = "testName";

        // Assert
        Assert.Equal("testName", nameValue.Name);
    }

    [Fact]
    public void NameValue_Value_ShouldBeSettable()
    {
        // Arrange
        var nameValue = new NameValue();

        // Act
        nameValue.Value = "testValue";

        // Assert
        Assert.Equal("testValue", nameValue.Value);
    }

    [Fact]
    public void NameValueGeneric_DefaultConstructor_ShouldCreateInstance()
    {
        // Arrange & Act
        var nameValue = new NameValue<int>();

        // Assert
        Assert.NotNull(nameValue);
        Assert.Null(nameValue.Name);
        Assert.Equal(0, nameValue.Value);
    }

    [Fact]
    public void NameValueGeneric_WithParameters_ShouldSetValues()
    {
        // Arrange & Act
        var nameValue = new NameValue<int>("count", 42);

        // Assert
        Assert.Equal("count", nameValue.Name);
        Assert.Equal(42, nameValue.Value);
    }

    [Fact]
    public void NameValueGeneric_WithNullableType_ShouldSupportNull()
    {
        // Arrange & Act
        var nameValue = new NameValue<int?>("nullable", null);

        // Assert
        Assert.Equal("nullable", nameValue.Name);
        Assert.Null(nameValue.Value);
    }

    [Fact]
    public void NameValueGeneric_WithComplexType_ShouldWork()
    {
        // Arrange
        var complexValue = new { Id = 1, Name = "Test" };

        // Act
        var nameValue = new NameValue<object>("complex", complexValue);

        // Assert
        Assert.Equal("complex", nameValue.Name);
        Assert.Same(complexValue, nameValue.Value);
    }

    [Fact]
    public void NameValue_ShouldBeSerializable()
    {
        // Arrange
        var type = typeof(NameValue);

        // Assert
        Assert.NotEmpty(type.GetCustomAttributes(typeof(SerializableAttribute), false));
    }

    [Fact]
    public void NameValueGeneric_ShouldBeSerializable()
    {
        // Arrange
        var type = typeof(NameValue<>);

        // Assert
        Assert.NotEmpty(type.GetCustomAttributes(typeof(SerializableAttribute), false));
    }
}

