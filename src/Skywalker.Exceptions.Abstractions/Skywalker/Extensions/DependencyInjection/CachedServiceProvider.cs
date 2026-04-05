// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.DependencyInjection;

internal class CachedServiceProvider : ICachedServiceProvider
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
        if (CachedServices.TryGetValue(serviceType, out var cached))
        {
            return cached!;
        }

        var resolved = ServiceProvider.GetService(serviceType);
        CachedServices[serviceType] = resolved;
        return resolved!;
    }
}
