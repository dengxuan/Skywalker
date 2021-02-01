using Microsoft.Extensions.DependencyInjection;
using Skywalker.Queries;
using Skywalker.Queries.Abstractions;

namespace Skywalker.Queriers.DependencyInjection
{
    public static class QueriesIServiceCollectionExtensions
    {
        public static IServiceCollection AddQueries(this IServiceCollection services)
        {
            services.AddScoped<ISearcher, DefaultSearcher>();
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
    }
}
