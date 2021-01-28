using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker;
using Skywalker.Aspects;
using Skywalker.Aspects.Abstractinons;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Defines <see cref="IServiceCollection"/> specific extension methods.
    /// </summary>
    internal static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add interception based services. 
        /// </summary>
        /// <param name="services">The contract for a collection of service descriptors.</param> 
        /// <param name="configure">The <see cref="Action{InterceptionBuilder}"/> used to perform more service registrations.</param>
        /// <returns>The current <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddAspects(this IServiceCollection services, Action<InterceptionBuilder>? configure = null)
        {
            Check.NotNull(services, nameof(services));
            if (services.Any(it => it.ServiceType == typeof(IDuplicate)))
            {
                throw new InvalidOperationException("Duplicate invocation to AddAspects method.");
            }
            var builder = new InterceptionBuilder(services);
            configure?.Invoke(builder);
            if (!builder.InterceptorProviderResolvers.OfType<AttributeInterceptorProviderResolver>().Any())
            {
                builder.InterceptorProviderResolvers.Add(nameof(AttributeInterceptorProviderResolver), new AttributeInterceptorProviderResolver());
            }
            services.TryAddSingleton(builder);
            services.TryAddSingleton<IDuplicate, Duplicate>();
            IInterceptorChainBuilder interceptorChainBuilder = new InterceptorChainBuilder();
            IInterceptorResolver interceptorResolver = new InterceptorResolver(interceptorChainBuilder, builder.InterceptorProviderResolvers);
            ICodeGeneratorFactory codeGeneratorFactory = new CodeGeneratorFactory();
            IInterceptableProxyFactoryCache interceptableProxyFactoryCache = new InterceptableProxyFactoryCache(codeGeneratorFactory, interceptorResolver);
            services.TryAddSingleton(interceptorChainBuilder);
            services.TryAddSingleton(interceptorResolver);
            services.TryAddSingleton(codeGeneratorFactory);
            services.TryAddSingleton(interceptableProxyFactoryCache);
            return services;
        }

        /// <summary>
        /// Try to add interception based services is missing. 
        /// </summary>
        /// <param name="services">The contract for a collection of service descriptors.</param> 
        /// <param name="configure">The <see cref="Action{InterceptionBuilder}"/> used to perform more service registrations.</param>
        /// <returns>The current <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection TryAddAspects(this IServiceCollection services, Action<InterceptionBuilder>? configure = null)
        {
            Check.NotNull(services, nameof(services));
            if (!services.Any(it => it.ServiceType == typeof(IDuplicate)))
            {
                services.AddAspects(configure);
            }
            return services;
        }
        private interface IDuplicate { }
        private class Duplicate : IDuplicate { }
    }
}
