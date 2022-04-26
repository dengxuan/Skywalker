// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Domain.Entities;

/// <summary>
/// A standard interface to add ModificationTime property.
/// </summary>
public interface IHasModificationTime
{

    /// <summary>
    /// The last modified time for this entity.
    /// </summary>
    DateTime? ModificationTime { get; set; }
}
