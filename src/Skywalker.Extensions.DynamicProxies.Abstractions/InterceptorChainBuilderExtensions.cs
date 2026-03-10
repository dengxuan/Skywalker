// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Extensions.DynamicProxies;

/// <summary>
/// <see cref="IInterceptorChainBuilder"/> 的扩展方法。
/// </summary>
public static class InterceptorChainBuilderExtensions
{
    /// <summary>
    /// 添加一个拦截器类型到拦截器链。
    /// </summary>
    /// <typeparam name="TInterceptor">拦截器类型。</typeparam>
    /// <param name="builder">构建器实例。</param>
    /// <returns>构建器实例，支持链式调用。</returns>
    /// <remarks>
    /// 拦截器将从 DI 容器中解析。
    /// </remarks>
    public static IInterceptorChainBuilder UseInterceptor<TInterceptor>(this IInterceptorChainBuilder builder)
        where TInterceptor : class, IInterceptor
    {
        return builder.UseInterceptor(typeof(TInterceptor));
    }

    /// <summary>
    /// 添加一个拦截器类型到拦截器链。
    /// </summary>
    /// <param name="builder">构建器实例。</param>
    /// <param name="interceptorType">拦截器类型。</param>
    /// <returns>构建器实例，支持链式调用。</returns>
    /// <remarks>
    /// 拦截器将从 DI 容器中解析并缓存。
    /// </remarks>
    public static IInterceptorChainBuilder UseInterceptor(this IInterceptorChainBuilder builder, Type interceptorType)
    {
        if (!typeof(IInterceptor).IsAssignableFrom(interceptorType))
        {
            throw new ArgumentException($"Type {interceptorType} does not implement {nameof(IInterceptor)}", nameof(interceptorType));
        }

        // 立即解析拦截器实例并缓存
        var interceptor = (IInterceptor)builder.ServiceProvider.GetRequiredService(interceptorType);
        
        return builder.UseInterceptor(interceptor);
    }

    /// <summary>
    /// 添加一个拦截器实例到拦截器链。
    /// </summary>
    /// <param name="builder">构建器实例。</param>
    /// <param name="interceptor">拦截器实例。</param>
    /// <returns>构建器实例，支持链式调用。</returns>
    public static IInterceptorChainBuilder UseInterceptor(this IInterceptorChainBuilder builder, IInterceptor interceptor)
    {
        return builder.Use(next => async context =>
        {
            // 创建一个临时的 invocation 包装器来处理 next 调用
            var wrapper = new InterceptorInvocationWrapper(context, next);
            await interceptor.InterceptAsync(wrapper);
        });
    }

    /// <summary>
    /// 添加多个拦截器到拦截器链。
    /// </summary>
    /// <param name="builder">构建器实例。</param>
    /// <param name="interceptorTypes">拦截器类型列表。</param>
    /// <returns>构建器实例，支持链式调用。</returns>
    public static IInterceptorChainBuilder UseInterceptors(this IInterceptorChainBuilder builder, IEnumerable<Type> interceptorTypes)
    {
        foreach (var type in interceptorTypes)
        {
            builder.UseInterceptor(type);
        }

        return builder;
    }

    /// <summary>
    /// 添加多个拦截器实例到拦截器链。
    /// </summary>
    /// <param name="builder">构建器实例。</param>
    /// <param name="interceptors">拦截器实例列表。</param>
    /// <returns>构建器实例，支持链式调用。</returns>
    /// <remarks>
    /// 拦截器按传入顺序添加，不进行排序。
    /// 排序应由调用方在传入前处理。
    /// </remarks>
    public static IInterceptorChainBuilder UseInterceptors(this IInterceptorChainBuilder builder, IEnumerable<IInterceptor> interceptors)
    {
        foreach (var interceptor in interceptors)
        {
            builder.UseInterceptor(interceptor);
        }

        return builder;
    }

    /// <summary>
    /// 包装器，用于将 InterceptorDelegate 转换为 IMethodInvocation.ProceedAsync。
    /// </summary>
    private sealed class InterceptorInvocationWrapper : IMethodInvocation
    {
        private readonly IMethodInvocation _inner;
        private readonly InterceptorDelegate _next;

        public InterceptorInvocationWrapper(IMethodInvocation inner, InterceptorDelegate next)
        {
            _inner = inner;
            _next = next;
        }

        public object Target => _inner.Target;
        public System.Reflection.MethodInfo Method => _inner.Method;
        public string MethodName => _inner.MethodName;
        public object?[] Arguments => _inner.Arguments;
        public Type ReturnType => _inner.ReturnType;

        public object? ReturnValue
        {
            get => _inner.ReturnValue;
            set => _inner.ReturnValue = value;
        }

        public System.Threading.Tasks.Task ProceedAsync() => _next(_inner);
    }
}
