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
                       classes.AssignableTo<ITransientDependency>();
                   })
                   .AsSelfWithInterfaces()
                   .WithTransientLifetime();
            scanner.FromApplicationDependencies()
                   .AddClasses(classes =>
                   {
                       classes.AssignableTo<IScopedDependency>();
                   })
                   .AsSelfWithInterfaces()
                   .WithScopedLifetime();
            scanner.FromApplicationDependencies()
                   .AddClasses(classes =>
                   {
                       classes.AssignableTo<ISingletonDependency>();
                   })
                   .AsSelfWithInterfaces()
                   .WithSingletonLifetime();
        });
        return services;
    }
}
