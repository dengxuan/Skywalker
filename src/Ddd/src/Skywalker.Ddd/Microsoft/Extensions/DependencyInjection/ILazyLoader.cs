using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface ILazyLoader
    {
        /// <summary>
        /// Creates a new Microsoft.Extensions.DependencyInjection.IServiceScope that can
        /// be used to resolve scoped services.
        /// </summary>
        /// <returns>A Microsoft.Extensions.DependencyInjection.IServiceScope that can be used to resolve scoped services.</returns>
        IServiceScope CreateScope();

        /// <summary>
        /// Get service of type serviceType from the System.IServiceProvider.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type serviceType.</returns>
        /// <exception cref="InvalidOperationException">There is no service of type serviceType.</exception>
        object GetRequiredService(Type serviceType);

        /// <summary>
        /// Get service of type T from the System.IServiceProvider.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <returns>A service object of type T.</returns>
        /// <exception cref="InvalidOperationException">There is no service of type T.</exception>
        T GetRequiredService<T>() where T : notnull;

        /// <summary>
        /// Get service of type T from the System.IServiceProvider.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <returns>A service object of type T or null if there is no such service.</returns>
        T? GetService<T>();

        /// <summary>
        /// Get an enumeration of services of type T from the System.IServiceProvider.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <returns>An enumeration of services of type T.</returns>
        IEnumerable<T> GetServices<T>();

        /// <summary>
        /// Get an enumeration of services of type serviceType from the System.IServiceProvider.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>An enumeration of services of type serviceType.</returns>
        IEnumerable<object?> GetServices(Type serviceType);
    }
}
