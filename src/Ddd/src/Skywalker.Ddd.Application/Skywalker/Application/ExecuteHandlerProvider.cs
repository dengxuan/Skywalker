using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Abstractions;
using Skywalker.Application.Dtos.Contracts;

namespace Skywalker.Application;

public class ExecuteHandlerProvider<TOutputDto> : IExecuteHandlerProvider<TOutputDto> where TOutputDto : IEntityDto
{
    private readonly IServiceProvider _serviceProvider;

    public ExecuteHandlerProvider(IServiceProvider serviceProvider)
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
            IExecuteHandler<TOutputDto> handler = _serviceProvider.GetRequiredService<IExecuteHandler<TOutputDto>>();
            return handler.HandleAsync(cancellationToken);
        }
    }
}

public class ExecuteQueryHandlerProvider<TInputDto, TOutputDto> : IExecuteHandlerProvider<TInputDto, TOutputDto>
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
            IExecuteHandler<TInputDto, TOutputDto> handler = _serviceProvider.GetRequiredService<IExecuteHandler<TInputDto, TOutputDto>>();
            return handler.HandleAsync(inputDto, cancellationToken);
        }
    }
}
