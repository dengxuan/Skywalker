namespace Skywalker.Extensions.DependencyInjection.Abstractions;

public interface IServiceProviderAccessor
{
    IServiceProvider? ServiceProvider { get; }
}
