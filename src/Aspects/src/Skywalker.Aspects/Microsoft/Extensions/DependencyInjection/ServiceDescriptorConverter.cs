using Microsoft.Extensions.DependencyInjection;
using Skywalker.Aspects.Abstractinons;
using System;

namespace Skywalker.Aspects
{
    internal sealed class ServiceDescriptorConverter
    {
        private readonly ServiceDescriptor _primaryDescriptor;
        private readonly ServiceDescriptor? _secondaryDescriptor = null;
        public ServiceDescriptorConverter(
            Type serviceType,
            Type implementationType,
            ServiceLifetime lifetime,
            IInterceptorResolver interceptorResolver,
            IInterceptableProxyFactoryCache factoryCache,
            ICodeGeneratorFactory codeGeneratorFactory)
            : this(new ServiceDescriptor(serviceType, implementationType, lifetime), interceptorResolver, factoryCache, codeGeneratorFactory)
        { }

        public ServiceDescriptorConverter(
            ServiceDescriptor serviceDescriptor,
            IInterceptorResolver interceptorResolver,
            IInterceptableProxyFactoryCache factoryCache,
            ICodeGeneratorFactory codeGeneratorFactory)
        {
            Check.NotNull(serviceDescriptor, nameof(serviceDescriptor));
            Check.NotNull(interceptorResolver, nameof(interceptorResolver));
            Check.NotNull(factoryCache, nameof(factoryCache));

            if (serviceDescriptor.ImplementationInstance != null || serviceDescriptor.ImplementationFactory != null || serviceDescriptor.IsInterceptable())
            {
                _primaryDescriptor = serviceDescriptor;
                return;
            }
            var serviceType = serviceDescriptor.ServiceType;
            var implementationType = serviceDescriptor.ImplementationType;
            if (implementationType == null)
            {
                throw new ArgumentException(nameof(serviceDescriptor.ServiceType), $"The implementation typeservice of service type '{serviceType}' can't be null!");
            }
            var lifetime = serviceDescriptor.Lifetime;

            if (serviceType.IsInterface)
            {
                var interceptors = interceptorResolver.GetInterceptors(serviceType, implementationType);
                if (interceptors.IsEmpty)
                {
                    _primaryDescriptor = new ServiceDescriptor(serviceType, implementationType, lifetime);
                }
                else if (serviceType.IsGenericTypeDefinition)
                {
                    _secondaryDescriptor = new ServiceDescriptor(implementationType, implementationType, lifetime);
                    var codeGenerator = codeGeneratorFactory.Create();
                    var context = new CodeGenerationContext(serviceType, implementationType, interceptors);
                    var proxyType = codeGenerator.GenerateInterceptableProxyClass(context);
                    _primaryDescriptor = new ServiceDescriptor(serviceType, proxyType, lifetime);
                }
                else
                {
                    _primaryDescriptor = new ServiceDescriptor(serviceType, Factory, lifetime);
                    object Factory(IServiceProvider serviceProvider)
                    {
                        Console.WriteLine("serviceProvider:{0}", serviceProvider.GetHashCode());
                        var target = serviceProvider.GetRequiredService(implementationType);
                        return factoryCache.GetInstanceFactory(serviceType, implementationType).Invoke(target);
                    }
                }
            }
            else
            {
                var interceptors = interceptorResolver.GetInterceptors(implementationType);
                if (interceptors.IsEmpty)
                {
                    _primaryDescriptor = new ServiceDescriptor(serviceType, implementationType, lifetime);
                }
                else
                {
                    var codeGenerator = codeGeneratorFactory.Create();
                    var context = new CodeGenerationContext(implementationType, interceptors);
                    var proxyType = codeGenerator.GenerateInterceptableProxyClass(context);
                    _primaryDescriptor = new ServiceDescriptor(serviceType, proxyType, lifetime);
                }
            }
        }

        public ServiceDescriptor[] AsServiceDescriptors()
        {
            if (_secondaryDescriptor == null)
            {
                return new ServiceDescriptor[] { _primaryDescriptor };
            }
            return new ServiceDescriptor[] { _primaryDescriptor, _secondaryDescriptor };
        }
    }
}
