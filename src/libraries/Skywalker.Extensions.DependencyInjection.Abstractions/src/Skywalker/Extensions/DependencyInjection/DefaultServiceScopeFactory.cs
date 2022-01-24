using Skywalker.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

[SingletonDependency]
public class DefaultServiceScopeFactory : IHybridServiceScopeFactory
{
    protected IServiceScopeFactory Factory { get; }

    public DefaultServiceScopeFactory(IServiceScopeFactory factory)
    {
        Factory = factory;
    }

    public IServiceScope CreateScope()
    {
        var serviceScope = Factory.CreateScope();
        return serviceScope;
    }
}
