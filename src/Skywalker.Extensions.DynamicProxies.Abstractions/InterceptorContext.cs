// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Skywalker.Extensions.DynamicProxies;

/// <summary>
/// 简化的拦截器上下文，用于中间件风格的拦截器管道。
/// </summary>
public sealed class InterceptorContext : IMethodInvocation
{
    private readonly Func<Task>? _proceedAction;

    /// <inheritdoc/>
    public object Target { get; }

    /// <inheritdoc/>
    public MethodInfo Method { get; }

    /// <inheritdoc/>
    public string MethodName { get; }

    /// <inheritdoc/>
    public object?[] Arguments { get; }

    /// <inheritdoc/>
    public Type ReturnType { get; }

    /// <inheritdoc/>
    public object? ReturnValue { get; set; }

    /// <summary>
    /// 初始化 <see cref="InterceptorContext"/> 类的新实例。
    /// </summary>
    /// <param name="target">目标对象。</param>
    /// <param name="methodName">方法名称。</param>
    public InterceptorContext(object target, string methodName)
    {
        Target = target ?? throw new ArgumentNullException(nameof(target));
        MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
        Arguments = Array.Empty<object?>();
        Method = null!; // 简化实现，不需要 MethodInfo
        ReturnType = typeof(object);
    }

    /// <summary>
    /// 初始化 <see cref="InterceptorContext"/> 类的新实例。
    /// </summary>
    /// <param name="target">目标对象。</param>
    /// <param name="method">方法信息。</param>
    /// <param name="arguments">方法参数。</param>
    public InterceptorContext(object target, MethodInfo method, object?[] arguments)
    {
        Target = target ?? throw new ArgumentNullException(nameof(target));
        Method = method ?? throw new ArgumentNullException(nameof(method));
        MethodName = method.Name;
        Arguments = arguments ?? Array.Empty<object?>();
        ReturnType = method.ReturnType;
    }

    /// <summary>
    /// 使用 proceed 委托初始化 <see cref="InterceptorContext"/> 类的新实例。
    /// </summary>
    internal InterceptorContext(object target, MethodInfo method, object?[] arguments, Func<Task> proceedAction)
        : this(target, method, arguments)
    {
        _proceedAction = proceedAction;
    }

    /// <inheritdoc/>
    public Task ProceedAsync()
    {
        if (_proceedAction == null)
        {
            throw new InvalidOperationException("ProceedAsync is not available in this context. Use the InterceptorChainBuilder pipeline instead.");
        }
        return _proceedAction();
    }
}
