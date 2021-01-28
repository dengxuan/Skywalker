using Microsoft.AspNetCore.Builder;
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
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Add interception based services. 
        /// </summary>
        /// <param name="services">The contract for a collection of service descriptors.</param> 
        /// <param name="configure">The <see cref="Action{InterceptionBuilder}"/> used to perform more service registrations.</param>
        /// <returns>The current <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddAspNetCoreAspects(this IServiceCollection services, Action<InterceptionBuilder>? configure = null)
        {
            Check.NotNull(services, nameof(services));
            services.TryAddAspects(configure);
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IScopedServiceProviderAccesssor, HttpContextScopedServiceProviderAccessor>();
            return services;
        }
    }
}
