using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Dtos.Contracts;

namespace Skywalker.Application.Abstractions;

/// <summary>
/// Pipeline behavior to surround the inner handler.
/// Implementations add additional behavior and await the next delegate.
/// </summary>
public interface IExecutePipelineBehavior<TOutputDto> : IScopedDependency where TOutputDto : IEntityDto
{
    /// <summary>
    /// Pipeline handler. Perform any additional behavior and await the <paramref name="next"/> delegate as necessary
    /// </summary>
    /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Awaitable task</returns>
    Task<TOutputDto?> HandleAsync(ExecuteHandlerDelegate<TOutputDto> next, CancellationToken cancellationToken = default);
}
