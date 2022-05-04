// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Application.Abstractions;
using Skywalker.Extensions.DependencyInjection.Abstractions;

namespace Skywalker.Ddd.Application.Pipeline;

/// <summary>
/// The default implementation of interceptor chain builder.
/// </summary>
public sealed class PipelineChainBuilder : IPipelineChainBuilder
{
    private delegate ValueTask InvokeDelegate(object interceptor, PipelineContext context);

    private readonly ISet<PipelineDelegate> _interceptors;

    private readonly Dictionary<Type, InvokeDelegate> _invokers = new();

    private static readonly object s_locker = new();


    /// <summary>
    /// Gets the service provider to get dependency services.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Create a new <see cref="InterceptorChainBuilder"/>.
    /// </summary>
    /// <param name="serviceProvider">The service provider to get dependency services.</param>
    /// <exception cref="ArgumentNullException">The argument <paramref name="serviceProvider"/> is null.</exception>
    public PipelineChainBuilder(IServiceProvider serviceProvider)
    {
        Check.NotNull(serviceProvider, nameof(serviceProvider));

        ServiceProvider = serviceProvider;
        _interceptors = new SortedSet<PipelineDelegate>();
    }

    private bool TryGetInvoke(Type interceptorType, out InvokeDelegate? invoker)
    {
        if (_invokers.TryGetValue(interceptorType, out invoker))
        {
            return true;
        }

        lock (s_locker)
        {
            if (_invokers.TryGetValue(interceptorType, out invoker))
            {
                return true;
            }

            var search = from it in interceptorType.GetTypeInfo().GetMethods()
                         let parameters = it.GetParameters()
                         where it.Name == "InvokeAsync" && it.ReturnType == typeof(ValueTask) && parameters.FirstOrDefault()?.ParameterType == typeof(PipelineContext)
                         select it;
            var invokeAsync = search.FirstOrDefault();
            if (null == invokeAsync)
            {
                return false;
            }

            var interceptor = Expression.Parameter(typeof(object), "interceptor");
            var invocationContext = Expression.Parameter(typeof(PipelineContext), "context");

            Expression instance = Expression.Convert(interceptor, interceptorType);
            var invoke = Expression.Call(instance, invokeAsync, invocationContext);
            invoker = Expression.Lambda<InvokeDelegate>(invoke, interceptor, invocationContext).Compile();
            _invokers[interceptorType] = invoker;
        }
        return true;
    }

    /// <summary>
    /// Register the interceptor of <paramref name="interceptorType"/> to specified interceptor chain builder.
    /// </summary>
    /// <param name="interceptorType">The interceptor type.</param>
    /// <returns>The interceptor chain builder with registered interceptor.</returns>
    /// <exception cref="ArgumentNullException">The argument <paramref name="interceptorType"/> is null.</exception>
    private IPipelineChainBuilder Use(Type interceptorType)
    {
        Check.NotNull(interceptorType, nameof(interceptorType));

        InterceptDelegate Intercept(InterceptDelegate next)
        {
            return async context =>
            {
                var serviceProvider = ServiceProvider.GetService<IScopedServiceProviderAccesssor>()?.Current ?? ServiceProvider;
                var interceptor = ActivatorUtilities.CreateInstance(serviceProvider, interceptorType, new object[] { next });
                if (TryGetInvoke(interceptorType, out var invoker))
                {
                    await invoker!(interceptor, context);
                }
                else
                {
                    throw new ArgumentException("Invalid interceptor type {interceptorType}", interceptor.GetType().Name);
                }
            };
        }
        return Use(Intercept);
    }

    /// <summary>
    /// Register the interceptor of <typeparamref name="TInterceptor"/> type to specified interceptor chain builder.
    /// </summary>
    /// <typeparam name="TInterceptor">The interceptor type.</typeparam>
    /// <param name="builder">The interceptor chain builder to which the interceptor is registered.</param>
    /// <returns>The interceptor chain builder with registered interceptor.</returns>
    /// <exception cref="ArgumentNullException">The argument <paramref name="builder"/> is null.</exception>
    public IPipelineChainBuilder Use<TInterceptor>()
    {
        return Use(typeof(TInterceptor));
    }


    /// <summary>
    /// Register specified interceptor.
    /// </summary>
    /// <param name="interceptor">The interceptor to register.</param>
    /// <param name="order">The order for the registered interceptor in the interceptor chain.</param>
    /// <returns>The interceptor chain builder with registered intercetor.</returns>
    /// <exception cref="ArgumentNullException">The argument <paramref name="interceptor"/> is null.</exception>
    public IPipelineChainBuilder Use(PipelineDelegate interceptor)
    {
        Check.NotNull(interceptor, nameof(interceptor));
        _interceptors.Add(interceptor);
        return this;
    }

    /// <summary>
    /// Build an interceptor chain using the registerd interceptors.
    /// </summary>
    /// <returns>A composite interceptor representing the interceptor chain.</returns>
    public PipelineDelegate Build()
    {
        if (_interceptors.IsNullOrEmpty())
        {
            return next => context => next(context);
        }

        if (_interceptors.Count == 1)
        {
            return _interceptors.Single();
        }

        var interceptors = _interceptors.Reverse().ToArray();

        return next =>
        {
            var current = next;
            for (var index = 0; index < interceptors.Length; index++)
            {
                current = interceptors[index](current);
            }
            return current;
        };
    }

    /// <summary>
    /// Create a new interceptor chain builder which owns the same service provider as the current one.
    /// </summary>
    /// <returns>The new interceptor to create.</returns>
    public IPipelineChainBuilder New()
    {
        return new PipelineChainBuilder(ServiceProvider);
    }
}
