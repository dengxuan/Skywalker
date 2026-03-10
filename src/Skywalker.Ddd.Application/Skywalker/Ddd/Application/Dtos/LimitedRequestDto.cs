// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Dtos;

/// <summary>
/// <see cref="ILimitedRequest"/>�ӿڵļ�ʵ��.
/// <see cref="ILimitedRequest.Limit"/>
/// </summary>
public record class LimitedRequestDto(int Limit = 20) : ILimitedRequest, IRequestDto;
