using System.Data;
using Skywalker.Ddd.Uow;
using Xunit;

namespace Skywalker.Ddd.AspNetCore.Tests;

public class UnitOfWorkAttributeTests
{
    [Fact]
    public void UnitOfWorkAttribute_DefaultConstructor_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var attribute = new UnitOfWorkAttribute();

        // Assert
        Assert.Null(attribute.IsTransactional);
        Assert.Null(attribute.Timeout);
        Assert.Null(attribute.IsolationLevel);
        Assert.False(attribute.IsDisabled);
    }

    [Fact]
    public void UnitOfWorkAttribute_WithIsTransactional_ShouldSetValue()
    {
        // Arrange & Act
        var attribute = new UnitOfWorkAttribute(true);

        // Assert
        Assert.True(attribute.IsTransactional);
    }

    [Fact]
    public void UnitOfWorkAttribute_WithIsTransactionalFalse_ShouldSetValue()
    {
        // Arrange & Act
        var attribute = new UnitOfWorkAttribute(false);

        // Assert
        Assert.False(attribute.IsTransactional);
    }

    [Fact]
    public void UnitOfWorkAttribute_WithIsolationLevel_ShouldSetValues()
    {
        // Arrange & Act
        var attribute = new UnitOfWorkAttribute(true, IsolationLevel.ReadCommitted);

        // Assert
        Assert.True(attribute.IsTransactional);
        Assert.Equal(IsolationLevel.ReadCommitted, attribute.IsolationLevel);
    }

    [Fact]
    public void UnitOfWorkAttribute_WithTimeout_ShouldSetValues()
    {
        // Arrange & Act
        var attribute = new UnitOfWorkAttribute(true, IsolationLevel.Serializable, 30000);

        // Assert
        Assert.True(attribute.IsTransactional);
        Assert.Equal(IsolationLevel.Serializable, attribute.IsolationLevel);
        Assert.Equal(30000, attribute.Timeout);
    }

    [Fact]
    public void UnitOfWorkAttribute_SetOptions_ShouldSetIsTransactional()
    {
        // Arrange
        var attribute = new UnitOfWorkAttribute(true);
        var options = new UnitOfWorkOptions();

        // Act
        attribute.SetOptions(options);

        // Assert
        Assert.True(options.IsTransactional);
    }

    [Fact]
    public void UnitOfWorkAttribute_SetOptions_ShouldSetTimeout()
    {
        // Arrange
        var attribute = new UnitOfWorkAttribute { Timeout = 5000 };
        var options = new UnitOfWorkOptions();

        // Act
        attribute.SetOptions(options);

        // Assert
        Assert.Equal(5000, options.Timeout);
    }

    [Fact]
    public void UnitOfWorkAttribute_SetOptions_ShouldSetIsolationLevel()
    {
        // Arrange
        var attribute = new UnitOfWorkAttribute { IsolationLevel = IsolationLevel.Snapshot };
        var options = new UnitOfWorkOptions();

        // Act
        attribute.SetOptions(options);

        // Assert
        Assert.Equal(IsolationLevel.Snapshot, options.IsolationLevel);
    }

    [Fact]
    public void UnitOfWorkAttribute_SetOptions_ShouldNotOverrideWhenNull()
    {
        // Arrange
        var attribute = new UnitOfWorkAttribute();
        var options = new UnitOfWorkOptions { IsTransactional = true, Timeout = 1000 };

        // Act
        attribute.SetOptions(options);

        // Assert
        Assert.True(options.IsTransactional);
        Assert.Equal(1000, options.Timeout);
    }

    [Fact]
    public void UnitOfWorkAttribute_IsDisabled_ShouldBeSettable()
    {
        // Arrange & Act
        var attribute = new UnitOfWorkAttribute { IsDisabled = true };

        // Assert
        Assert.True(attribute.IsDisabled);
    }

    [Fact]
    public void UnitOfWorkAttribute_ShouldBeApplicableToMethodClassAndInterface()
    {
        // Arrange
        var attribute = typeof(UnitOfWorkAttribute).GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .FirstOrDefault() as AttributeUsageAttribute;

        // Assert
        Assert.NotNull(attribute);
        Assert.True((attribute!.ValidOn & AttributeTargets.Method) == AttributeTargets.Method);
        Assert.True((attribute.ValidOn & AttributeTargets.Class) == AttributeTargets.Class);
        Assert.True((attribute.ValidOn & AttributeTargets.Interface) == AttributeTargets.Interface);
    }
}

