using Skywalker.Validation;
using Xunit;

namespace Skywalker.Validation.Tests;

public class ValidationErrorTests
{
    [Fact]
    public void Constructor_SetsPropertyNameAndMessage()
    {
        var error = new ValidationError("Name", "Name is required");

        Assert.Equal("Name", error.PropertyName);
        Assert.Equal("Name is required", error.ErrorMessage);
    }

    [Fact]
    public void Constructor_WithErrorCode_SetsAllProperties()
    {
        var error = new ValidationError("Email", "Invalid email", "INVALID_EMAIL");

        Assert.Equal("Email", error.PropertyName);
        Assert.Equal("Invalid email", error.ErrorMessage);
        Assert.Equal("INVALID_EMAIL", error.ErrorCode);
    }

    [Fact]
    public void Severity_DefaultsToError()
    {
        var error = new ValidationError("Name", "Name is required");

        Assert.Equal(ValidationSeverity.Error, error.Severity);
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        var error = new ValidationError("Name", "Name is required");

        Assert.Equal("Name: Name is required", error.ToString());
    }
}

