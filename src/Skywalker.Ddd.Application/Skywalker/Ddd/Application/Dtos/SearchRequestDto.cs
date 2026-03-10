using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Dtos;

/// <summary>
/// The search request, including filter, sort, paging
/// </summary>
[Serializable]
public record SearchRequestDto(string? Filter, string? Sorting, int Skip = 0, int Limit = 20) : PagedRequestDto(Skip, Limit), ISearchRequest, IRequestDto;
