using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Queries.Abstractions;

/// <summary>
/// Represents an async continuation for the next task to execute in the pipeline
/// </summary>
/// <typeparam name="TResponse">Response type</typeparam>
/// <returns>Awaitable task returning a <typeparamref name="TResponse"/></returns>
public delegate ValueTask<TResponse?> QueryHandlerDelegate<TResponse>() where TResponse : IResponseDto;

/// <summary>
/// Pipeline behavior to surround the inner handler.
/// Implementations add additional behavior and await the next delegate.
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public interface IQueryPipelineBehavior<in TRequest, TResponse> where TRequest : IRequestDto where TResponse : IResponseDto
{
    /// <summary>
    /// Pipeline handler. Perform any additional behavior and await the <paramref name="next"/> delegate as necessary
    /// </summary>
    /// <param name="querier">Incoming request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
    /// <returns>Awaitable task returning the <typeparamref name="TResponse"/></returns>
    ValueTask<TResponse?> HandleAsync(TRequest querier, QueryHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default);
}
