namespace Microsoft.Extensions.DependencyInjection;

public static partial class DomainIServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        return services.AddDomainServices((builder) => { });
    }

    public static IServiceCollection AddDomainServices(this IServiceCollection services, Action<DomainServiceBuilder> builderOptions)
    {
        var builder = new DomainServiceBuilder(services);
        builderOptions(builder);
        return builder.AddDomainServicesCore();
    }
}
