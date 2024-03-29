﻿// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Dtos;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <param name="Id"></param>
public record class EntityDto<TKey>(TKey Id) : IEntityDto<TKey>, IRequestDto, IResponseDto;
