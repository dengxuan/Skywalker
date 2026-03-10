using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Skywalker.Ddd.Exceptions;
using Xunit;

namespace Skywalker.Ddd.AspNetCore.Tests.ExceptionHandling;

public class ExceptionTypesTests
{
    [Fact]
    public void EntityNotFoundException_ShouldSetEntityTypeAndId()
    {
        // Arrange & Act
        var exception = new EntityNotFoundException(typeof(string), "test-id");

        // Assert
        Assert.Equal(typeof(string), exception.EntityType);
        Assert.Equal("test-id", exception.EntityId);
        Assert.Contains("String", exception.Message);
        Assert.Contains("test-id", exception.Message);
    }

    [Fact]
    public void EntityNotFoundException_WithMessage_ShouldSetMessage()
    {
        // Arrange & Act
        var exception = new EntityNotFoundException("Custom message");

        // Assert
        Assert.Equal("Custom message", exception.Message);
    }

    [Fact]
    public void EntityNotFoundException_ShouldImplementIHasErrorCode()
    {
        // Arrange & Act
        var exception = new EntityNotFoundException("test");

        // Assert
        Assert.IsAssignableFrom<IHasErrorCode>(exception);
        Assert.Equal("Skywalker:EntityNotFound", exception.Code);
    }

    [Fact]
    public void AuthorizationException_ShouldSetMessage()
    {
        // Arrange & Act
        var exception = new AuthorizationException("Access denied");

        // Assert
        Assert.Equal("Access denied", exception.Message);
    }

    [Fact]
    public void AuthorizationException_ShouldImplementIHasLogLevel()
    {
        // Arrange & Act
        var exception = new AuthorizationException();

        // Assert
        Assert.IsAssignableFrom<IHasLogLevel>(exception);
        Assert.Equal(LogLevel.Warning, exception.LogLevel);
    }

    [Fact]
    public void AuthorizationException_ShouldImplementIHasErrorCode()
    {
        // Arrange & Act
        var exception = new AuthorizationException();

        // Assert
        Assert.IsAssignableFrom<IHasErrorCode>(exception);
        Assert.Equal("Skywalker:Authorization", exception.Code);
    }

    [Fact]
    public void SkywalkerValidationException_ShouldSetValidationErrors()
    {
        // Arrange
        var validationResults = new List<ValidationResult>
        {
            new("Name is required", ["Name"]),
            new("Email is invalid", ["Email"])
        };

        // Act
        var exception = new SkywalkerValidationException(validationResults);

        // Assert
        Assert.Equal(2, exception.ValidationErrors.Count);
        Assert.Contains(exception.ValidationErrors, v => v.ErrorMessage == "Name is required");
        Assert.Contains(exception.ValidationErrors, v => v.ErrorMessage == "Email is invalid");
    }

    [Fact]
    public void SkywalkerValidationException_ShouldImplementIHasValidationErrors()
    {
        // Arrange & Act
        var exception = new SkywalkerValidationException();

        // Assert
        Assert.IsAssignableFrom<IHasValidationErrors>(exception);
    }

    [Fact]
    public void SkywalkerValidationException_ShouldImplementIHasLogLevel()
    {
        // Arrange & Act
        var exception = new SkywalkerValidationException();

        // Assert
        Assert.IsAssignableFrom<IHasLogLevel>(exception);
        Assert.Equal(LogLevel.Warning, exception.LogLevel);
    }

    [Fact]
    public void UserFriendlyException_ShouldSetCodeAndMessage()
    {
        // Arrange & Act
        var exception = new UserFriendlyException("ERR001", "Something went wrong");

        // Assert
        Assert.Equal("ERR001", exception.Code);
        Assert.Equal("Something went wrong", exception.Message);
    }

    [Fact]
    public void SkywalkerException_ShouldSetMessage()
    {
        // Arrange & Act
        var exception = new SkywalkerException("Test exception");

        // Assert
        Assert.Equal("Test exception", exception.Message);
    }

    [Fact]
    public void SkywalkerException_ShouldSetInnerException()
    {
        // Arrange
        var inner = new InvalidOperationException("Inner");

        // Act
        var exception = new SkywalkerException("Outer", inner);

        // Assert
        Assert.Equal("Outer", exception.Message);
        Assert.Same(inner, exception.InnerException);
    }
}

