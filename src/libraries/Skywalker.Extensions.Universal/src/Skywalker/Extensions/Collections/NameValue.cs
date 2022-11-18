// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.Collections.Generic;

namespace Skywalker.Extensions.Collections;

/// <summary>
/// Can be used to store Name/Value (or Key/Value) pairs.
/// </summary>
[Serializable]
public class NameValue : NameValue<string>
{
    ///// <summary>
    ///// Creates a new <see cref="NameValue"/>.
    ///// The <see cref="Name"/>  and the <see cref="Value"/> is null.
    ///// </summary>
    //public NameValue() { }

    /// <summary>
    /// Creates a new <see cref="NameValue"/>.
    /// </summary>
    public NameValue(string name, string value) : base(name, value) { }
}
