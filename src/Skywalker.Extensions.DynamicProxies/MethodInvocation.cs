using System.Reflection;

namespace Skywalker.Extensions.DynamicProxies;

/// <summary>
/// <see cref="IMethodInvocation"/> 的默认实现。
/// </summary>
internal sealed class MethodInvocation : IMethodInvocation
{
    private readonly Func<Task> _proceed;

    /// <summary>
    /// 初始化 <see cref="MethodInvocation"/> 类的新实例。
    /// </summary>
    public MethodInvocation(
        object target,
        MethodInfo method,
        object?[] arguments,
        Func<Task> proceed)
    {
        Target = target;
        Method = method;
        Arguments = arguments;
        _proceed = proceed;
    }

    /// <inheritdoc/>
    public object Target { get; }

    /// <inheritdoc/>
    public MethodInfo Method { get; }

    /// <inheritdoc/>
    public string MethodName => Method.Name;

    /// <inheritdoc/>
    public object?[] Arguments { get; }

    /// <inheritdoc/>
    public Type ReturnType => Method.ReturnType;

    /// <inheritdoc/>
    public object? ReturnValue { get; set; }

    /// <inheritdoc/>
    public Task ProceedAsync() => _proceed();
}
