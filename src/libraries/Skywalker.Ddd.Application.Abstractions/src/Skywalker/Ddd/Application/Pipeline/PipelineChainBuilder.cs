// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Application.Dtos.Abstractions;
using Skywalker.Ddd.Application.Pipeline.Abstractions;
using Skywalker.Extensions.DependencyInjection.Abstractions;

namespace Skywalker.Ddd.Application.Pipeline;

/// <summary>
/// The default implementation of interceptor chain builder.
/// </summary>
public sealed class PipelineChainBuilder : IPipelineChainBuilder
{
    private delegate ValueTask InvokeDelegate(object pipeline, PipelineContext context);

    private readonly List<PipelineDelegate> _pipelines = new();

    private readonly Dictionary<Type, InvokeDelegate> _invokers = new();

    private static readonly object s_locker = new();


    /// <summary>
    /// Gets the service provider to get dependency services.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Create a new <see cref="IPipelineChainBuilder"/>.
    /// </summary>
    /// <param name="serviceProvider">The service provider to get dependency services.</param>
    /// <exception cref="ArgumentNullException">The argument <paramref name="serviceProvider"/> is null.</exception>
    public PipelineChainBuilder(IServiceProvider serviceProvider)
    {
        Check.NotNull(serviceProvider, nameof(serviceProvider));

        ServiceProvider = serviceProvider;
    }

    private bool TryGetInvoke(Type pipelineType, out InvokeDelegate? invoker)
    {
        if (_invokers.TryGetValue(pipelineType, out invoker))
        {
            return true;
        }

        lock (s_locker)
        {
            if (_invokers.TryGetValue(pipelineType, out invoker))
            {
                return true;
            }

            var search = from it in pipelineType.GetTypeInfo().GetMethods()
                         let parameters = it.GetParameters()
                         where it.Name == "InvokeAsync" && it.ReturnType == typeof(ValueTask) && parameters.FirstOrDefault()?.ParameterType == typeof(PipelineContext)
                         select it;
            var invokeAsync = search.FirstOrDefault();
            if (null == invokeAsync)
            {
                return false;
            }

            var pipeline = Expression.Parameter(typeof(object), "pipeline");
            var pipelineContext = Expression.Parameter(typeof(PipelineContext), "context");

            Expression instance = Expression.Convert(pipeline, pipelineType);
            var invoke = Expression.Call(instance, invokeAsync, pipelineContext);
            invoker = Expression.Lambda<InvokeDelegate>(invoke, pipeline, pipelineContext).Compile();
            _invokers[pipelineType] = invoker;
        }
        return true;
    }

    /// <summary>
    /// Register the pipeline of <paramref name="pipelineType"/> to specified pipeline chain builder.
    /// </summary>
    /// <param name="pipelineType">The pipeline type.</param>
    /// <returns>The pipeline chain builder with registered pipeline.</returns>
    /// <exception cref="ArgumentNullException">The argument <paramref name="pipelineType"/> is null.</exception>
    private IPipelineChainBuilder Use(Type pipelineType)
    {
        Check.NotNull(pipelineType, nameof(pipelineType));

        InterceptDelegate Intercept(InterceptDelegate next)
        {
            return async context =>
            {
                var serviceProvider = ServiceProvider.GetService<IScopedServiceProviderAccesssor>()?.Current ?? ServiceProvider;
                var pipeline = ActivatorUtilities.CreateInstance(serviceProvider, pipelineType, new object[] { next });
                if (TryGetInvoke(pipelineType, out var invoker))
                {
                    await invoker!(pipeline, context);
                }
                else
                {
                    throw new ArgumentException("Invalid interceptor type {pipeline}", pipeline.GetType().Name);
                }
            };
        }
        return Add(Intercept);
    }

    /// <summary>
    /// Register the pipeline of <typeparamref name="TPipeline"/> type to specified pipeline chain builder.
    /// </summary>
    /// <typeparam name="TPipeline">The pipeline type.</typeparam>
    /// <returns>The pipeline chain builder with registered pipeline.</returns>
    public IPipelineChainBuilder Add<TPipeline>()
    {
        return Use(typeof(TPipeline));
    }


    /// <summary>
    /// Register specified pipeline.
    /// </summary>
    /// <param name="pipeline">The pipeline to register.</param>
    /// <returns>The pipeline chain builder with registered pipeline.</returns>
    /// <exception cref="ArgumentNullException">The argument <paramref name="pipeline"/> is null.</exception>
    public IPipelineChainBuilder Add(PipelineDelegate pipeline)
    {
        Check.NotNull(pipeline, nameof(pipeline));
        _pipelines.Add(pipeline);
        return this;
    }

    /// <summary>
    /// Build an pipeline chain using the registerd pipelines.
    /// </summary>
    /// <returns>A composite pipeline representing the pipeline chain.</returns>
    public InterceptDelegate Build()
    {
        InterceptDelegate current = async context => await context.Target(context);
        for (var c = _pipelines.Count - 1; c >= 0; c--)
        {
            current = _pipelines[c](current);
        }
        return current;
        //return next =>
        //{
        //    var current = next;
        //    for (var c = _pipelines.Count - 1; c >= 0; c--)
        //    {
        //        current = _pipelines[c](current);
        //    }
        //    return current;
        //};
    }

    /// <summary>
    /// Create a new pipeline chain builder which owns the same service provider as the current one.
    /// </summary>
    /// <returns>The new pipeline to create.</returns>
    public IPipelineChainBuilder New()
    {
        return new PipelineChainBuilder(ServiceProvider);
    }
}
