using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Dtos;

/// <summary>
/// Paged result <see cref="IPagedResponse"/> implementation
/// </summary>
[Serializable]
public record PagedResultDto<T> : IResponseDto, IPagedResponse<T>
{
    public required int TotalCount { get; init; }

    public required IEnumerable<T> Items { get; init; }
}
