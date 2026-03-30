using Skywalker.Validation;
using Xunit;

namespace Skywalker.Validation.Tests;

public class CompositeValidatorTests
{
    private class AlwaysValidValidator : IValidator
    {
        public IValidationResult Validate(object instance) => ValidationResult.Success();
        public Task<IValidationResult> ValidateAsync(object instance, CancellationToken cancellationToken = default)
            => Task.FromResult<IValidationResult>(ValidationResult.Success());
    }

    private class AlwaysFailValidator : IValidator
    {
        public IValidationResult Validate(object instance)
            => ValidationResult.Failure(new[] { new ValidationError("Field", "Error") });
        public Task<IValidationResult> ValidateAsync(object instance, CancellationToken cancellationToken = default)
            => Task.FromResult<IValidationResult>(ValidationResult.Failure(new[] { new ValidationError("Field", "Error") }));
    }

    [Fact]
    public void Validate_AllValid_ReturnsSuccess()
    {
        var composite = new CompositeValidator(new IValidator[]
        {
            new AlwaysValidValidator(),
            new AlwaysValidValidator()
        });

        var result = composite.Validate(new object());

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_OneInvalid_ReturnsFailure()
    {
        var composite = new CompositeValidator(new IValidator[]
        {
            new AlwaysValidValidator(),
            new AlwaysFailValidator()
        });

        var result = composite.Validate(new object());

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void Validate_MultipleInvalid_AggregatesErrors()
    {
        var composite = new CompositeValidator(new IValidator[]
        {
            new AlwaysFailValidator(),
            new AlwaysFailValidator()
        });

        var result = composite.Validate(new object());

        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count());
    }

    [Fact]
    public void Validate_EmptyValidators_ReturnsSuccess()
    {
        var composite = new CompositeValidator(Enumerable.Empty<IValidator>());

        var result = composite.Validate(new object());

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_AllValid_ReturnsSuccess()
    {
        var composite = new CompositeValidator(new IValidator[]
        {
            new AlwaysValidValidator(),
            new AlwaysValidValidator()
        });

        var result = await composite.ValidateAsync(new object());

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_OneInvalid_ReturnsFailure()
    {
        var composite = new CompositeValidator(new IValidator[]
        {
            new AlwaysValidValidator(),
            new AlwaysFailValidator()
        });

        var result = await composite.ValidateAsync(new object());

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
    }
}
