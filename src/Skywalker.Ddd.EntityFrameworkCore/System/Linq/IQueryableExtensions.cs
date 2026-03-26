// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Linq.Dynamic.Core;

namespace System.Linq;

/// <summary>
/// IQueryable扩展 
/// </summary>
public static class IQueryableExtensions
{
    /// <summary>
    /// 条件查询, 如果条件为真则执行查询
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="condition"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, string? filter)
    {
        return condition
            ? query.Where(filter!)
            : query;
    }

    /// <summary>
    /// 条件排序, 如果条件为真则执行排序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="condition"></param>
    /// <param name="sorting"></param>
    /// <returns></returns>
    public static IQueryable<T> OrderByIf<T>(this IQueryable<T> query, bool condition, string? sorting)
    {
        return condition
            ? query.OrderBy(sorting!)
            : query;
    }
}