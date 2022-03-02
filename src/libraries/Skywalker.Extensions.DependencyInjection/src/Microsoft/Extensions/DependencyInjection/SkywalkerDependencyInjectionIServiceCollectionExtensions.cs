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
                   .UsingRegistrationStrategy(RegistrationStrategy.Throw)
                   .AsImplementedInterfaces()
                   .WithTransientLifetime();
            scanner.FromApplicationDependencies()
                   .AddClasses(classes =>
                   {
                       classes.WithAttribute<ScopedDependencyAttribute>();
                   })
                   .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                   .AsImplementedInterfaces()
                   .WithScopedLifetime();
            scanner.FromApplicationDependencies()
                   .AddClasses(classes =>
                   {
                       classes.WithAttribute<SingletonDependencyAttribute>();
                   })
                   .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                   .AsImplementedInterfaces()
                   .WithSingletonLifetime();
        });
        return services;
    }
}
