using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Abstractions;
using Skywalker.Application.Dtos.Contracts;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Application;

public class ExecuteQueryHandlerProvider<TOutputDto> : IExecuteQueryHandlerProvider<TOutputDto> where TOutputDto : IEntityDto
{
    private readonly IServiceProvider _serviceProvider;

    public ExecuteQueryHandlerProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private Task<TOutputDto?> SeedAsync(CancellationToken cancellationToken)
    {
        IExecuteQueryHandler<TOutputDto> queryHandler = _serviceProvider.GetRequiredService<IExecuteQueryHandler<TOutputDto>>();
        return queryHandler.HandleAsync(cancellationToken);
    }

    private ExecuteQueryHandlerDelegate<TOutputDto> Pipeline(ExecuteQueryHandlerDelegate<TOutputDto> next, IExecuteQueryPipelineBehavior<TOutputDto> behavior)
    {
        return (CancellationToken cancellationToken) =>
        {
            return behavior.HandleAsync(next, cancellationToken);
        };
    }

    public Task<TOutputDto?> HandleAsync(CancellationToken cancellationToken)
    {
        var behaviors = _serviceProvider.GetServices<IExecuteQueryPipelineBehavior<TOutputDto>>().Reverse();
        var executeDelegate = behaviors.Aggregate((ExecuteQueryHandlerDelegate<TOutputDto>)SeedAsync, Pipeline);
        return executeDelegate(cancellationToken);
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

    private Task<TOutputDto?> SeedAsync(TInputDto inputDto, CancellationToken cancellationToken)
    {
        IExecuteQueryHandler<TInputDto, TOutputDto> queryHandler = _serviceProvider.GetRequiredService<IExecuteQueryHandler<TInputDto, TOutputDto>>();
        return queryHandler.HandleAsync(inputDto, cancellationToken);
    }

    private ExecuteQueryHandlerDelegate<TInputDto, TOutputDto> Pipeline(ExecuteQueryHandlerDelegate<TInputDto, TOutputDto> next, IExecuteQueryPipelineBehavior<TInputDto, TOutputDto> behavior)
    {
        return (TInputDto inputDto, CancellationToken cancellationToken) =>
        {
            return behavior.HandleAsync(inputDto, next, cancellationToken);
        };
    }

    public Task<TOutputDto?> HandleAsync(TInputDto inputDto, CancellationToken cancellationToken)
    {
        var behaviors = _serviceProvider.GetServices<IExecuteQueryPipelineBehavior<TInputDto, TOutputDto>>().Reverse();
        var executeDelegate = behaviors.Aggregate((ExecuteQueryHandlerDelegate<TInputDto, TOutputDto>)SeedAsync, Pipeline);
        return executeDelegate(inputDto, cancellationToken);
    }
}
