using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Extensions.DependencyInjection.Abstractions;

public interface IHybridServiceScopeFactory : IServiceScopeFactory, ISingletonDependency
{

}
