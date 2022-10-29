// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.ApplicationService.Example.Abstractions.Skywalker.Ddd.ApplicationService.Examples;

/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
public record class GetExampleRequestDto(int Id, string Name) : IRequestDto<GetExampleResponseDto>;

/// <summary>
/// 
/// </summary>
/// <param name="Description"></param>
public record class GetExampleResponseDto(string Description) : IResponseDto;
