using System.Reflection;
namespace Skywalker.Extensions.DynamicProxies;

/// <summary>
/// <see cref="IMethodInvocation"/> 的默认实现。
/// </summary>
public sealed class MethodInvocation : IMethodInvocation
{
    private readonly IReadOnlyList<IInterceptor> _interceptors;
    private readonly Func<object?[], Task<object?>> _targetInvoker;
    private int _currentInterceptorIndex;

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

    /// <summary>
    /// 初始化 <see cref="MethodInvocation"/> 类的新实例。
    /// </summary>
    /// <param name="target">目标对象。</param>
    /// <param name="method">被调用的方法。</param>
    /// <param name="arguments">方法参数。</param>
    /// <param name="interceptors">拦截器列表。</param>
    /// <param name="targetInvoker">调用实际目标方法的委托。</param>
    public MethodInvocation(
        object target,
        MethodInfo method,
        object?[] arguments,
        IReadOnlyList<IInterceptor> interceptors,
        Func<object?[], Task<object?>> targetInvoker)
    {
        Target = target ?? throw new ArgumentNullException(nameof(target));
        Method = method ?? throw new ArgumentNullException(nameof(method));
        Arguments = arguments ?? Array.Empty<object?>();
        _interceptors = interceptors ?? Array.Empty<IInterceptor>();
        _targetInvoker = targetInvoker ?? throw new ArgumentNullException(nameof(targetInvoker));
        _currentInterceptorIndex = -1;
    }

    /// <inheritdoc/>
    public async Task ProceedAsync()
    {
        _currentInterceptorIndex++;

        if (_currentInterceptorIndex < _interceptors.Count)
        {
            // 调用下一个拦截器
            await _interceptors[_currentInterceptorIndex].InterceptAsync(this);
        }
        else
        {
            // 调用实际的目标方法
            ReturnValue = await _targetInvoker(Arguments);
        }
    }

    /// <summary>
    /// 执行拦截器链。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    public Task ExecuteAsync()
    {
        return ProceedAsync();
    }
}
