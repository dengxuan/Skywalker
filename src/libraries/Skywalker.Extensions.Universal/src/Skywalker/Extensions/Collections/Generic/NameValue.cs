// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.Collections.Generic;


/// <summary>
/// Can be used to store Name/Value (or Key/Value) pairs.
/// </summary>
[Serializable]
public class NameValue<T>
{
    /// <summary>
    /// Name.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Value.
    /// </summary>
    public T? Value { get; set; }

    /// <summary>
    /// Creates a new <see cref="NameValue"/>.
    /// The <see cref="Name"/>  and the <see cref="Value"/> is null.
    /// </summary>
    public NameValue() { }

    /// <summary>
    /// Creates a new <see cref="NameValue"/>.
    /// </summary>
    public NameValue(string name, T? value)
    {
        Name = name;
        Value = value;
    }
}
