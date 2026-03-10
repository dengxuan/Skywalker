// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// 标准化查询请求接口，以便于设置最大查询结果数量
/// </summary>
public interface ILimitedRequest
{
    /// <summary>
    /// 应返回最大查询结果计数，通常用于限制分页的查询结果计数
    /// </summary>
    int Limit { get; init; }
}
