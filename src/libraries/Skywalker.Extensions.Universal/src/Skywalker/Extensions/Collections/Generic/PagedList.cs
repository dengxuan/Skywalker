// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Collections;

namespace Skywalker.Extensions.Collections.Generic;

/// <summary>
/// <inheritdoc/>
/// </summary>
public class PagedList<T> : List<T>, IPagedList<T>, IReadOnlyList<T>, IEnumerable<T>, IEnumerable
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int TotalCount { get; }

    public PagedList(int totalCount, IEnumerable<T> collection) : base(collection)
    {
        TotalCount = totalCount;
    }
}

