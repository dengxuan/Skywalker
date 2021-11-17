using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Skywalker.Validation
{
    public interface IHasValidationErrors
    {
        IList<ValidationResult> ValidationErrors { get; }
    }
}