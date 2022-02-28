namespace Skywalker.Extensions.DependencyInjection.Abstractions;

public interface IServiceProviderAccessor: ISingletonDependency
{
    IServiceProvider? ServiceProvider { get; }
}
