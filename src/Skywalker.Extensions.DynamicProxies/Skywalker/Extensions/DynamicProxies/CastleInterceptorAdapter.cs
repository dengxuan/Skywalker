using Castle.DynamicProxy;

namespace Skywalker.Extensions.DynamicProxies;

/// <summary>
/// Castle.DynamicProxy 拦截器适配器，将 Skywalker 拦截器链桥接到 Castle。
/// </summary>
internal sealed class CastleInterceptorAdapter : Castle.DynamicProxy.IInterceptor
{
    private readonly IEnumerable<IInterceptor> _interceptors;

    /// <summary>
    /// 初始化 <see cref="CastleInterceptorAdapter"/> 类的新实例。
    /// </summary>
    /// <param name="interceptors">Skywalker 拦截器列表。</param>
    public CastleInterceptorAdapter(IEnumerable<IInterceptor> interceptors)
    {
        _interceptors = interceptors ?? throw new ArgumentNullException(nameof(interceptors));
    }

    /// <inheritdoc/>
    public void Intercept(IInvocation invocation)
    {
        // 同步执行异步拦截
        InterceptAsync(invocation).GetAwaiter().GetResult();
    }

    private async Task InterceptAsync(IInvocation invocation)
    {
        // 构建拦截器链
        var interceptorList = _interceptors.ToList();
        var index = 0;

        async Task ProceedAsync()
        {
            if (index < interceptorList.Count)
            {
                var current = interceptorList[index++];
                var methodInvocation = new MethodInvocation(
                    invocation.InvocationTarget,
                    invocation.Method,
                    invocation.Arguments,
                    ProceedAsync);

                await current.InterceptAsync(methodInvocation);

                // 同步返回值
                if (methodInvocation.ReturnValue != null)
                {
                    invocation.ReturnValue = methodInvocation.ReturnValue;
                }
            }
            else
            {
                // 调用实际方法
                invocation.Proceed();

                // 处理异步方法
                if (invocation.ReturnValue is Task task)
                {
                    await task;
                }
            }
        }

        await ProceedAsync();
    }
}
