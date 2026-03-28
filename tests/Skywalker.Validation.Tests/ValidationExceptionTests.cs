using Skywalker.Validation;
using Xunit;

namespace Skywalker.Validation.Tests;

public class ValidationExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        var ex = new ValidationException("Something wrong");
        Assert.Equal("Something wrong", ex.Message);
        Assert.Empty(ex.Errors);
    }

    [Fact]
    public void Constructor_WithErrors_BuildsMessage()
    {
        var errors = new[]
        {
            new ValidationError("Name", "Required"),
            new ValidationError("Email", "Invalid")
        };
        var ex = new ValidationException(errors);
        Assert.Equal(2, ex.Errors.Count);
        Assert.Contains("2 errors", ex.Message);
    }

    [Fact]
    public void Constructor_WithSingleError_BuildsMessage()
    {
        var errors = new[] { new ValidationError("Name", "Required") };
        var ex = new ValidationException(errors);
        Assert.Single(ex.Errors);
        Assert.Contains("Required", ex.Message);
    }

    [Fact]
    public void Constructor_WithEmptyErrors_DefaultMessage()
    {
        var ex = new ValidationException(Array.Empty<ValidationError>());
        Assert.Equal("Validation failed", ex.Message);
    }

    [Fact]
    public void Constructor_WithPropertyAndMessage()
    {
        var ex = new ValidationException("Name", "Name is required");
        Assert.Single(ex.Errors);
        Assert.Equal("Name", ex.Errors[0].PropertyName);
    }

    [Fact]
    public void Constructor_WithValidationResult()
    {
        var result = ValidationResult.Failure("Email", "Invalid");
        var ex = new ValidationException(result);
        Assert.Single(ex.Errors);
    }
}
