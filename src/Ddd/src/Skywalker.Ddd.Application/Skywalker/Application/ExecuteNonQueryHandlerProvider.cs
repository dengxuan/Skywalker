using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Abstractions;
using Skywalker.Application.Dtos.Contracts;

namespace Skywalker.Application;

public class ExecuteNonQueryHandlerProvider<TInputDto> : IExecuteNonQueryHandlerProvider<TInputDto> where TInputDto : IEntityDto
{
    private readonly IServiceProvider _serviceProvider;

    public ExecuteNonQueryHandlerProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private ExecuteNonQueryHandlerDelegate<TInputDto> Pipeline(ExecuteNonQueryHandlerDelegate<TInputDto> next, IExecuteNonQueryPipelineBehavior<TInputDto> behavior)
    {
        return (TInputDto input, CancellationToken cancellationToken) =>
        {
            return behavior.HandleAsync(input, next, cancellationToken);
        };
    }

    public Task HandleAsync(TInputDto inputDto, CancellationToken cancellationToken)
    {
        var behaviors = _serviceProvider.GetServices<IExecuteNonQueryPipelineBehavior<TInputDto>>().Reverse();

        IExecuteNonQueryHandler<TInputDto> handler = _serviceProvider.GetRequiredService<IExecuteNonQueryHandler<TInputDto>>();

        var executeDelegate = behaviors.Aggregate((ExecuteNonQueryHandlerDelegate<TInputDto>)handler.HandleAsync, Pipeline);
        return executeDelegate(inputDto, cancellationToken);
    }

}