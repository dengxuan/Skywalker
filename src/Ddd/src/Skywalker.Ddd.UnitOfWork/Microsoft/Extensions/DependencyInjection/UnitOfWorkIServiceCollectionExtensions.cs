using Skywalker.Ddd.UnitOfWork;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class UnitOfWorkIServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services, Action<UnitOfWorkDefaultOptions> options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.Configure(options);
            services.AddSingleton<UnitOfWorkInterceptor>();
            return services;
        }
    }
}
