// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// 标准化查询请求接口，以便于设置查询分页和排序
/// </summary>
public interface IPagedAndSortedRequest : IPagedRequest, ISortedRequest
{

}
