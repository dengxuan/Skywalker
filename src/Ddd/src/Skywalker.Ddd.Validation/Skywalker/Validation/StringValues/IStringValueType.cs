using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Validation.StringValues
{
    public interface IStringValueType
    {
        string Name { get; }

        [MaybeNull]
        object this[string key] { get; set; }

        [NotNull]
        Dictionary<string, object> Properties { get; }

        IValueValidator Validator { get; set; }
    }
}
