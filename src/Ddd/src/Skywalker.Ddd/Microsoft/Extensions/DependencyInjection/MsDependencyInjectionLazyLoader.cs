using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    internal class MsDependencyInjectionLazyLoader : ILazyLoader
    {
        private readonly IServiceProvider _serviceProvider;

        internal MsDependencyInjectionLazyLoader(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IServiceScope CreateScope()
        {
            return _serviceProvider.CreateScope();
        }

        public object GetRequiredService(Type serviceType)
        {
            return _serviceProvider.GetRequiredService(serviceType);
        }

        public T GetRequiredService<T>() where T : notnull
        {
            return new Lazy<T>(_serviceProvider.GetRequiredService<T>()).Value;
        }

        public T? GetService<T>()
        {
            return _serviceProvider.GetService<T>();
        }

        public IEnumerable<T> GetServices<T>()
        {
            return _serviceProvider.GetServices<T>();
        }

        public IEnumerable<object?> GetServices(Type serviceType)
        {
            return _serviceProvider.GetServices(serviceType);
        }
    }
}
