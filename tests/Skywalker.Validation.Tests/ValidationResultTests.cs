using Skywalker.Validation;
using Xunit;

namespace Skywalker.Validation.Tests;

public class ValidationResultTests
{
    [Fact]
    public void Success_ReturnsValidResult()
    {
        var result = ValidationResult.Success();

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Failure_WithSingleError_ReturnsInvalidResult()
    {
        var result = ValidationResult.Failure("Name", "Name is required");

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Name", result.Errors[0].PropertyName);
    }

    [Fact]
    public void Failure_WithMultipleErrors_ReturnsAllErrors()
    {
        var errors = new[]
        {
            new ValidationError("Name", "Name is required"),
            new ValidationError("Email", "Email is invalid")
        };
        var result = ValidationResult.Failure(errors);

        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
    }

    [Fact]
    public void AddError_AddsErrorToResult()
    {
        var result = new ValidationResult();
        result.AddError(new ValidationError("Name", "Name is required"));

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void IsValid_WithOnlyWarnings_ReturnsTrue()
    {
        var result = new ValidationResult();
        result.AddError(new ValidationError("Name", "Consider providing a name")
        {
            Severity = ValidationSeverity.Warning
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ToString_WhenValid_ReturnsSuccessMessage()
    {
        var result = ValidationResult.Success();

        Assert.Equal("Validation succeeded", result.ToString());
    }

    [Fact]
    public void ToString_WhenInvalid_ReturnsErrorMessage()
    {
        var result = ValidationResult.Failure("Name", "Name is required");

        Assert.Contains("Validation failed", result.ToString());
        Assert.Contains("Name: Name is required", result.ToString());
    }
}

