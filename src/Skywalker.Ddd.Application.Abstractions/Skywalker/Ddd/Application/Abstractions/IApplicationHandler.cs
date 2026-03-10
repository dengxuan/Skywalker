// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Application.Dtos.Abstractions;
namespace Skywalker.Ddd.Application.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IApplicationHandler //: ITransientDependency
{

}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public interface IApplicationHandler<TRequest> : IApplicationHandler where TRequest : IRequestDto
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask HandleAsync(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface IApplicationHandler<TRequest, TResponse> : IApplicationHandler where TRequest : IRequestDto<TResponse>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}
