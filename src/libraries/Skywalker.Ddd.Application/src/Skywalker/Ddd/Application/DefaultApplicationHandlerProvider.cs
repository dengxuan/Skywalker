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
    private readonly IPipelineChainBuilder _pipelineChainBuilder;
    private readonly IEnumerable<IPipelineBehavior> _behaviors;

    public DefaultApplicationHandlerProvider(IServiceProvider serviceProvider, IPipelineChainBuilder pipelineChainBuilder)
    {
        _serviceProvider = serviceProvider;
        _pipelineChainBuilder = pipelineChainBuilder;
        _behaviors = serviceProvider.GetServices<IPipelineBehavior>().Reverse();
    }

    public async ValueTask HandleAsync<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequestDto
    {
        var pipeline = _pipelineChainBuilder.Build();
        var handler = _serviceProvider.GetRequiredService<IApplicationHandler<TRequest>>();
        var pipe = pipeline.Invoke(async (context) =>
        {
            await handler!.HandleAsync(request, cancellationToken);
        });
        var method = (typeof(IApplicationHandler<TRequest>)).GetMethod("HandleAsync")!.CreateDelegate(typeof(ApplicationHandlerDelegate<TRequest>), handler);
        var context = new PipelineContext(handler, method, request, cancellationToken);
        await pipe(context);
        //var aggregate = _behaviors.Aggregate((ApplicationHandlerDelegate<TRequest>)Handler, (next, pipeline) =>
        //{
        //    return (request, cancellationToken) =>
        //    {
        //        return pipeline.HandleAsync(request, next, cancellationToken);
        //    };
        //});

        //await aggregate(request, cancellationToken);

        //ValueTask Handler(TRequest request, CancellationToken cancellationToken)
        //{
        //    var handler = _serviceProvider.GetRequiredService<IApplicationHandler<TRequest>>();
        //    return handler!.HandleAsync(request, cancellationToken);
        //}
    }

    public async ValueTask<TResponse?> HandleAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequestDto where TResponse : IResponseDto
    {
        var pipeline = _pipelineChainBuilder.Build();
        var handler = _serviceProvider.GetRequiredService<IApplicationHandler<TRequest, TResponse>>();
        var pipe = pipeline.Invoke(async (context) =>
        {
            context.ReturnValue = await handler!.HandleAsync(request, cancellationToken);
        });
        var method = (typeof(IApplicationHandler<TRequest, TResponse>)).GetMethod("HandleAsync")!.CreateDelegate(typeof(ApplicationHandlerDelegate<TRequest, TResponse>), handler);
        var context = new PipelineContext(handler, method, request, cancellationToken);
        await pipe(context);

        return (TResponse?)context.ReturnValue;
        //var aggregate = _behaviors.Aggregate((ApplicationHandlerDelegate<TRequest, TResponse>)Handler, (next, pipeline) =>
        //{
        //    return (request, cancellationToken) =>
        //    {
        //        return pipeline.HandleAsync(request, next, cancellationToken);
        //    };
        //});
        //return await aggregate(request, cancellationToken);

        //ValueTask<TResponse?> Handler(TRequest request, CancellationToken cancellationToken)
        //{
        //    var handler = _serviceProvider.GetRequiredService<IApplicationHandler<TRequest, TResponse>>();
        //    return handler!.HandleAsync(request, cancellationToken);
        //}
    }
}
