using System.ComponentModel.DataAnnotations;

namespace Skywalker.Exceptions;

public interface IHasValidationErrors
{
    IReadOnlyList<ValidationResult> ValidationErrors { get; }
}
