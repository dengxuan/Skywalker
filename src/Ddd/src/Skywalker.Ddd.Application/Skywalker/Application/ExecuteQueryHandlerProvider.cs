using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Abstractions;
using Skywalker.Application.Dtos.Contracts;

namespace Skywalker.Application;

public class ExecuteQueryHandlerProvider<TOutputDto> : IExecuteQueryHandlerProvider<TOutputDto> where TOutputDto : IEntityDto
{
    private readonly IServiceProvider _serviceProvider;

    public ExecuteQueryHandlerProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private ExecuteHandlerDelegate<TOutputDto> Pipeline(ExecuteHandlerDelegate<TOutputDto> next, IExecutePipelineBehavior<TOutputDto> behavior)
    {
        return (CancellationToken cancellationToken) =>
        {
            return behavior.HandleAsync(next, cancellationToken);
        };
    }

    public Task<TOutputDto?> HandleAsync(CancellationToken cancellationToken)
    {
        var behaviors = _serviceProvider.GetServices<IExecutePipelineBehavior<TOutputDto>>().Reverse();

        var executeDelegate = behaviors.Aggregate((ExecuteHandlerDelegate<TOutputDto>)SeedAsync, Pipeline);
        return executeDelegate(cancellationToken);
        Task<TOutputDto?> SeedAsync(CancellationToken cancellationToken)
        {
            IExecuteQueryHandler<TOutputDto> queryHandler = _serviceProvider.GetRequiredService<IExecuteQueryHandler<TOutputDto>>();
            return queryHandler.HandleAsync(cancellationToken);
        }
    }
}

public class ExecuteQueryHandlerProvider<TInputDto, TOutputDto> : IExecuteQueryHandlerProvider<TInputDto, TOutputDto>
    where TInputDto : IEntityDto
    where TOutputDto : IEntityDto
{
    private readonly IServiceProvider _serviceProvider;

    public ExecuteQueryHandlerProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private ExecuteHandlerDelegate<TOutputDto> Pipeline(ExecuteHandlerDelegate<TOutputDto> next, IExecutePipelineBehavior<TOutputDto> behavior)
    {
        return (CancellationToken cancellationToken) =>
        {
            return behavior.HandleAsync(next, cancellationToken);
        };
    }

    public Task<TOutputDto?> HandleAsync(TInputDto inputDto, CancellationToken cancellationToken)
    {
        var behaviors = _serviceProvider.GetServices<IExecutePipelineBehavior<TOutputDto>>().Reverse();
        var executeDelegate = behaviors.Aggregate((ExecuteHandlerDelegate<TOutputDto>)SeedAsync, Pipeline);
        return  executeDelegate(cancellationToken);

        Task<TOutputDto?> SeedAsync(CancellationToken cancellationToken)
        {
            IExecuteQueryHandler<TInputDto, TOutputDto> queryHandler = _serviceProvider.GetRequiredService<IExecuteQueryHandler<TInputDto, TOutputDto>>();
            return queryHandler.HandleAsync(inputDto, cancellationToken);
        }
    }
}
