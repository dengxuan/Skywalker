using Skywalker;

namespace Skywalker.SourceGenerators.Tests;

/// <summary>
/// Tests for <see cref="SkywalkerModuleAttribute"/>.
/// </summary>
public class SkywalkerModuleAttributeTests
{
    [Fact]
    public void Constructor_WithValidModuleName_SetsModuleName()
    {
        // Arrange & Act
        var attribute = new SkywalkerModuleAttribute("Security");

        // Assert
        Assert.Equal("Security", attribute.ModuleName);
    }

    [Fact]
    public void Constructor_WithNullModuleName_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SkywalkerModuleAttribute(null!));
    }

    [Fact]
    public void Constructor_WithEmptyModuleName_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SkywalkerModuleAttribute(""));
    }

    [Fact]
    public void Constructor_WithWhitespaceModuleName_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SkywalkerModuleAttribute("   "));
    }

    [Fact]
    public void Attribute_HasCorrectUsage()
    {
        // Arrange
        var attributeType = typeof(SkywalkerModuleAttribute);

        // Act
        var usageAttribute = (AttributeUsageAttribute?)Attribute.GetCustomAttribute(
            attributeType, typeof(AttributeUsageAttribute));

        // Assert
        Assert.NotNull(usageAttribute);
        Assert.Equal(AttributeTargets.Assembly, usageAttribute.ValidOn);
        Assert.False(usageAttribute.AllowMultiple);
        Assert.False(usageAttribute.Inherited);
    }

    [Theory]
    [InlineData("Security")]
    [InlineData("RedisCaching")]
    [InlineData("EntityFrameworkCore")]
    [InlineData("EventBusLocal")]
    public void Constructor_WithVariousModuleNames_SetsModuleName(string moduleName)
    {
        // Arrange & Act
        var attribute = new SkywalkerModuleAttribute(moduleName);

        // Assert
        Assert.Equal(moduleName, attribute.ModuleName);
    }
}

