namespace Skywalker.Validation;

/// <summary>
/// Default implementation of <see cref="IValidationResult"/>.
/// </summary>
public class ValidationResult : IValidationResult
{
    private readonly List<ValidationError> _errors;

    /// <inheritdoc />
    public bool IsValid => _errors.Count == 0 || _errors.All(e => e.Severity != ValidationSeverity.Error);

    /// <inheritdoc />
    public IReadOnlyList<ValidationError> Errors => _errors.AsReadOnly();

    /// <summary>
    /// Creates a new validation result.
    /// </summary>
    public ValidationResult()
    {
        _errors = new List<ValidationError>();
    }

    /// <summary>
    /// Creates a new validation result with errors.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    public ValidationResult(IEnumerable<ValidationError> errors)
    {
        _errors = new List<ValidationError>(errors);
    }

    /// <summary>
    /// Adds an error to the result.
    /// </summary>
    /// <param name="error">The error to add.</param>
    public void AddError(ValidationError error)
    {
        _errors.Add(error);
    }

    /// <summary>
    /// Adds errors to the result.
    /// </summary>
    /// <param name="errors">The errors to add.</param>
    public void AddErrors(IEnumerable<ValidationError> errors)
    {
        _errors.AddRange(errors);
    }

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    public static ValidationResult Success() => new();

    /// <summary>
    /// Creates a failed validation result with a single error.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="errorMessage">The error message.</param>
    public static ValidationResult Failure(string propertyName, string errorMessage)
    {
        return new ValidationResult(new[] { new ValidationError(propertyName, errorMessage) });
    }

    /// <summary>
    /// Creates a failed validation result with multiple errors.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    public static ValidationResult Failure(IEnumerable<ValidationError> errors)
    {
        return new ValidationResult(errors);
    }

    public override string ToString()
    {
        if (IsValid)
        {
            return "Validation succeeded";
        }

        return $"Validation failed: {string.Join("; ", _errors.Select(e => e.ToString()))}";
    }
}

