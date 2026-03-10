// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// The search request, including filter, sort, paging
/// </summary>
public interface ISearchRequest : IFilteredRequest, ISortedRequest,IPagedRequest
{

}
