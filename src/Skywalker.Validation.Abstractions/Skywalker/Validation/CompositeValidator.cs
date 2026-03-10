namespace Skywalker.Validation;

/// <summary>
/// A composite validator that combines multiple validators.
/// </summary>
public class CompositeValidator : IValidator
{
    private readonly IEnumerable<IValidator> _validators;

    /// <summary>
    /// Creates a new composite validator.
    /// </summary>
    /// <param name="validators">The validators to combine.</param>
    public CompositeValidator(IEnumerable<IValidator> validators)
    {
        _validators = validators;
    }

    public IValidationResult Validate(object instance)
    {
        var allErrors = new List<ValidationError>();

        foreach (var validator in _validators)
        {
            var result = validator.Validate(instance);
            if (!result.IsValid)
            {
                allErrors.AddRange(result.Errors);
            }
        }

        return allErrors.Count > 0
            ? ValidationResult.Failure(allErrors)
            : ValidationResult.Success();
    }

    public async Task<IValidationResult> ValidateAsync(object instance, CancellationToken cancellationToken = default)
    {
        var allErrors = new List<ValidationError>();

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(instance, cancellationToken);
            if (!result.IsValid)
            {
                allErrors.AddRange(result.Errors);
            }
        }

        return allErrors.Count > 0
            ? ValidationResult.Failure(allErrors)
            : ValidationResult.Success();
    }
}

