using Skywalker.Extensions.Collections;

namespace System.Collections.Generic;

public static class NamedTypeSelectorListExtensions
{
    /// <summary>
    /// Add list of types to the list.
    /// </summary>
    /// <param name="list">List of NamedTypeSelector items</param>
    /// <param name="name">An arbitrary but unique name (can be later used to remove types from the list)</param>
    /// <param name="types"></param>
    public static void Add(this IList<NamedTypeSelector> list, string name, params Type[] types)
    {
        list.NotNull(nameof(list));
        name.NotNull(nameof(name));
        types.NotNull(nameof(types));

        list.Add(new NamedTypeSelector(name, type => types.Any(type.IsAssignableFrom)));
    }
}
