using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Skywalker.Validation
{
    public interface IAbpValidationResult
    {
        List<ValidationResult> Errors { get; }
    }
}