namespace Skywalker.Validation;

/// <summary>
/// Interface for validation results.
/// </summary>
public interface IValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the validation was successful.
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    IReadOnlyList<ValidationError> Errors { get; }
}

