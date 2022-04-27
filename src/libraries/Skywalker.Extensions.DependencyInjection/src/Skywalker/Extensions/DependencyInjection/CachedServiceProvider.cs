// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.DependencyInjection.Abstractions;

namespace Skywalker.Extensions.DependencyInjection;

public class CachedServiceProvider : ICachedServiceProvider, IScopedDependency
{
    protected IServiceProvider ServiceProvider { get; }

    protected IDictionary<Type, object?> CachedServices { get; }

    public CachedServiceProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;

        CachedServices = new Dictionary<Type, object?>
        {
            { typeof(IServiceProvider), serviceProvider }
        };
    }

    public object GetService(Type serviceType)
    {
        return CachedServices.GetOrAdd(serviceType, () => ServiceProvider.GetService(serviceType))!;
    }
}
