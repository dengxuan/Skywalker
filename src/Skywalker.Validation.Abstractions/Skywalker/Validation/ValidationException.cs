namespace Skywalker.Validation;

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    public IReadOnlyList<ValidationError> Errors { get; }

    /// <summary>
    /// Creates a new validation exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ValidationException(string message) : base(message)
    {
        Errors = Array.Empty<ValidationError>();
    }

    /// <summary>
    /// Creates a new validation exception with errors.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    public ValidationException(IEnumerable<ValidationError> errors)
        : base(BuildMessage(errors))
    {
        Errors = errors.ToList().AsReadOnly();
    }

    /// <summary>
    /// Creates a new validation exception with a single error.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="errorMessage">The error message.</param>
    public ValidationException(string propertyName, string errorMessage)
        : this(new[] { new ValidationError(propertyName, errorMessage) })
    {
    }

    /// <summary>
    /// Creates a new validation exception from a validation result.
    /// </summary>
    /// <param name="result">The validation result.</param>
    public ValidationException(IValidationResult result)
        : this(result.Errors)
    {
    }

    private static string BuildMessage(IEnumerable<ValidationError> errors)
    {
        var errorList = errors.ToList();
        if (errorList.Count == 0)
        {
            return "Validation failed";
        }

        if (errorList.Count == 1)
        {
            return $"Validation failed: {errorList[0]}";
        }

        return $"Validation failed with {errorList.Count} errors: {string.Join("; ", errorList.Select(e => e.ToString()))}";
    }
}

