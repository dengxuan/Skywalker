using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Dtos;

/// <summary>
/// ʵ�ַ�ҳ��Ӧ�ӿ� <see cref="IPagedResponse{T}"/>.
/// </summary>
/// <typeparam name="T">��ѯ�����<see cref="ListResponseDto{T}.Items"/>������</typeparam>
/// <param name="TotalCount"><see cref="IPagedResponse{T}.TotalCount"/></param>
/// <param name="Items"><see cref="ListResponseDto{T}.Items"/></param>
[Serializable]
public record class PagedResultDto<T>(int TotalCount, IReadOnlyList<T> Items) : ListResponseDto<T>(Items), IResponseDto, IPagedResponse<T>;
