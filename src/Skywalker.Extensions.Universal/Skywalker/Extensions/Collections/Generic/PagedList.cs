// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.Collections.Generic;

/// <summary>
/// <inheritdoc/>
/// </summary>
public class PagedList<T>(int totalCount, IEnumerable<T> collection)
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int TotalCount { get; } = totalCount;

    public IEnumerable<T> Items { get; } = collection;
}

