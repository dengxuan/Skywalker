// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// Paged Request
/// </summary>
public interface IPagedRequest : ILimitedRequest
{
    /// <summary>
    /// The number of records to skip
    /// </summary>
    int Skip { get; init; }
}
