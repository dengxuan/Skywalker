using Skywalker.Aspects.Abstractinons;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Skywalker.Aspects
{
    /// <summary>
    /// An attribute based interceptor provider.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class InterceptorAttribute : Attribute, IInterceptorProvider, IOrderedSequenceItem
    {
        /// <summary>
        /// The order for the registered interceptor in the interceptor chain.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The type for the registerd interceptor in the interceptor chain.
        /// </summary>
        public Type InterceptorType { get; set; }

        public InterceptorAttribute(Type interceptorType)
        {
            InterceptorType = interceptorType;
        }

        /// <summary>
        /// Register the provided interceptor to the specified interceptor chain builder.
        /// </summary>
        /// <param name="builder">The interceptor chain builder to which the provided interceptor is registered.</param>
        public void Use(IInterceptorChainBuilder builder)
        {
            //builder.Use(InterceptorType, 1);
        }
    }
}