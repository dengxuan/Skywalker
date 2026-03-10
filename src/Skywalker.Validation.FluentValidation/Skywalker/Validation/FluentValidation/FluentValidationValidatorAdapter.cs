using FV = FluentValidation;

namespace Skywalker.Validation.FluentValidation;

/// <summary>
/// Adapter that wraps a FluentValidation validator.
/// </summary>
/// <typeparam name="T">The type of object to validate.</typeparam>
public class FluentValidationValidatorAdapter<T> : IValidator<T> where T : class
{
    private readonly FV.IValidator<T> _fluentValidator;

    /// <summary>
    /// Creates a new adapter.
    /// </summary>
    /// <param name="fluentValidator">The FluentValidation validator.</param>
    public FluentValidationValidatorAdapter(FV.IValidator<T> fluentValidator)
    {
        _fluentValidator = fluentValidator;
    }

    public IValidationResult Validate(T instance)
    {
        var result = _fluentValidator.Validate(instance);
        return ConvertResult(result);
    }

    public IValidationResult Validate(object instance)
    {
        if (instance is T typedInstance)
        {
            return Validate(typedInstance);
        }

        throw new ArgumentException($"Expected instance of type {typeof(T).Name}, but got {instance?.GetType().Name ?? "null"}");
    }

    public async Task<IValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default)
    {
        var result = await _fluentValidator.ValidateAsync(instance, cancellationToken);
        return ConvertResult(result);
    }

    public Task<IValidationResult> ValidateAsync(object instance, CancellationToken cancellationToken = default)
    {
        if (instance is T typedInstance)
        {
            return ValidateAsync(typedInstance, cancellationToken);
        }

        throw new ArgumentException($"Expected instance of type {typeof(T).Name}, but got {instance?.GetType().Name ?? "null"}");
    }

    private static IValidationResult ConvertResult(FV.Results.ValidationResult result)
    {
        if (result.IsValid)
        {
            return ValidationResult.Success();
        }

        var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage, e.ErrorCode)
        {
            AttemptedValue = e.AttemptedValue,
            Severity = ConvertSeverity(e.Severity)
        });

        return ValidationResult.Failure(errors);
    }

    private static ValidationSeverity ConvertSeverity(FV.Severity severity)
    {
        return severity switch
        {
            FV.Severity.Error => ValidationSeverity.Error,
            FV.Severity.Warning => ValidationSeverity.Warning,
            FV.Severity.Info => ValidationSeverity.Info,
            _ => ValidationSeverity.Error
        };
    }
}

