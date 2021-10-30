using Skywalker;
using Skywalker.Ddd.DependencyInjection;
using Skywalker.ExceptionHandling;

namespace Microsoft.Extensions.DependencyInjection;

public static class SkywalkerIServiceCollectionExtensions
{
    internal static SkywalkerDddBuilder AddSkywalkerCore(this IServiceCollection services)
    {
        services.AddSingleton(typeof(ObjectAccessor<>));
        services.AddSingleton(typeof(IObjectAccessor<>), typeof(ObjectAccessor<>));
        services.AddSingleton<IExceptionNotifier, ExceptionNotifier>();
        services.AddSingleton<IHybridServiceScopeFactory, DefaultServiceScopeFactory>();

        SkywalkerDddBuilder builder = new(services);
        return builder;

    }
    public static IServiceCollection AddSkywalker(this IServiceCollection services, Action<SkywalkerDddBuilder> buildAction)
    {
        services.AddGuidGenerator();
        services.AddTiming();
        //AddTransientServices(services);
        //AddSingletonServices(services);
        //AddScopedServices(services);
        SkywalkerDddBuilder builder = services.AddSkywalkerCore();
        buildAction?.Invoke(builder);
        return services;
    }

    //private static IServiceCollection AddTransientServices(IServiceCollection services)
    //{
    //    return services.Scan(scanner =>
    //    {
    //        scanner.FromApplicationDependencies()
    //               .AddClasses(filter =>
    //               {
    //                   filter.AssignableTo<ITransientDependency>();
    //               })
    //               .AsImplementedInterfaces()
    //               .AsSelf()
    //               .WithTransientLifetime();
    //    });
    //}

    //private static IServiceCollection AddScopedServices(IServiceCollection services)
    //{
    //    return services.Scan(scanner =>
    //    {
    //        scanner.FromApplicationDependencies()
    //               .AddClasses(filter =>
    //               {
    //                   filter.AssignableTo<IScopedDependency>();
    //               })
    //               .AsImplementedInterfaces()
    //               .AsSelf()
    //               .WithScopedLifetime();
    //    });
    //}

    //private static IServiceCollection AddSingletonServices(IServiceCollection services)
    //{
    //    return services.Scan(scanner =>
    //    {
    //        scanner.FromApplicationDependencies()
    //               .AddClasses(filter =>
    //               {
    //                   filter.AssignableTo<ISingletonDependency>();
    //               })
    //               .AsImplementedInterfaces()
    //               .AsSelf()
    //               .WithSingletonLifetime();
    //    });
    //}
}
