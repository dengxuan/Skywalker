namespace Microsoft.Extensions.DependencyInjection;

public interface IServiceProviderAccessor
{
    IServiceProvider? ServiceProvider { get; }
}
