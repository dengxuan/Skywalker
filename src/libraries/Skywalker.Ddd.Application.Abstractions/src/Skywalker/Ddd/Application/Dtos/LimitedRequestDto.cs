// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Dtos;

/// <summary>
/// <see cref="ILimitedRequest"/>接口的简单实现.
/// <paramref name="MaxResultCount"><see cref="ILimitedRequest.MaxResultCount"/></paramref>
/// </summary>
public record class LimitedRequestDto(int MaxResultCount = 20) : IRequestDto, ILimitedRequest;
