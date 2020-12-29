namespace Skywalker.Aspects.Interceptors
{
    /// <summary>
    /// Represents interceptor provider.
    /// </summary>
    public interface IInterceptorProvider
    {
        /// <summary>
        /// Register the provided interceptor to the specified interceptor chain builder.
        /// </summary>
        /// <param name="builder">The interceptor chain builder to which the provided interceptor is registered.</param>
        void Use(IInterceptorChainBuilder builder);
    }
}
