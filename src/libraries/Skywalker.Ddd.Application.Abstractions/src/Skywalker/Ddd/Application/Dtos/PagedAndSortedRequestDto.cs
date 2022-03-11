using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Dtos;

/// <summary>
/// ��ҳ����������<see cref="IPagedAndSortedRequest"/>�ļ�ʵ�֡�
/// </summary>
[Serializable]
public record class PagedAndSortedRequestDto(string Sorting, int SkipCount = 0, int MaxResultCount = 20) : PagedRequestDto(SkipCount, MaxResultCount), IRequestDto, IPagedAndSortedRequest;
