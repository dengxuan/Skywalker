// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.Collections.Generic;

namespace System.Linq;

public static class QueryableExtensions
{
    public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int skip, int limit)
    {
        var totalCount = source.Count();
        var items = source.Page(skip, limit);
        var result = new PagedList<T>(totalCount, items);
        return result;
    }

    public static Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int skip, int limit)
    {
        return Task.Run(() => source.ToPagedList(skip, limit));
    }
}

