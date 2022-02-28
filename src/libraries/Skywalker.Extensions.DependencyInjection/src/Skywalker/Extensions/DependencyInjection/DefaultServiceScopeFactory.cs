using Microsoft.Extensions.DependencyInjection;
using Skywalker.Extensions.DependencyInjection.Abstractions;

namespace Skywalker.Extensions.DependencyInjection;

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
