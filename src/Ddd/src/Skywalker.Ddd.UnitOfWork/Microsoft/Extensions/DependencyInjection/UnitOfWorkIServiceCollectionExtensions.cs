using Microsoft.Extensions.Hosting;
using Skywalker.Aspects;
using Skywalker.Aspects.Abstractinons;
using Skywalker.Ddd.Queries.Abstractions;
using Skywalker.Ddd.UnitOfWork;
using System;

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
