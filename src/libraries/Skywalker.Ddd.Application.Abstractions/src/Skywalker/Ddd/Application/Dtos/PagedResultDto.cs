using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Dtos;

/// <summary>
/// 实现分页响应接口 <see cref="IPagedResponse{T}"/>.
/// </summary>
/// <typeparam name="T">查询结果集<see cref="ListResponseDto{T}.Items"/>的类型</typeparam>
/// <param name="TotalCount"><see cref="IPagedResponse{T}.TotalCount"/></param>
/// <param name="Items"><see cref="ListResponseDto{T}.Items"/></param>
[Serializable]
public record class PagedResultDto<T>(int TotalCount, IReadOnlyList<T> Items) : ListResponseDto<T>(Items), IResponseDto, IPagedResponse<T>;
