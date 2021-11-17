namespace Skywalker.Aspects.Abstractions;

/// <summary>
/// Interface methods Interceptor
/// </summary>
public interface IAspectsInterceptor
{
    /// <summary>
    /// Intercept the method call
    /// </summary>
    /// <param name="invocation">An invocation of a proxied method</param> 
    /// <param name="arguments">action的参数集合</param>
    /// <returns></returns>
    object Intercept(IInvocation invocation, object?[] arguments);
}
