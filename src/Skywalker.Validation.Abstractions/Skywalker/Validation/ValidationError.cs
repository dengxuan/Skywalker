namespace Skywalker.Validation;

/// <summary>
/// Represents a validation error.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Gets or sets the property name that caused the error.
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets the attempted value that caused the error.
    /// </summary>
    public object? AttemptedValue { get; set; }

    /// <summary>
    /// Gets or sets the severity of the error.
    /// </summary>
    public ValidationSeverity Severity { get; set; } = ValidationSeverity.Error;

    /// <summary>
    /// Creates a new validation error.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="errorMessage">The error message.</param>
    public ValidationError(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a new validation error with error code.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="errorCode">The error code.</param>
    public ValidationError(string propertyName, string errorMessage, string errorCode)
        : this(propertyName, errorMessage)
    {
        ErrorCode = errorCode;
    }

    public override string ToString()
    {
        return $"{PropertyName}: {ErrorMessage}";
    }
}

