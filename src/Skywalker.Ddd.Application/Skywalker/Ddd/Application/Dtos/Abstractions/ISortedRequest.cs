// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// The sorted request
/// </summary>
public interface ISortedRequest
{
    /// <summary>
    /// The sorting string, separated by comma.
    /// The sorting string should be the property name of the entity, and can be followed by ASC or DESC.
    /// Multiple sorting can be separated by comma.
    /// e.g. "Name", "Name DESC", "Name ASC, Age DESC"
    /// The default sorting is ascending.
    /// If the sorting is not specified, the default sorting will be used.
    /// </summary>
    /// <example>
    /// e.g.
    /// "Name"
    /// "Name DESC"
    /// "Name ASC, Age DESC"
    /// </example>
    string? Sorting { get; init; }
}
