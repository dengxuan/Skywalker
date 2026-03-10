using Microsoft.Extensions.Localization;
using Skywalker.Template;
using Xunit;

namespace Skywalker.Template.Tests;

public class TemplateDefinitionTests
{
    [Fact]
    public void TemplateDefinition_ShouldSetName()
    {
        // Arrange & Act
        var template = new TemplateDefinition("TestTemplate");

        // Assert
        Assert.Equal("TestTemplate", template.Name);
    }

    [Fact]
    public void TemplateDefinition_DisplayName_ShouldDefaultToName()
    {
        // Arrange & Act
        var template = new TemplateDefinition("TestTemplate");

        // Assert
        Assert.Equal("TestTemplate", template.DisplayName.Value);
    }

    [Fact]
    public void TemplateDefinition_ShouldSetDisplayName()
    {
        // Arrange
        var displayName = new LocalizedString("TestTemplate", "Test Template Display");

        // Act
        var template = new TemplateDefinition("TestTemplate", displayName: displayName);

        // Assert
        Assert.Equal("Test Template Display", template.DisplayName.Value);
    }

    [Fact]
    public void TemplateDefinition_IsLayout_ShouldDefaultToFalse()
    {
        // Arrange & Act
        var template = new TemplateDefinition("TestTemplate");

        // Assert
        Assert.False(template.IsLayout);
    }

    [Fact]
    public void TemplateDefinition_IsLayout_ShouldBeSettable()
    {
        // Arrange & Act
        var template = new TemplateDefinition("TestTemplate", isLayout: true);

        // Assert
        Assert.True(template.IsLayout);
    }

    [Fact]
    public void TemplateDefinition_Layout_ShouldBeSettable()
    {
        // Arrange & Act
        var template = new TemplateDefinition("TestTemplate", layout: "MainLayout");

        // Assert
        Assert.Equal("MainLayout", template.Layout);
    }

    [Fact]
    public void TemplateDefinition_DefaultCultureName_ShouldBeSettable()
    {
        // Arrange & Act
        var template = new TemplateDefinition("TestTemplate", defaultCultureName: "en-US");

        // Assert
        Assert.Equal("en-US", template.DefaultCultureName);
    }

    [Fact]
    public void TemplateDefinition_WithProperty_ShouldAddProperty()
    {
        // Arrange
        var template = new TemplateDefinition("TestTemplate");

        // Act
        template.WithProperty("key1", "value1");

        // Assert
        Assert.True(template.Properties.ContainsKey("key1"));
        Assert.Equal("value1", template.Properties["key1"]);
    }

    [Fact]
    public void TemplateDefinition_WithProperty_ShouldReturnSelf_ForChaining()
    {
        // Arrange
        var template = new TemplateDefinition("TestTemplate");

        // Act
        var result = template.WithProperty("key1", "value1").WithProperty("key2", "value2");

        // Assert
        Assert.Same(template, result);
        Assert.Equal(2, template.Properties.Count);
    }

    [Fact]
    public void TemplateDefinition_WithRenderEngine_ShouldSetRenderEngine()
    {
        // Arrange
        var template = new TemplateDefinition("TestTemplate");

        // Act
        template.WithRenderEngine("Scriban");

        // Assert
        Assert.Equal("Scriban", template.RenderEngine);
    }

    [Fact]
    public void TemplateDefinition_WithRenderEngine_ShouldReturnSelf_ForChaining()
    {
        // Arrange
        var template = new TemplateDefinition("TestTemplate");

        // Act
        var result = template.WithRenderEngine("Scriban").WithProperty("key", "value");

        // Assert
        Assert.Same(template, result);
    }

    [Fact]
    public void TemplateDefinition_Indexer_ShouldGetAndSetProperty()
    {
        // Arrange
        var template = new TemplateDefinition("TestTemplate");

        // Act
        template["key1"] = "value1";

        // Assert
        Assert.Equal("value1", template["key1"]);
    }

    [Fact]
    public void TemplateDefinition_Indexer_ShouldReturnNull_WhenPropertyNotExists()
    {
        // Arrange
        var template = new TemplateDefinition("TestTemplate");

        // Assert
        Assert.Null(template["nonexistent"]);
    }

    [Fact]
    public void TemplateDefinition_MaxNameLength_ShouldBe128()
    {
        // Assert
        Assert.Equal(128, TemplateDefinition.MaxNameLength);
    }
}

