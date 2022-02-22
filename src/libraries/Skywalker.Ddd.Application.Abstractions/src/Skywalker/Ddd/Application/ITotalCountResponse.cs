// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// 标准化查询结果接口，以便于设置返回给客户端的数据总数
/// </summary>
public interface ITotalCountResponse
{
    /// <summary>
    /// 符合查询条件的数据总数
    /// </summary>
    int TotalCount { get; init; }
}
