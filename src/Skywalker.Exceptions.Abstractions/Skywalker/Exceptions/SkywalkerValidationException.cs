using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace Skywalker.Exceptions;

[Serializable]
public class SkywalkerValidationException : SkywalkerException, IHasErrorCode, IHasLogLevel, IHasValidationErrors
{
    public string? Code => "Skywalker:Validation";
    public LogLevel LogLevel { get; set; } = LogLevel.Warning;
    public IReadOnlyList<ValidationResult> ValidationErrors { get; }

    public SkywalkerValidationException()
    {
        ValidationErrors = Array.Empty<ValidationResult>();
    }

    public SkywalkerValidationException(string message) : base(message)
    {
        ValidationErrors = Array.Empty<ValidationResult>();
    }

    public SkywalkerValidationException(IEnumerable<ValidationResult> validationErrors)
        : base(BuildErrorMessage(validationErrors))
    {
        ValidationErrors = validationErrors.ToList().AsReadOnly();
    }

    public SkywalkerValidationException(string message, IEnumerable<ValidationResult> validationErrors)
        : base(message)
    {
        ValidationErrors = validationErrors.ToList().AsReadOnly();
    }

    public SkywalkerValidationException(string message, Exception? innerException) : base(message, innerException)
    {
        ValidationErrors = Array.Empty<ValidationResult>();
    }

    private static string BuildErrorMessage(IEnumerable<ValidationResult> validationErrors)
    {
        var errors = validationErrors.ToList();
        if (errors.Count == 0) return "Validation failed.";
        return "Validation failed: " + string.Join("; ", errors.Select(e => e.ErrorMessage));
    }
}
