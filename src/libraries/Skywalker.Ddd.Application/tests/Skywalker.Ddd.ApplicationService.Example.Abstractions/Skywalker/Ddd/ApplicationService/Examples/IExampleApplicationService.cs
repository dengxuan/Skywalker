using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Application.Abstractions;
using Skywalker.Ddd.Application.Dtos.Abstractions;
using Skywalker.Ddd.ApplicationService.Example.Abstractions.Skywalker.Ddd.ApplicationService.Examples;

namespace Skywalker.Ddd.ApplicationService.Examples;

/// <summary>
/// 
/// </summary>
public interface IExampleApplicationService : IApplicationService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    ValueTask<GetExampleResponseDto> GetValueAsync(GetExampleRequestDto request);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    ValueTask<GetExampleResponseDto> GetValue1Async(GetExampleRequestDto request);
}

/// <summary>
/// 
/// </summary>
public interface IExample1ApplicationService : IApplicationService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    ValueTask<GetExampleResponseDto> GetValue1Async(GetExampleRequestDto request);
}

