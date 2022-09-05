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

    public static Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int skip, int limit, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => source.ToPagedList(skip, limit), cancellationToken);
    }

    /// <summary>
    /// Filters a <see cref="IEnumerable{T}"/> by given predicate if given condition is true.
    /// </summary>
    /// <param name="source">Enumerable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the enumerable</param>
    /// <returns>Filtered or not filtered enumerable based on <paramref name="condition"/></returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Func<T, bool> predicate)
    {
        return condition
            ? source.Where(predicate).AsQueryable()
            : source;
    }

    /// <summary>
    /// Filters a <see cref="IEnumerable{T}"/> by given predicate if given condition is true.
    /// </summary>
    /// <param name="source">Enumerable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the enumerable</param>
    /// <returns>Filtered or not filtered enumerable based on <paramref name="condition"/></returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Func<T, int, bool> predicate)
    {
        return condition
            ? source.Where(predicate).AsQueryable()
            : source;
    }
}

