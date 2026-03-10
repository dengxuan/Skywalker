// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// 标准化查询结果接口，以便于设置返回给客户端的分页结果集与数据总数
/// 详情参见 <see cref="IListResponse{T}"/> 和 <see cref="ITotalCountResponse"/>
/// </summary>
/// <typeparam name="T">查询结果集数据类型 <see cref="IListResponse{T}.Items"/></typeparam>
public interface IPagedResponse<T> : IListResponse<T>, ITotalCountResponse
{

}
