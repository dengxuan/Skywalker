using Skywalker.Ddd.UnitOfWork;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class UnitOfWorkIServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddSingleton<UnitOfWorkInterceptor>();
            return services;
        }
    }
}
