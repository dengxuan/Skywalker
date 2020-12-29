﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Skywalker.Aspects.Interceptors
{
    /// <summary>
    /// The default implementation of interceptor chain builder.
    /// </summary>
    public class InterceptorChainBuilder : IInterceptorChainBuilder
    {
        private readonly List<Tuple<int, InterceptorDelegate>> _interceptors;

        /// <summary>
        /// Gets the service provider to get dependency services.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Create a new <see cref="InterceptorChainBuilder"/>.
        /// </summary>
        /// <param name="serviceProvider">The service provider to get dependency services.</param>
        /// <exception cref="ArgumentNullException">The argument <paramref name="serviceProvider"/> is null.</exception>
        public InterceptorChainBuilder(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            ServiceProvider = serviceProvider;
            _interceptors = new List<Tuple<int, InterceptorDelegate>>();
        }


        /// <summary>
        /// Register specified interceptor.
        /// </summary>
        /// <param name="interceptor">The interceptor to register.</param>
        /// <param name="order">The order for the registered interceptor in the interceptor chain.</param>
        /// <returns>The interceptor chain builder with registered intercetor.</returns>
        /// <exception cref="ArgumentNullException">The argument <paramref name="interceptor"/> is null.</exception>
        public IInterceptorChainBuilder Use(InterceptorDelegate interceptor, int order)
        {
            Check.NotNull(interceptor, nameof(interceptor));
            _interceptors.Add(new Tuple<int, InterceptorDelegate>(order, interceptor));
            return this;
        }

        /// <summary>
        /// Build an interceptor chain using the registerd interceptors.
        /// </summary>
        /// <returns>A composite interceptor representing the interceptor chain.</returns>
        public InterceptorDelegate Build()
        {
            var result = _interceptors.OrderBy(keySelector => keySelector.Item1).Select(selector => selector.Item2);
            return next =>
            {
                var current = next;
                foreach (var it in result.Reverse())
                {
                    current = it(current);
                }
                return current;
            };
        }

        /// <summary>
        /// Create a new interceptor chain builder which owns the same service provider as the current one.
        /// </summary>
        /// <returns>The new interceptor to create.</returns>
        public IInterceptorChainBuilder New()
        {
            return new InterceptorChainBuilder(this.ServiceProvider);
        }
    }
}
