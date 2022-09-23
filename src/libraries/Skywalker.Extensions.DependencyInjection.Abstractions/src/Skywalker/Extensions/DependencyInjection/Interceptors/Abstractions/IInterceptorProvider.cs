using System;
using System.Collections.Generic;
using System.Text;

namespace Skywalker.Extensions.DependencyInjection.Interceptors.Abstractions
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

        /// <summary>
        /// Indicate whether multiple interceptors of the same type can be applied to a single one method.
        /// </summary>
        bool AllowMultiple { get; }
    }
}
