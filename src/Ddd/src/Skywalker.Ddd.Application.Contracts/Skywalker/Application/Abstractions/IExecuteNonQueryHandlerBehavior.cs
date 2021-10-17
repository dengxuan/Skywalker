using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Dtos.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Application.Abstractions;

public delegate Task ExecuteNonQueryHandlerDelegate<TInputDto>(TInputDto inputDto, CancellationToken cancellationToken) where TInputDto : IEntityDto;


/// <summary>
/// Pipeline behavior to surround the inner handler.
/// Implementations add additional behavior and await the next delegate.
/// </summary>
/// <typeparam name="TInputDto">Response type</typeparam>
public interface IExecuteNonQueryPipelineBehavior<TInputDto> : IScopedDependency where TInputDto : IEntityDto
{
    /// <summary>
    /// Pipeline handler. Perform any additional behavior and await the <paramref name="next"/> delegate as necessary
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
    /// <returns>Awaitable task returning the <typeparamref name="TInputDto"/></returns>
    Task HandleAsync(TInputDto inputDto, ExecuteNonQueryHandlerDelegate<TInputDto> next, CancellationToken cancellationToken = default);
}