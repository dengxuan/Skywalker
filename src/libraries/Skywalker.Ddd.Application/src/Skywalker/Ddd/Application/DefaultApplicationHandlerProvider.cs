// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Application.Abstractions;
using Skywalker.Ddd.Application.Dtos.Abstractions;
using Skywalker.Ddd.Application.Pipeline;

namespace Skywalker.Ddd.Application;

internal class DefaultApplicationHandlerProvider : IApplicationHandlerProvider
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IEnumerable<IPipelineBehavior> _behaviors;

    public DefaultApplicationHandlerProvider(IServiceProvider serviceProvider, IEnumerable<IPipelineBehavior> behaviors)
    {
        _serviceProvider = serviceProvider;
        _behaviors = behaviors.Reverse();
    }

    public async ValueTask HandleAsync<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequestDto
    {
        var aggregate = _behaviors.Aggregate((ApplicationHandlerDelegate<TRequest>)Handler, (next, pipeline) =>
        {
            return (request, cancellationToken) =>
            {
                return pipeline.HandleAsync(request, next, cancellationToken);
            };
        });

        await aggregate(request, cancellationToken);

        ValueTask Handler(TRequest request, CancellationToken cancellationToken)
        {
            var handler = _serviceProvider.GetRequiredService<IApplicationHandler<TRequest>>();
            return handler!.HandleAsync(request, cancellationToken);
        }
    }

    public async ValueTask<TResponse?> HandleAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequestDto where TResponse : IResponseDto
    {
        var aggregate = _behaviors.Aggregate((ApplicationHandlerDelegate<TRequest, TResponse>)Handler, (next, pipeline) =>
        {
            return (request, cancellationToken) =>
            {
                return pipeline.HandleAsync(request, next, cancellationToken);
            };
        });
        return await aggregate(request, cancellationToken);

        ValueTask<TResponse?> Handler(TRequest request, CancellationToken cancellationToken)
        {
            var handler = _serviceProvider.GetRequiredService<IApplicationHandler<TRequest, TResponse>>();
            return handler!.HandleAsync(request, cancellationToken);
        }
    }
}
