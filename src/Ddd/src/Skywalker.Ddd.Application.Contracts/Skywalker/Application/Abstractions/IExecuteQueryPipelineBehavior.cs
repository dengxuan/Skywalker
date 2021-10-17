using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Dtos.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Application.Abstractions;

/// <summary>
/// Represents an async continuation for the next task to execute in the pipeline
/// </summary>
/// <typeparam name="TOutputDto">Response type</typeparam>
/// <returns>Awaitable task returning a <typeparamref name="TOutputDto"/></returns>
public delegate Task<TOutputDto?> ExecuteQueryHandlerDelegate<TOutputDto>(CancellationToken cancellationToken) where TOutputDto : IEntityDto;

/// <summary>
/// Represents an async continuation for the next task to execute in the pipeline
/// </summary>
/// <typeparam name="TOutputDto">Response type</typeparam>
/// <returns>Awaitable task returning a <typeparamref name="TOutputDto"/></returns>
public delegate Task<TOutputDto?> ExecuteQueryHandlerDelegate<TInputDto, TOutputDto>(TInputDto inputDto, CancellationToken cancellationToken) where TInputDto : IEntityDto where TOutputDto : IEntityDto;

/// <summary>
/// Pipeline behavior to surround the inner handler.
/// Implementations add additional behavior and await the next delegate.
/// </summary>
/// <typeparam name="TInputDto">Request type</typeparam>
/// <typeparam name="TOutputDto">Response type</typeparam>
public interface IExecuteQueryPipelineBehavior<TInputDto, TOutputDto> : IScopedDependency where TInputDto : IEntityDto where TOutputDto : IEntityDto
{
    /// <summary>
    /// Pipeline handler. Perform any additional behavior and await the <paramref name="next"/> delegate as necessary
    /// </summary>
    /// <param name="inputDto">Incoming request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
    /// <returns>Awaitable task returning the <typeparamref name="TOutputDto"/></returns>
    Task<TOutputDto?> HandleAsync(TInputDto inputDto, ExecuteQueryHandlerDelegate<TInputDto, TOutputDto> next, CancellationToken cancellationToken = default);
}

/// <summary>
/// Pipeline behavior to surround the inner handler.
/// Implementations add additional behavior and await the next delegate.
/// </summary>
/// <typeparam name="TOutputDto">Response type</typeparam>
public interface IExecuteQueryPipelineBehavior<TOutputDto> : IScopedDependency where TOutputDto : IEntityDto
{
    /// <summary>
    /// Pipeline handler. Perform any additional behavior and await the <paramref name="next"/> delegate as necessary
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
    /// <returns>Awaitable task returning the <typeparamref name="TOutputDto"/></returns>
    Task<TOutputDto?> HandleAsync(ExecuteQueryHandlerDelegate<TOutputDto> next, CancellationToken cancellationToken = default);
}
