using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Dtos;

/// <summary>
/// 分区请求接口<see cref="IPagedRequest"/>的简单实现.
/// </summary>
[Serializable]
public record class PagedRequestDto(int SkipCount) : LimitedRequestDto, IRequestDto, IPagedRequest;
