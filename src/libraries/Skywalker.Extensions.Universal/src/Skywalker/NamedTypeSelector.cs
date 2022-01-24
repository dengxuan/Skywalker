// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker;

/// <summary>
/// Used to represent a named type selector.
/// </summary>
public class NamedTypeSelector
{
    /// <summary>
    /// Name of the selector.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Predicate.
    /// </summary>
    public Func<Type, bool> Predicate { get; set; }

    /// <summary>
    /// Creates new <see cref="NamedTypeSelector"/> object.
    /// </summary>
    /// <param name="name">Name</param>
    /// <param name="predicate">Predicate</param>
    public NamedTypeSelector(string name, Func<Type, bool> predicate)
    {
        Name = name;
        Predicate = predicate;
    }
}
