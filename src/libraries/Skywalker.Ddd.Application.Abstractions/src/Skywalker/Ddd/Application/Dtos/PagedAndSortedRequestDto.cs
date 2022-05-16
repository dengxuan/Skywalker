using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Dtos;

/// <summary>
/// ��ҳ����������<see cref="IPagedAndSortedRequest"/>�ļ�ʵ�֡�
/// </summary>
[Serializable]
public record class PagedAndSortedRequestDto(string Sorting, int Skip = 0, int Limit = 20) : PagedRequestDto(Skip, Limit), IRequestDto, IPagedAndSortedRequest;
