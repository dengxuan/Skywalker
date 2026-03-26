// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// 标准化查询结果接口，以便于返回给客户端的查询结果集
/// </summary>
/// <typeparam name="T">查询结果集的数据类型，参考<see cref="Items"/> </typeparam>
public interface IListResponse<T> : IResponseDto
{
    /// <summary>
    /// 查询结果集
    /// </summary>
    IReadOnlyList<T> Items { get; init; }
}
