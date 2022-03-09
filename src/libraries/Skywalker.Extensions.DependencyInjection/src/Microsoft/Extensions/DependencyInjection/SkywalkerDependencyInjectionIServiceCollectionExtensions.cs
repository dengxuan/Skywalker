using Scrutor;
using Skywalker.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class SkywalkerDependencyInjectionIServiceCollectionExtensions
{

    public static IServiceCollection AddSkywalker(this IServiceCollection services)
    {
        services.Scan(scanner =>
        {
            scanner.FromApplicationDependencies()
                   .AddClasses(classes =>
                   {
                       classes.WithAttribute<TransientDependencyAttribute>();
                   })
                   .AsImplementedInterfaces()
                   .WithTransientLifetime();
            scanner.FromApplicationDependencies()
                   .AddClasses(classes =>
                   {
                       classes.WithAttribute<ScopedDependencyAttribute>();
                   })
                   .AsImplementedInterfaces()
                   .WithScopedLifetime();
            scanner.FromApplicationDependencies()
                   .AddClasses(classes =>
                   {
                       classes.WithAttribute<SingletonDependencyAttribute>();
                   })
                   .AsImplementedInterfaces()
                   .WithSingletonLifetime();
        });
        return services;
    }
}
