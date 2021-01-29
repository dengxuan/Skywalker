using Microsoft.Extensions.DependencyInjection;
using Skywalker.Aspects.Interceptors;
using System;

namespace Skywalker.Aspects
{
    /// <summary>
    /// Represents the custom <see cref="IServiceProviderFactory{TContainerBuilder}"/> for interception extensions.
    /// </summary>
    public sealed class InterceptableServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        private readonly ServiceProviderOptions? _options;

        /// <summary>
        /// Create a new <see cref="InterceptableServiceProviderFactory"/>.
        /// </summary>
        /// <param name="options">Options for configuring various behaviors of the default <see cref="IServiceProvider"/> implementation.</param>
        /// <param name="configure">The <see cref="Action{InterceptionBuilder}"/> used to perform more service registrations.</param>
        public InterceptableServiceProviderFactory(ServiceProviderOptions? options)
        {
            _options = options;
        }

        private object CreateInstance(IServiceProvider provider, Type serviceType, object implementationInstance)
        {
            IInterceptorFactory builder = provider.GetRequiredService<IInterceptorFactory>();
            return builder.CreateProxy(serviceType, implementationInstance);
        }

        /// <summary>
        /// Creates a container builder from an Microsoft.Extensions.DependencyInjection.IServiceCollection.
        /// </summary>
        /// <param name="services">The contract for a collection of service descriptors.</param>
        /// <returns>The <see cref="IServiceCollection"/> with interception based service registrations.</returns>
        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            IServiceCollection aspects = new ServiceCollection();
            aspects.AddAspects();
            aspects.AddOptions();

            foreach (var service in services)
            {
                if (!ProxyHelper.ShouldProxy(service.ServiceType))
                {
                    aspects.Add(service);
                    continue;
                }
                if (service.ImplementationType != null)
                {
                    aspects.Add(ServiceDescriptor.Describe(service.ServiceType, sp =>
                    {
                        object instance = ActivatorUtilities.CreateInstance(sp, service.ImplementationType);
                        return CreateInstance(sp, service.ServiceType, instance);
                    }, service.Lifetime));
                }
                else if (service.ImplementationInstance != null)
                {
                    aspects.Add(ServiceDescriptor.Describe(service.ServiceType, sp => CreateInstance(sp, service.ServiceType, service.ImplementationInstance), service.Lifetime));

                }
                else if (service.ImplementationFactory != null)
                {
                    aspects.Add(ServiceDescriptor.Describe(service.ServiceType, sp =>
                    {
                        var instance = service.ImplementationFactory.Invoke(sp);
                        return CreateInstance(sp, service.ServiceType, instance);
                    }, service.Lifetime));
                }
            }
            return aspects;
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
