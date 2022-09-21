using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Skywalker.Aspects
{
    public static class ServiceDescriptorExtensions
    {
        private static readonly Assembly _assembly = typeof(ServiceDescriptorExtensions).Assembly;
        public static bool IsInterceptable(this ServiceDescriptor serviceDescriptor)
        {
            Check.NotNull(serviceDescriptor, nameof(serviceDescriptor));
            return serviceDescriptor.ImplementationFactory?.Method?.DeclaringType?.Assembly == _assembly;
        }

        /// <summary>
        /// Method searches <see cref="IServiceCollection"/> for all registered services implementing <see cref="TInterface"/>,
        /// and swaps them with <see cref="TProxy"/> based decorator. Note that created instance will inherit from
        /// <see cref="TProxy" /> (which inherits from <see cref="DispatchProxy"/>) AND implements supplied <see cref="TInterface"/>.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <typeparam name="TInterface">Service type, which you want to decorate.</typeparam>
        /// <typeparam name="TProxy">DispatchProxy child, a decorator.</typeparam>
        /// <returns>Modified service collection</returns>
        /// <exception cref="InvalidOperationException"><see cref="TProxy"/> needs to have a static method
        /// with output type of <see cref="TInterface"/> (usually called "Create")</exception>
        public static IServiceCollection DecorateWithDispatchProxy<TInterface, TProxy>(this IServiceCollection services)
            where TInterface : class
            where TProxy : class, TInterface
        {
            //MethodInfo createMethod;
            //try
            //{
            //    createMethod = typeof(TProxy)
            //        .GetMethods(BindingFlags.Public | BindingFlags.Static)
            //        .First(info => !info.IsGenericMethod && info.ReturnType == typeof(TInterface));
            //}
            //catch (InvalidOperationException e)
            //{
            //    throw new InvalidOperationException($"Looks like there is no static method in {typeof(TProxy)} " +
            //                                        $"which creates instance of {typeof(TInterface)} (note that this method should not be generic)", e);
            //}

            //var argInfos = createMethod.GetParameters();

            // Save all descriptors that needs to be decorated into a list.
            var descriptorsToDecorate = services
                .Where(s => s.ServiceType == typeof(TInterface))
                .ToList();

            if (descriptorsToDecorate.Count == 0)
            {
                throw new InvalidOperationException($"Attempted to Decorate services of type {typeof(TInterface)}, " +
                                                    "but no such services are present in ServiceCollection");
            }

            foreach (var descriptor in descriptorsToDecorate)
            {
                var decorated = ServiceDescriptor.Describe(
                    typeof(TInterface),
                    sp =>
                    {
                        var target = sp.CreateInstance(descriptor);
                        var decoratorInstance = ProxyGenerator.CreateProxyInstance(sp, descriptor);

                        return (TInterface)decoratorInstance;
                    },
                    descriptor.Lifetime);

                services.Remove(descriptor);
                services.Add(decorated);
            }

            return services;
        }


        internal static object CreateInstance(this IServiceProvider services, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationInstance != null)
            {
                return descriptor.ImplementationInstance;
            }

            if (descriptor.ImplementationFactory != null)
            {
                return descriptor.ImplementationFactory(services);
            }

            return ActivatorUtilities.GetServiceOrCreateInstance(services, descriptor.ImplementationType!);
        }
    }
}