namespace Microsoft.Extensions.DependencyInjection;

public static class DomainIServiceCollectionRepositoryExtensions
{
    public static DomainServiceBuilder AddDomainServices(this IServiceCollection services, Action<DomainServiceBuilder> builderOptions)
    {
        var builder = new DomainServiceBuilder(services);
        builderOptions?.Invoke(builder);
        return builder.AddDomainServicesCore();
    }
}
