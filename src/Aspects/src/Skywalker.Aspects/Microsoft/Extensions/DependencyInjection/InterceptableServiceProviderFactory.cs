using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Aspects.Abstractinons;
using System;
using System.Linq;

namespace Skywalker.Aspects
{
    /// <summary>
    /// Represents the custom <see cref="IServiceProviderFactory{TContainerBuilder}"/> for interception extensions.
    /// </summary>
    public sealed class InterceptableServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        private readonly Action<InterceptionBuilder>? _configure;
        private readonly ServiceProviderOptions? _options;

        /// <summary>
        /// Create a new <see cref="InterceptableServiceProviderFactory"/>.
        /// </summary>
        /// <param name="options">Options for configuring various behaviors of the default <see cref="IServiceProvider"/> implementation.</param>
        /// <param name="configure">The <see cref="Action{InterceptionBuilder}"/> used to perform more service registrations.</param>
        public InterceptableServiceProviderFactory(ServiceProviderOptions? options, Action<InterceptionBuilder>? configure)
        {
            _configure = configure;
            _options = options;
        }

        /// <summary>
        /// Creates a container builder from an Microsoft.Extensions.DependencyInjection.IServiceCollection.
        /// </summary>
        /// <param name="services">The contract for a collection of service descriptors.</param>
        /// <returns>The <see cref="IServiceCollection"/> with interception based service registrations.</returns>
        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));
            IServiceCollection newServices = new ServiceCollection();

            newServices.TryAddAspects(_configure);

            var resolver = newServices.GetSingletonInstance<IInterceptorResolver>();
            var codeGeneratorFactory = newServices.GetSingletonInstance<ICodeGeneratorFactory>();
            var factoryCache = newServices.GetSingletonInstance<IInterceptableProxyFactoryCache>();

            foreach (var service in services)
            {
                foreach (var newService in new ServiceDescriptorConverter(service, resolver, factoryCache, codeGeneratorFactory).AsServiceDescriptors())
                {
                    newServices.Add(newService);
                }
            }
            return newServices;
        }

        /// <summary>
        /// Creates an <see cref="IServiceProvider"/> from the container builder.
        /// </summary>
        /// <param name="containerBuilder">The <see cref="IServiceCollection"/> with interception based service registrations.</param>
        /// <returns>The created <see cref="IServiceProvider"/>.</returns>
        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            IServiceProvider provider = _options == null
                       ? containerBuilder.BuildServiceProvider()
                       : containerBuilder.BuildServiceProvider(_options);
            Console.WriteLine("provider:{0}", provider.GetHashCode());
            return provider;
        }
    }
}
