// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.IdentifierGenerator.Abstractions;

/// <summary>
/// Used to generate Guid.
/// </summary>
public interface IGuidGenerator
{
    /// <summary>
    /// Creates a new <see cref="Guid"/>.
    /// </summary>
    Guid Create();
}
