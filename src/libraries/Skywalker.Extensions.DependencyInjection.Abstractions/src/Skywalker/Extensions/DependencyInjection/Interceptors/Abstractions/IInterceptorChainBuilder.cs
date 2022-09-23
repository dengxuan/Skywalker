namespace Skywalker.Extensions.DependencyInjection.Interceptors.Abstractions;

/// <summary>
/// Represents a builder to build an interceptor chain.
/// </summary>
public interface IInterceptorChainBuilder
{
    /// <summary>
    /// Register specified interceptor.
    /// </summary>
    /// <param name="interceptor">The interceptor to register.</param>
    /// <returns>The interceptor chain builder with registered intercetor.</returns>
    IInterceptorChainBuilder Use(InterceptorDelegate interceptor);

    IInterceptorChainBuilder Use<TInterceptor>();

    /// <summary>
    /// Build an interceptor chain using the registerd interceptors.
    /// </summary>
    /// <returns>A composite interceptor representing the interceptor chain.</returns>
    InterceptorDelegate Build();

    /// <summary>
    /// Create a new interceptor chain builder which owns the same service provider as the current one.
    /// </summary>
    /// <returns>The new interceptor to create.</returns>
    IInterceptorChainBuilder New();
}
