using Skywalker.Settings;

namespace Skywalker.Settings.Tests;

public class SettingDefinitionTests
{
    [Fact]
    public void SettingDefinition_ShouldSetName()
    {
        // Arrange & Act
        var definition = new SettingDefinition("TestSetting");

        // Assert
        Assert.Equal("TestSetting", definition.Name);
    }

    [Fact]
    public void SettingDefinition_ShouldSetDefaultValue()
    {
        // Arrange & Act
        var definition = new SettingDefinition("TestSetting", defaultValue: "default");

        // Assert
        Assert.Equal("default", definition.DefaultValue);
    }

    [Fact]
    public void SettingDefinition_ShouldUseNameAsDisplayName_WhenNotProvided()
    {
        // Arrange & Act
        var definition = new SettingDefinition("TestSetting");

        // Assert
        Assert.Equal("TestSetting", definition.DisplayName);
    }

    [Fact]
    public void SettingDefinition_ShouldSetDisplayName()
    {
        // Arrange & Act
        var definition = new SettingDefinition("TestSetting", displayName: "Test Display");

        // Assert
        Assert.Equal("Test Display", definition.DisplayName);
    }

    [Fact]
    public void SettingDefinition_ShouldSetDescription()
    {
        // Arrange & Act
        var definition = new SettingDefinition("TestSetting", description: "Test description");

        // Assert
        Assert.Equal("Test description", definition.Description);
    }

    [Fact]
    public void SettingDefinition_IsVisibleToClients_ShouldDefaultToFalse()
    {
        // Arrange & Act
        var definition = new SettingDefinition("TestSetting");

        // Assert
        Assert.False(definition.IsVisibleToClients);
    }

    [Fact]
    public void SettingDefinition_IsInherited_ShouldDefaultToTrue()
    {
        // Arrange & Act
        var definition = new SettingDefinition("TestSetting");

        // Assert
        Assert.True(definition.IsInherited);
    }

    [Fact]
    public void SettingDefinition_IsEncrypted_ShouldDefaultToFalse()
    {
        // Arrange & Act
        var definition = new SettingDefinition("TestSetting");

        // Assert
        Assert.False(definition.IsEncrypted);
    }

    [Fact]
    public void SettingDefinition_WithProperty_ShouldAddProperty()
    {
        // Arrange
        var definition = new SettingDefinition("TestSetting");

        // Act
        definition.WithProperty("key1", "value1");

        // Assert
        Assert.True(definition.Properties.ContainsKey("key1"));
        Assert.Equal("value1", definition.Properties["key1"]);
    }

    [Fact]
    public void SettingDefinition_WithProperty_ShouldReturnSelf_ForChaining()
    {
        // Arrange
        var definition = new SettingDefinition("TestSetting");

        // Act
        var result = definition.WithProperty("key1", "value1").WithProperty("key2", "value2");

        // Assert
        Assert.Same(definition, result);
        Assert.Equal(2, definition.Properties.Count);
    }

    [Fact]
    public void SettingDefinition_WithProviders_ShouldAddProviders()
    {
        // Arrange
        var definition = new SettingDefinition("TestSetting");

        // Act
        definition.WithProviders("Provider1", "Provider2");

        // Assert
        Assert.Contains("Provider1", definition.Providers);
        Assert.Contains("Provider2", definition.Providers);
    }

    [Fact]
    public void SettingDefinition_WithProviders_ShouldReturnSelf_ForChaining()
    {
        // Arrange
        var definition = new SettingDefinition("TestSetting");

        // Act
        var result = definition.WithProviders("Provider1").WithProviders("Provider2");

        // Assert
        Assert.Same(definition, result);
        Assert.Equal(2, definition.Providers.Count);
    }
}

