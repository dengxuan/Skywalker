using Skywalker.Aspects.Abstractinons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Skywalker.Aspects
{
    internal class CompositeInterceptorProviderResolver : IInterceptorProviderResolver
    {
        private readonly IInterceptorProviderResolver[]  _providerResolvers;

        public CompositeInterceptorProviderResolver(IEnumerable<IInterceptorProviderResolver> providerResolvers)
        {
            _providerResolvers = providerResolvers.ToArray();
        }

        public IInterceptorProvider[] GetInterceptorProvidersForType(Type type, out ISet<Type> excludedInterceptorProviders)
        {
            Check.NotNull(type, nameof(type));
            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }

            var providers = new List<IInterceptorProvider>();
            var execluded = new List<Type>();

            for (int index = 0; index < _providerResolvers.Length; index++)
            {
                var resolver = _providerResolvers[index];
                providers.AddRange(resolver.GetInterceptorProvidersForType(type, out var excludedProviders));
                execluded.AddRange(excludedProviders);
            }

            excludedInterceptorProviders = new HashSet<Type>(execluded);
            return providers.ToArray();
        }

        public IInterceptorProvider[] GetInterceptorProvidersForMethod(Type targetType, MethodInfo method, out ISet<Type> excludedInterceptorProviders)
        {
            Check.NotNull(method, nameof(method));

            var providers = new List<IInterceptorProvider>();
            var execluded = new List<Type>();

            for (int index = 0; index < _providerResolvers.Length; index++)
            {
                var resolver = _providerResolvers[index];
                providers.AddRange(resolver.GetInterceptorProvidersForMethod(targetType, method, out var excludedProviders));
                execluded.AddRange(excludedProviders);
            }

            excludedInterceptorProviders = new HashSet<Type>(execluded);
            return providers.ToArray();
        } 
       
        public IInterceptorProvider[] GetInterceptorProvidersForProperty(Type targetType, PropertyInfo property, PropertyMethod propertyMethod, out ISet<Type> excludedInterceptorProviders)
        {
            Check.NotNull(property, nameof(property));
            if (propertyMethod == PropertyMethod.Get && property.GetMethod == null)
            {
                throw new ArgumentException($"The property{ property.Name } of { property.DeclaringType.AssemblyQualifiedName} does not have a Get method.", nameof(propertyMethod));
            }
            if (propertyMethod == PropertyMethod.Set && property.SetMethod == null)
            {
                throw new ArgumentException($"The property { property.Name } of { property.DeclaringType.AssemblyQualifiedName} does not have a Set method.", nameof(propertyMethod));
            }

            var providers = new List<IInterceptorProvider>();
            var execluded = new List<Type>();

            for (int index = 0; index < _providerResolvers.Length; index++)
            {
                var resolver = _providerResolvers[index];
                providers.AddRange(resolver.GetInterceptorProvidersForProperty(targetType, property, propertyMethod, out var excludedProviders));
                execluded.AddRange(excludedProviders);
            }

            excludedInterceptorProviders = new HashSet<Type>(execluded);
            return providers.ToArray();
        }

        public bool? WillIntercept(Type type)
        {
            Check.NotNull(type, nameof(type));
            for (int index = _providerResolvers.Length - 1; index >= 0; index--)
            {
                var result = _providerResolvers[index].WillIntercept(type);
                if (result == null)
                {
                    continue;
                }
                return result.Value;
            }
            return null;
        }

        public bool? WillIntercept(Type targetType, MethodInfo method)
        {
            Check.NotNull(targetType, nameof(targetType));
            Check.NotNull(method, nameof(method));
            for (int index = _providerResolvers.Length - 1; index >= 0; index--)
            {
                var result = _providerResolvers[index].WillIntercept(targetType, method);
                if (result == null)
                {
                    continue;
                }
                return result.Value;
            }
            return null;
        }

        public bool? WillIntercept(Type targetType, PropertyInfo property)
        {
            Check.NotNull(targetType, nameof(targetType));
            Check.NotNull(property, nameof(property));
            for (int index = _providerResolvers.Length - 1; index >= 0; index--)
            {
                var result = _providerResolvers[index].WillIntercept(targetType, property);
                if (result == null)
                {
                    continue;
                }
                return result.Value;
            }
            return null;
        }
    }
}
