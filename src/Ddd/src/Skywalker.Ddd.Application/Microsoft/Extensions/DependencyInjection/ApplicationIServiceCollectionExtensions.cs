using Skywalker.Ddd.Application;
using Skywalker.Ddd.Application.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationIServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IApplication, Application>();
            services.AddScoped(typeof(IApplicationHandlerProvider<>), typeof(ApplicationHandlerProvider<>));
            services.AddScoped(typeof(IApplicationHandlerProvider<,>), typeof(DefaultApplicationHandlerProvider<,>));
            services.Scan(scanner =>
            {
                scanner.FromApplicationDependencies()
                       .AddClasses(filter =>
                       {
                           filter.AssignableTo(typeof(IApplicationHandler<>));
                       })
                       .AddClasses(filter =>
                       {
                           filter.AssignableTo(typeof(IApplicationHandler<,>));
                       })
                       .AsImplementedInterfaces()
                       .WithScopedLifetime();
            });
            return services;
        }
    }
}
