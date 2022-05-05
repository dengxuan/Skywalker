// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Application.Dtos.Abstractions;
using Skywalker.Ddd.Application.Pipeline;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Application.Abstractions;

/// <summary>
/// Pipeline behavior to surround the inner handler.
/// Implementations add additional behavior and await the next delegate.
/// </summary>
public interface IApplicationHandlerBehavior : ISingletonDependency
{

    /// <summary>
    /// Pipeline handler. Perform any additional behavior and await the <paramref name="next"/> delegate as necessary
    /// </summary>
    /// <param name="request">Incoming request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
    ValueTask HandleAsync<TRequest>(TRequest request, ApplicationHandlerDelegate<TRequest> next, CancellationToken cancellationToken) where TRequest : notnull, IRequestDto;

    /// <summary>
    /// Pipeline handler. Perform any additional behavior and await the <paramref name="next"/> delegate as necessary
    /// </summary>
    /// <param name="request">Incoming request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
    /// <returns>Awaitable task returning the <typeparamref name="TResponse"/></returns>
    ValueTask<TResponse?> HandleAsync<TRequest, TResponse>(TRequest request, ApplicationHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken) where TRequest : notnull, IRequestDto where TResponse : notnull, IResponseDto;

}
