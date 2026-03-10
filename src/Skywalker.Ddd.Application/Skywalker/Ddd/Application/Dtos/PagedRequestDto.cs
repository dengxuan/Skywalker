using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Dtos;

/// <summary>
/// Paged request <see cref="IPagedRequest"/> implementation
/// </summary>
[Serializable]
public record PagedRequestDto(int Skip = 0, int Limit = 20) : LimitedRequestDto(Limit), IPagedRequest, IRequestDto;
