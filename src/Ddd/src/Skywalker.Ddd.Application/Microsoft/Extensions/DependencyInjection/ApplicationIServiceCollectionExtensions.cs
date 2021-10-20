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
            return services;
        }
    }
}
