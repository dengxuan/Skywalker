using Skywalker.Application;
using Skywalker.Application.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationIServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IApplication, Application>();
            services.AddScoped(typeof(IExecuteQueryHandlerProvider<>), typeof(ExecuteQueryHandlerProvider<>));
            services.AddScoped(typeof(IExecuteQueryHandlerProvider<,>), typeof(ExecuteQueryHandlerProvider<,>));
            services.AddScoped(typeof(IExecuteNonQueryHandlerProvider<>), typeof(ExecuteNonQueryHandlerProvider<>));
            services.Scan(scanner =>
            {
                scanner.FromApplicationDependencies()
                       .AddClasses(filter =>
                       {
                           filter.AssignableTo(typeof(IExecuteQueryHandler<>));
                       })
                       .AddClasses(filter =>
                       {
                           filter.AssignableTo(typeof(IExecuteQueryHandler<,>));
                       })
                       .AddClasses(filter =>
                       {
                           filter.AssignableTo(typeof(IExecuteNonQueryHandler<>));
                       })
                       .AsImplementedInterfaces()
                       .WithScopedLifetime();
            });
            return services;
        }
    }
}
