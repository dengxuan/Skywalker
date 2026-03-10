// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// 标准化查询请求接口，以便实现查询分页
/// </summary>
public interface IPagedRequest : ILimitedRequest
{
    /// <summary>
    /// 跳过数据条数(分页的开始行数)
    /// </summary>
    int Skip { get; init; }
}
