using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Skywalker;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SkywalkerIServiceCollectionExtensions
    {
        public static T GetSingletonInstanceOrNull<T>(this IServiceCollection services)
        {
            return (T)services
                .FirstOrDefault(d => d.ServiceType == typeof(T))
                ?.ImplementationInstance;
        }

        public static T GetSingletonInstance<T>(this IServiceCollection services)
        {
            var service = services.GetSingletonInstanceOrNull<T>();
            if (service == null)
            {
                throw new Exception("Can not find service: " + typeof(T).AssemblyQualifiedName);
            }

            return service;
        }

        public static IServiceCollection ReplaceConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Replace(ServiceDescriptor.Singleton(configuration));
        }

        public static IConfiguration? GetConfiguration(this IServiceCollection services)
        {
            var hostBuilderContext = services.GetSingletonInstanceOrNull<HostBuilderContext>();
            if (hostBuilderContext?.Configuration != null)
            {
                return hostBuilderContext.Configuration as IConfigurationRoot;
            }

            return services.GetSingletonInstance<IConfiguration>();
        }

        public static IServiceCollection AddSkywalker(this IServiceCollection services, Action<SkywalkerBuilder> buildAction)
        {
            services.AddSingleton<ILazyLoader>(sp =>
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
