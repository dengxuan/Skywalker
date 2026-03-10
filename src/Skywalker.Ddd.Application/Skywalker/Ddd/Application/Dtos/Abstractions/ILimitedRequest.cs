// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// The limited request
/// </summary>
public interface ILimitedRequest
{
    /// <summary>
    /// The number of records to take
    /// </summary>
    int Limit { get; init; }
}
