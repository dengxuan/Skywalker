namespace Skywalker.Validation;

/// <summary>
/// Specifies the severity level of a validation error.
/// </summary>
public enum ValidationSeverity
{
    /// <summary>
    /// Informational message.
    /// </summary>
    Info,

    /// <summary>
    /// Warning message.
    /// </summary>
    Warning,

    /// <summary>
    /// Error message. Validation fails.
    /// </summary>
    Error
}

