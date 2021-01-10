using Skywalker;
using System;

namespace Skywalker.Aspects.DynamicProxy
{
    /// <summary>
    /// Interface based instance dynamic proxy generator.
    /// </summary>
    /// <seealso cref="Dora.DynamicProxy.IInstanceDynamicProxyGenerator" />
    public class InterfaceDynamicProxyGenerator : IInstanceDynamicProxyGenerator
    {
        private readonly IDynamicProxyFactoryCache _dynamicProxyFactoryCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterfaceDynamicProxyGenerator"/> class.
        /// </summary>
        /// <param name="dynamicProxyFactoryCache">The dynamic proxy factory cache.</param>
        public InterfaceDynamicProxyGenerator(IDynamicProxyFactoryCache dynamicProxyFactoryCache)
        {
            _dynamicProxyFactoryCache = Check.NotNull(dynamicProxyFactoryCache, nameof(dynamicProxyFactoryCache));
        }

        /// <summary>
        /// Determines whether this specified type can be intercepted.
        /// </summary>
        /// <param name="type">The type to intercept.</param>
        /// <returns>
        ///   <c>true</c> if the specified type can be intercepted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="type"/> is null.</exception>
        public bool CanIntercept(Type type)
        {
            return Check.NotNull(type, nameof(type)).IsInterface;
        }

        /// <summary>
        /// Creates a new interceptable dynamic proxy to wrap the specficied target instance.
        /// </summary>
        /// <param name="type">The interface type to intercept.</param>
        /// <param name="target">The target instance wrapped by the proxy.</param>
        /// <param name="interceptors">The <see cref="InterceptorRegistry" /> representing the type members decorated with interceptors.</param>
        /// <returns>
        /// The generated interceptable dynamic proxy.
        /// </returns>     
        /// <exception cref="ArgumentNullException"> <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentNullException"> <paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentNullException"> <paramref name="interceptors"/> is null.</exception>
        public object Wrap(Type type, object target, InterceptorRegistry interceptors)
        {
            Check.NotNull(type, nameof(type));
            Check.NotNull(target, nameof(target));
            Check.NotNull(interceptors, nameof(interceptors));

            if (CanIntercept(type))
            {
                var factory = _dynamicProxyFactoryCache.GetInstanceFactory(type, interceptors);
                return factory(target, interceptors);
            }

            return target;
        }
    }
}
