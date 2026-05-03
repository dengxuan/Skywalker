namespace Skywalker.Extensions.DynamicProxies;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class SkywalkerGeneratedDynamicProxyAttribute : Attribute
{
    public SkywalkerGeneratedDynamicProxyAttribute(Type serviceType, Type implementationType, Type proxyType)
    {
        ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        ImplementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
        ProxyType = proxyType ?? throw new ArgumentNullException(nameof(proxyType));
    }

    public Type ServiceType { get; }

    public Type ImplementationType { get; }

    public Type ProxyType { get; }
}