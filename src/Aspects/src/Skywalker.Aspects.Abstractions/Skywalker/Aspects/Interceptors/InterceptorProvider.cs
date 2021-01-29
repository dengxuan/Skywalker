namespace Skywalker.Aspects.Interceptors
{
    /// <summary>
    /// An attribute based interceptor provider.
    /// </summary>
    public abstract class InterceptorProvider : IInterceptorProvider
    {

        /// <summary>
        /// The order for the registered interceptor in the interceptor chain.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Register the provided interceptor to the specified interceptor chain builder.
        /// </summary>
        /// <param name="builder">The interceptor chain builder to which the provided interceptor is registered.</param>
        public abstract void Use(IInterceptorChainBuilder builder);
    }
}
