using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Skywalker.Aspects
{
    internal static class ServiceDescriptorExtensions
    {
        private static readonly Assembly _assembly = typeof(ServiceDescriptorExtensions).Assembly;
        public static bool IsInterceptable(this ServiceDescriptor serviceDescriptor)
        {
            Check.NotNull(serviceDescriptor, nameof(serviceDescriptor));
            if (serviceDescriptor.ImplementationFactory?.Method?.DeclaringType?.Assembly == _assembly)
            {
                return true;
            }
            InterceptorAttribute? interceptorAttribute = serviceDescriptor.ServiceType.GetCustomAttribute<InterceptorAttribute>();
            return interceptorAttribute == null;
        }
    }
}