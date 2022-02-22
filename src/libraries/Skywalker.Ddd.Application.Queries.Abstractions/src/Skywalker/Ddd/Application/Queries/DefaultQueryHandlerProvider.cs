using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Application.Dtos.Abstractions;
using Skywalker.Ddd.Application.Queries.Abstractions;

namespace Skywalker.Ddd.Application.Queries;

public class DefaultQueryHandlerProvider<TQuery, TResponse> : IQueryHandlerProvider<TQuery, TResponse> where TQuery : IRequestDto where TResponse : IResponseDto
{
    private readonly IServiceProvider _serviceProvider;

    public DefaultQueryHandlerProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ValueTask<TResponse?> HandleAsync(TQuery querier, CancellationToken cancellationToken)
    {
        var queryHandler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();
        ValueTask<TResponse?> Handler() => queryHandler!.HandleAsync(querier, cancellationToken);

        return _serviceProvider.GetServices<IQueryPipelineBehavior<TQuery, TResponse>>()
                               .Reverse()
                               .Aggregate((QueryHandlerDelegate<TResponse>)Handler, (next, pipeline) => () => pipeline.HandleAsync(querier, next, cancellationToken))();
    }
}
