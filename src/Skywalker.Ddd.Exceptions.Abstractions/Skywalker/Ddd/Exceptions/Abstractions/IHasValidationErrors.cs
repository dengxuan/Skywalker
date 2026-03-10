using System.ComponentModel.DataAnnotations;

namespace Skywalker.Ddd.Exceptions;

/// <summary>
/// Interface for exceptions that have validation errors.
/// </summary>
public interface IHasValidationErrors
{
    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    IReadOnlyList<ValidationResult> ValidationErrors { get; }
}

