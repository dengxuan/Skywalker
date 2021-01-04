using Skywalker.Aspects;
using Skywalker.Aspects.DynamicProxy;
using Skywalker.Aspects.Interceptors;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AspectsServiceCollectionExtensions
    {
        public static IServiceCollection AddAspects(this IServiceCollection services)
        {
            services.AddSingleton<IProxyGenerator, ProxyGenerator>();
            services.AddSingleton<IInterceptorFactory, InterceptorFactory>();
            services.AddSingleton<IInterceptorChainBuilder, InterceptorChainBuilder>();
            services.AddSingleton<InvocationContext>();
            return services;
        }

        private static object CreateInstance(IServiceProvider provider, Type serviceType, object implementationInstance)
        {
            IInterceptorFactory builder = provider.GetRequiredService<IInterceptorFactory>();
            return builder.CreateProxy(serviceType, implementationInstance);
        }

        public static IServiceProvider BuildAspectServiceProvider(this IServiceCollection services)
        {
            IServiceCollection aspects = new ServiceCollection();
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
            return aspects.BuildServiceProvider(); ;
        }
    }
}
