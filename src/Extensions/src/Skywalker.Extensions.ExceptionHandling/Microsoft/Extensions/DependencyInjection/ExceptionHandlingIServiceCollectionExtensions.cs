using Skywalker.ExceptionHandling;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ExceptionHandlingIServiceCollectionExtensions
    {
        public static IServiceCollection AddExceptionHandling(this IServiceCollection services)
        {
            services.AddSingleton<IExceptionNotifier, ExceptionNotifier>();
            return services;
        }
    }
}
