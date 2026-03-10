using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace Skywalker.Ddd.Exceptions;

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
[Serializable]
public class SkywalkerValidationException : SkywalkerException, IHasErrorCode, IHasLogLevel, IHasValidationErrors
{
    /// <summary>
    /// Gets the error code.
    /// </summary>
    public string? Code => "Skywalker:Validation";

    /// <summary>
    /// Gets the log level for this exception.
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Warning;

    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    public IReadOnlyList<ValidationResult> ValidationErrors { get; }

    /// <summary>
    /// Creates a new <see cref="SkywalkerValidationException"/> object.
    /// </summary>
    public SkywalkerValidationException()
    {
        ValidationErrors = Array.Empty<ValidationResult>();
    }

    /// <summary>
    /// Creates a new <see cref="SkywalkerValidationException"/> object.
    /// </summary>
    /// <param name="message">Exception message</param>
    public SkywalkerValidationException(string message)
        : base(message)
    {
        ValidationErrors = Array.Empty<ValidationResult>();
    }

    /// <summary>
    /// Creates a new <see cref="SkywalkerValidationException"/> object.
    /// </summary>
    /// <param name="validationErrors">Validation errors</param>
    public SkywalkerValidationException(IEnumerable<ValidationResult> validationErrors)
        : base(BuildErrorMessage(validationErrors))
    {
        ValidationErrors = validationErrors.ToList().AsReadOnly();
    }

    /// <summary>
    /// Creates a new <see cref="SkywalkerValidationException"/> object.
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="validationErrors">Validation errors</param>
    public SkywalkerValidationException(string message, IEnumerable<ValidationResult> validationErrors)
        : base(message)
    {
        ValidationErrors = validationErrors.ToList().AsReadOnly();
    }

    /// <summary>
    /// Creates a new <see cref="SkywalkerValidationException"/> object.
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public SkywalkerValidationException(string message, Exception? innerException)
        : base(message, innerException)
    {
        ValidationErrors = Array.Empty<ValidationResult>();
    }

    private static string BuildErrorMessage(IEnumerable<ValidationResult> validationErrors)
    {
        var errors = validationErrors.ToList();
        if (errors.Count == 0)
        {
            return "Validation failed.";
        }

        return "Validation failed: " + string.Join("; ", errors.Select(e => e.ErrorMessage));
    }
}

