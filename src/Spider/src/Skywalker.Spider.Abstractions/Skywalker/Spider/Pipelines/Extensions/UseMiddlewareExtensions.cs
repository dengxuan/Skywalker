using Microsoft.Extensions.DependencyInjection;
using Skywalker.Spider.Pipelines.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Skywalker.Spider.Pipelines.Extensions;

/// <summary>
/// Define some extension methods specific to <see cref="IPipelineChainBuilder"/>.
/// </summary>
public static class UseMiddlewareExtensions
{
    internal const string InvokeMethodName = "InvokeAsync";

    private static readonly MethodInfo GetServiceInfo = typeof(UseMiddlewareExtensions).GetMethod(nameof(GetService), BindingFlags.NonPublic | BindingFlags.Static)!;

    // We're going to keep all public constructors and public methods on middleware
    private const DynamicallyAccessedMemberTypes MiddlewareAccessibility = DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods;

    /// <summary>
    /// Adds a middleware type to the application's request pipeline.
    /// </summary>
    /// <typeparam name="TMiddleware">The middleware type.</typeparam>
    /// <param name="app">The <see cref="IApplicationBuilder"/> instance.</param>
    /// <param name="args">The arguments to pass to the middleware type instance's constructor.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
    public static IPipelineChainBuilder UseMiddleware<[DynamicallyAccessedMembers(MiddlewareAccessibility)]TMiddleware>(this IPipelineChainBuilder app, params object?[] args)
    {
        return app.UseMiddleware(typeof(TMiddleware), args);
    }

    /// <summary>
    /// Adds a middleware type to the application's request pipeline.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> instance.</param>
    /// <param name="middleware">The middleware type.</param>
    /// <param name="args">The arguments to pass to the middleware type instance's constructor.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
    public static IPipelineChainBuilder UseMiddleware(this IPipelineChainBuilder app, [DynamicallyAccessedMembers(MiddlewareAccessibility)] Type middleware, params object?[] args)
    {
        var applicationServices = app.ApplicationServices;
        return app.Use(next =>
        {
            var methods = middleware.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            var invokeMethods = methods.Where(m => string.Equals(m.Name, InvokeMethodName, StringComparison.Ordinal)).ToArray();

            if (invokeMethods.Length > 1)
            {
                throw new InvalidOperationException($"Multiple public '{InvokeMethodName}' methods are available.");
            }

            if (invokeMethods.Length == 0)
            {
                throw new InvalidOperationException($"No public '{InvokeMethodName}' method found for middleware of type '{middleware}'.");
            }

            var methodInfo = invokeMethods[0];
            if (!typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
            {
                throw new InvalidOperationException($"'{InvokeMethodName}' does not return an object of type '{ nameof(Task)}'.");
            }

            var parameters = methodInfo.GetParameters();
            if (parameters.Length == 0 || parameters[0].ParameterType != typeof(PipelineContext))
            {
                throw new InvalidOperationException($"The '{InvokeMethodName}' method's first argument must be of type '{nameof(PipelineContext)}'.");
            }

            var ctorArgs = new object[args.Length + 1];
            ctorArgs[0] = next;
            Array.Copy(args, 0, ctorArgs, 1, args.Length);
            var instance = ActivatorUtilities.CreateInstance(app.ApplicationServices, middleware, ctorArgs);
            if (parameters.Length == 1)
            {
                return (PipelineDelegate)methodInfo.CreateDelegate(typeof(PipelineDelegate), instance);
            }

            var factory = Compile<object>(methodInfo, parameters);

            return context =>
            {
                var serviceProvider = context.PipelineServices ?? applicationServices;
                if (serviceProvider == null)
                {
                    throw new InvalidOperationException($"'{nameof(IServiceProvider)}' is not available.");
                }

                return factory(instance, context, serviceProvider);
            };
        });
    }

    private static Func<T, PipelineContext, IServiceProvider, Task> Compile<T>(MethodInfo methodInfo, ParameterInfo[] parameters)
    {
        // If we call something like
        //
        // public class Middleware
        // {
        //    public Task Invoke(HttpContext context, ILoggerFactory loggerFactory)
        //    {
        //
        //    }
        // }
        //

        // We'll end up with something like this:
        //   Generic version:
        //
        //   Task Invoke(Middleware instance, HttpContext httpContext, IServiceProvider provider)
        //   {
        //      return instance.Invoke(httpContext, (ILoggerFactory)UseMiddlewareExtensions.GetService(provider, typeof(ILoggerFactory));
        //   }

        //   Non generic version:
        //
        //   Task Invoke(object instance, HttpContext httpContext, IServiceProvider provider)
        //   {
        //      return ((Middleware)instance).Invoke(httpContext, (ILoggerFactory)UseMiddlewareExtensions.GetService(provider, typeof(ILoggerFactory));
        //   }

        var middleware = typeof(T);

        var httpContextArg = Expression.Parameter(typeof(PipelineContext), "pipelineContext");
        var providerArg = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");
        var instanceArg = Expression.Parameter(middleware, "middleware");

        var methodArguments = new Expression[parameters.Length];
        methodArguments[0] = httpContextArg;
        for (int i = 1; i < parameters.Length; i++)
        {
            var parameterType = parameters[i].ParameterType;
            if (parameterType.IsByRef)
            {
                throw new NotSupportedException($"The '{InvokeMethodName}' method must not have ref or out parameters.");
            }

            var parameterTypeExpression = new Expression[]
            {
                    providerArg,
                    Expression.Constant(parameterType, typeof(Type)),
                    Expression.Constant(methodInfo.DeclaringType, typeof(Type))
            };

            var getServiceCall = Expression.Call(GetServiceInfo, parameterTypeExpression);
            methodArguments[i] = Expression.Convert(getServiceCall, parameterType);
        }

        Expression middlewareInstanceArg = instanceArg;
        if (methodInfo.DeclaringType != null && methodInfo.DeclaringType != typeof(T))
        {
            middlewareInstanceArg = Expression.Convert(middlewareInstanceArg, methodInfo.DeclaringType);
        }

        var body = Expression.Call(middlewareInstanceArg, methodInfo, methodArguments);

        var lambda = Expression.Lambda<Func<T, PipelineContext, IServiceProvider, Task>>(body, instanceArg, httpContextArg, providerArg);

        return lambda.Compile();
    }

    private static object GetService(IServiceProvider sp, Type type, Type middleware)
    {
        var service = sp.GetService(type);
        if (service == null)
        {
            throw new InvalidOperationException($"Unable to resolve service for type '{type}' while attempting to Invoke middleware '{middleware}'.");
        }

        return service;
    }
}
