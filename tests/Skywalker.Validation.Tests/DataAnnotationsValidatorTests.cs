using System.ComponentModel.DataAnnotations;
using Skywalker.Validation;
using Xunit;

namespace Skywalker.Validation.Tests;

public class DataAnnotationsValidatorTests
{
    private readonly DataAnnotationsValidator _validator = new();

    [Fact]
    public void Validate_ValidObject_ReturnsSuccess()
    {
        var model = new TestModel { Name = "Test", Email = "test@example.com" };

        var result = _validator.Validate(model);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_MissingRequiredField_ReturnsError()
    {
        var model = new TestModel { Email = "test@example.com" };

        var result = _validator.Validate(model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_InvalidEmail_ReturnsError()
    {
        var model = new TestModel { Name = "Test", Email = "invalid" };

        var result = _validator.Validate(model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task ValidateAsync_ValidObject_ReturnsSuccess()
    {
        var model = new TestModel { Name = "Test", Email = "test@example.com" };

        var result = await _validator.ValidateAsync(model);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_MultipleErrors_ReturnsAllErrors()
    {
        var model = new TestModel();

        var result = _validator.Validate(model);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 2);
    }

    public class TestModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }
    }
}

