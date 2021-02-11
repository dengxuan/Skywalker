using Skywalker;
using Skywalker.Ddd.Queries;
using Skywalker.Ddd.Queries.Abstractions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SkywalkerIServiceCollectionExtensions
    {
        public static IServiceCollection AddSkywalker(this IServiceCollection services, Action<SkywalkerBuilder> buildAction)
        {
            services.AddScoped<ILazyLoader, MsDependencyInjectionLazyLoader>();
            services.AddGuidGenerator();
            services.AddTiming();
            //services.AddCommands();
            //services.AddQueries();
            AddTransientServices(services);
            AddSingletonServices(services);
            AddScopedServices(services);
            SkywalkerBuilder builder = new SkywalkerBuilder(services);
            buildAction?.Invoke(builder);
            return services;
        }

        public static IServiceCollection AddQueries(this IServiceCollection services)
        {
            services.AddScoped<ISearcher, DefaultSearcher>();
            services.AddScoped(typeof(IQueryHandlerProvider<>), typeof(DefaultQueryHandlerProvider<>));
            services.AddScoped(typeof(IQueryHandlerProvider<,>), typeof(DefaultQueryHandlerProvider<,>));
            services.Scan(scanner =>
            {
                scanner.FromApplicationDependencies()
                       .AddClasses(filter =>
                       {
                           filter.AssignableTo(typeof(IQueryHandler<,>));
                       })
                       .AsImplementedInterfaces()
                       .WithScopedLifetime();
            });
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
