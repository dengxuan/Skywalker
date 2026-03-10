using FluentValidation;
using Skywalker.Validation.FluentValidation;
using Xunit;

namespace Skywalker.Validation.Tests;

public class FluentValidationTests
{
    [Fact]
    public void Validate_ValidObject_ReturnsSuccess()
    {
        var fluentValidator = new TestModelValidator();
        var adapter = new FluentValidationValidatorAdapter<TestModel>(fluentValidator);
        var model = new TestModel { Name = "Test", Age = 25 };

        var result = adapter.Validate(model);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_InvalidObject_ReturnsErrors()
    {
        var fluentValidator = new TestModelValidator();
        var adapter = new FluentValidationValidatorAdapter<TestModel>(fluentValidator);
        var model = new TestModel { Name = "", Age = -1 };

        var result = adapter.Validate(model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
        Assert.Contains(result.Errors, e => e.PropertyName == "Age");
    }

    [Fact]
    public async Task ValidateAsync_ValidObject_ReturnsSuccess()
    {
        var fluentValidator = new TestModelValidator();
        var adapter = new FluentValidationValidatorAdapter<TestModel>(fluentValidator);
        var model = new TestModel { Name = "Test", Age = 25 };

        var result = await adapter.ValidateAsync(model);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_PreservesErrorCode()
    {
        var fluentValidator = new TestModelValidator();
        var adapter = new FluentValidationValidatorAdapter<TestModel>(fluentValidator);
        var model = new TestModel { Name = "", Age = 25 };

        var result = adapter.Validate(model);

        Assert.Contains(result.Errors, e => e.ErrorCode == "NotEmptyValidator");
    }

    public class TestModel
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    public class TestModelValidator : AbstractValidator<TestModel>
    {
        public TestModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Age).GreaterThanOrEqualTo(0);
        }
    }
}

