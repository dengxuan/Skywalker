﻿using Skywalker;
using System;

namespace Skywalker.Aspects.DynamicProxy
{
    /// <summary>
    /// Virtual method interception based type dynamic proxy generator.
    /// </summary>                                                          
    public class VirtualMethodDynamicProxyGenerator : ITypeDynamicProxyGenerator
    {
        #region Fields
        private readonly IDynamicProxyFactoryCache _dynamicProxyFactoryCache;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualMethodDynamicProxyGenerator"/> class.
        /// </summary>
        /// <param name="dynamicProxyFactoryCache">The dynamic proxy factory cache.</param>
        public VirtualMethodDynamicProxyGenerator(IDynamicProxyFactoryCache dynamicProxyFactoryCache)
        {
            _dynamicProxyFactoryCache = Check.NotNull(dynamicProxyFactoryCache, nameof(dynamicProxyFactoryCache));
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether this specified type can be intercepted.
        /// </summary>
        /// <param name="type">The type to intercept.</param>
        /// <returns>
        ///   <c>true</c> if the specified type can be intercepted; otherwise, <c>false</c>.
        /// </returns>
        public bool CanIntercept(Type type)
        {
            return !Check.NotNull(type, nameof(type)).IsSealed;
        }

        /// <summary>
        /// Creates a new interceptable dynamic proxy.
        /// </summary>
        /// <param name="type">The type to intercept.</param>
        /// <param name="interceptors">The <see cref="InterceptorRegistry" /> representing the type members decorated with interceptors.</param>
        /// <param name="serviceProvider">The <see cref="IServiceProvider" /> helping the creating object.</param>
        /// <returns>
        /// The interceptable dynamic proxy.
        /// </returns>
        public object Create(Type type, InterceptorRegistry interceptors, IServiceProvider serviceProvider)
        {
            Check.NotNull(type, nameof(type));
            Check.NotNull(interceptors, nameof(interceptors));
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            if (CanIntercept(type) && !interceptors.IsEmpty)
            {
                var factory = _dynamicProxyFactoryCache.GetTypeFactory(type, interceptors);
                return factory(interceptors, serviceProvider);
            }
            return serviceProvider.GetService(type);
        }
        #endregion
    }
} 