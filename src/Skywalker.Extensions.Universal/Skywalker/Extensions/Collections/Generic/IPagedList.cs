// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.Collections.Generic;

/// <summary>
/// 分页集合
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPagedList<out T> : IReadOnlyList<T>
{
    /// <summary>
    /// 总数据量
    /// </summary>
    public int TotalCount { get; }
}
