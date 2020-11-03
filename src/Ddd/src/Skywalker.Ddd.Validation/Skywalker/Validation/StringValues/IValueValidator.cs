using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Validation.StringValues
{
    public interface IValueValidator
    {
        string Name { get; }

        [MaybeNull]
        object this[string key] { get; set; }

        [NotNull]
        IDictionary<string, object> Properties { get; }

        bool IsValid(object value);
    }
}