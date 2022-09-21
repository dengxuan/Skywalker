using Skywalker.Aspects.Abstractinons;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Skywalker.Aspects
{
    /// <summary>
    /// An attribute based interceptor provider.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public abstract class InterceptorAttribute : Attribute, IInterceptorProvider, IOrderedSequenceItem
    {
        private readonly ConcurrentBag<Attribute> _attributes = new();
        private bool? _allowMultiple;

        /// <summary>
        /// The order for the registered interceptor in the interceptor chain.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Indicate whether multiple interceptors of the same type can be applied to a single one method.
        /// </summary>
        public bool AllowMultiple
        {
            get
            {
                return _allowMultiple ?? (_allowMultiple = false).Value;
            }
        }

        /// <summary>
        /// Register the provided interceptor to the specified interceptor chain builder.
        /// </summary>
        /// <param name="builder">The interceptor chain builder to which the provided interceptor is registered.</param>
        public abstract void Use(IInterceptorChainBuilder builder);
    }
}