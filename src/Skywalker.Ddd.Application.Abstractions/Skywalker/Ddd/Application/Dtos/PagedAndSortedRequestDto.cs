using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Dtos;

/// <summary>
/// 分页和排序请求<see cref="IPagedAndSortedRequest"/>的简单实现。
/// </summary>
[Serializable]
public record class PagedAndSortedRequestDto(string Sorting, int Skip = 0, int Limit = 20) : PagedRequestDto(Skip, Limit), IRequestDto, IPagedAndSortedRequest;
