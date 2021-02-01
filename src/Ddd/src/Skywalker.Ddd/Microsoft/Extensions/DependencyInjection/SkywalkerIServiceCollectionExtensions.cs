using Skywalker;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SkywalkerIServiceCollectionExtensions
    {
        public static IServiceCollection AddSkywalker(this IServiceCollection services, Action<SkywalkerBuilder> buildAction)
        {
            services.AddScoped<ILazyLoader>(sp =>
            {
                return new MsDependencyInjectionLazyLoader(sp);
            });
            services.AddGuidGenerator();
            services.AddTiming();

            AddTransientServices(services);
            AddSingletonServices(services);
            AddScopedServices(services);
            SkywalkerBuilder builder = new SkywalkerBuilder(services);
            buildAction?.Invoke(builder);
            return services;
        }

        private static IServiceCollection AddTransientServices(IServiceCollection services)
        {
            return services.Scan(scanner =>
            {
                scanner.FromApplicationDependencies()
                       .AddClasses(filter =>
                       {
                           filter.AssignableTo<ITransientDependency>();
                       })
                       .AsImplementedInterfaces()
                       .AsSelf()
                       .WithTransientLifetime();
            });
        }

        private static IServiceCollection AddScopedServices(IServiceCollection services)
        {
            return services.Scan(scanner =>
            {
                scanner.FromApplicationDependencies()
                       .AddClasses(filter =>
                       {
                           filter.AssignableTo<IScopedDependency>();
                       })
                       .AsImplementedInterfaces()
                       .AsSelf()
                       .WithScopedLifetime();
            });
        }

        private static IServiceCollection AddSingletonServices(IServiceCollection services)
        {
            return services.Scan(scanner =>
            {
                scanner.FromApplicationDependencies()
                       .AddClasses(filter =>
                       {
                           filter.AssignableTo<ISingletonDependency>();
                       })
                       .AsImplementedInterfaces()
                       .AsSelf()
                       .WithSingletonLifetime();
            });
        }
    }
}
