// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// 标准化查询请求接口，以便于设置对查询结果的排序
/// </summary>
public interface ISortedRequest
{
    /// <summary>
    /// 排序信息。
    /// 需要包含排序字段和可选的排序指令(ASC 或 DESC)，可包含多个字段，使用逗号(,)分割.
    /// </summary>
    /// <example>
    /// 例如:
    /// "Name"
    /// "Name DESC"
    /// "Name ASC, Age DESC"
    /// </example>
    string Sorting { get; init; }
}
