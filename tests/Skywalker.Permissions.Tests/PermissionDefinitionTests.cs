using Skywalker.Permissions;

namespace Skywalker.Permissions.Tests;

public class PermissionDefinitionTests
{
    [Fact]
    public void PermissionDefinition_ShouldSetName()
    {
        // Arrange & Act
        var permission = new PermissionDefinition("TestPermission", "Test Permission");

        // Assert
        Assert.Equal("TestPermission", permission.Name);
    }

    [Fact]
    public void PermissionDefinition_ShouldSetDisplayName()
    {
        // Arrange & Act
        var permission = new PermissionDefinition("TestPermission", "Test Display Name");

        // Assert
        Assert.Equal("Test Display Name", permission.DisplayName);
    }

    [Fact]
    public void PermissionDefinition_IsEnabled_ShouldDefaultToTrue()
    {
        // Arrange & Act
        var permission = new PermissionDefinition("TestPermission", "Test");

        // Assert
        Assert.True(permission.IsEnabled);
    }

    [Fact]
    public void PermissionDefinition_IsEnabled_ShouldBeSettable()
    {
        // Arrange & Act
        var permission = new PermissionDefinition("TestPermission", "Test", isEnabled: false);

        // Assert
        Assert.False(permission.IsEnabled);
    }

    [Fact]
    public void PermissionDefinition_AddChild_ShouldAddChildPermission()
    {
        // Arrange
        var parent = new PermissionDefinition("Parent", "Parent Permission");

        // Act
        var child = parent.AddChild("Child", "Child Permission");

        // Assert
        Assert.Single(parent.Children);
        Assert.Same(child, parent.Children[0]);
        Assert.Same(parent, child.Parent);
    }

    [Fact]
    public void PermissionDefinition_AddChild_ShouldSupportMultipleChildren()
    {
        // Arrange
        var parent = new PermissionDefinition("Parent", "Parent Permission");

        // Act
        parent.AddChild("Child1", "Child 1");
        parent.AddChild("Child2", "Child 2");

        // Assert
        Assert.Equal(2, parent.Children.Count);
    }

    [Fact]
    public void PermissionDefinition_WithProperty_ShouldAddProperty()
    {
        // Arrange
        var permission = new PermissionDefinition("TestPermission", "Test");

        // Act
        permission.WithProperty("key1", "value1");

        // Assert
        Assert.True(permission.Properties.ContainsKey("key1"));
        Assert.Equal("value1", permission.Properties["key1"]);
    }

    [Fact]
    public void PermissionDefinition_WithProperty_ShouldReturnSelf_ForChaining()
    {
        // Arrange
        var permission = new PermissionDefinition("TestPermission", "Test");

        // Act
        var result = permission.WithProperty("key1", "value1").WithProperty("key2", "value2");

        // Assert
        Assert.Same(permission, result);
        Assert.Equal(2, permission.Properties.Count);
    }

    [Fact]
    public void PermissionDefinition_Indexer_ShouldGetAndSetProperty()
    {
        // Arrange
        var permission = new PermissionDefinition("TestPermission", "Test");

        // Act
        permission["key1"] = "value1";

        // Assert
        Assert.Equal("value1", permission["key1"]);
    }

    [Fact]
    public void PermissionDefinition_Indexer_ShouldReturnNull_WhenPropertyNotExists()
    {
        // Arrange
        var permission = new PermissionDefinition("TestPermission", "Test");

        // Assert
        Assert.Null(permission["nonexistent"]);
    }

    [Fact]
    public void PermissionDefinition_ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var permission = new PermissionDefinition("TestPermission", "Test");

        // Act
        var result = permission.ToString();

        // Assert
        Assert.Contains("PermissionDefinition", result);
        Assert.Contains("TestPermission", result);
    }
}

