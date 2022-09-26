// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using System.Reflection;
using Skywalker.Ddd.Application.Abstractions;
using Skywalker.Ddd.Application.Dtos.Abstractions;
using Skywalker.Ddd.Application.Pipeline;
using Skywalker.Ddd.Application.Pipeline.Abstractions;

namespace Skywalker.Ddd.Application.Wrappers;

public abstract class ResponseHandlerWrapper : Wrapper
{
    protected ResponseHandlerWrapper(IServiceProvider services) : base(services)
    {
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public abstract class ResponseHandlerWrapper<TResponse> : ResponseHandlerWrapper
{
    protected ResponseHandlerWrapper(IServiceProvider services) : base(services)
    {
    }

    public abstract ValueTask<TResponse> Handle(IRequestDto<TResponse> request, CancellationToken cancellationToken);
}

public class ResponseHandlerWrapper<TRequest, TResponse> : ResponseHandlerWrapper<TResponse>
    where TRequest : IRequestDto<TResponse>
    where TResponse : notnull
{
    private static readonly ConcurrentDictionary<(Type, Type), MethodInfo> s_methods = new();
    private readonly InterceptDelegate _pipeline;

    public ResponseHandlerWrapper(IPipelineChainBuilder pipelineChainBuilder, IServiceProvider services) : base(services)
    {
        _pipeline = pipelineChainBuilder.Build();
    }

    public override async ValueTask<TResponse> Handle(IRequestDto<TResponse> request, CancellationToken cancellationToken)
    {
        var handler = GetHandler<IApplicationHandler<TRequest, TResponse>>();
        var handlerType = handler.GetType();
        var method = s_methods.GetOrAdd((handlerType, typeof(TRequest)), key =>
        {
            var search = from it in handler.GetType().GetMethods()
                         let parameters = it.GetParameters()
                         where it.Name == "HandleAsync" && it.ReturnType == typeof(ValueTask<TResponse>)
                         where parameters.FirstOrDefault()?.ParameterType == typeof(TRequest)
                         select it;
            var handleAsync = search.FirstOrDefault();
            return handleAsync;
        });
        PipelineContext context = new PipelineContext(async (PipelineContext context) =>
        {
            context.ReturnValue = await handler.HandleAsync((TRequest)request, cancellationToken);
        }, request, cancellationToken);
        context.Properties["Method"] = method;
        context.Properties["Handler"] = handler;
        context.Properties["HandlerType"] = handlerType;
        await _pipeline(context);
        return (TResponse)context.ReturnValue;
    }
}
