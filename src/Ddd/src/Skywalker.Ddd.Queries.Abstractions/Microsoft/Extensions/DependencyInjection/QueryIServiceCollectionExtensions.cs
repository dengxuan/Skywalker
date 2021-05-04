using Skywalker.Ddd.Queries;
using Skywalker.Ddd.Queries.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static  class QueryIServiceCollectionExtensions
    {
        public static IServiceCollection AddQueries(this IServiceCollection services)
        {
            services.AddScoped<ISearcher, DefaultSearcher>();
            services.AddScoped(typeof(IQueryHandlerProvider<>), typeof(DefaultQueryHandlerProvider<>));
            services.AddScoped(typeof(IQueryHandlerProvider<,>), typeof(DefaultQueryHandlerProvider<,>));
            //services.Scan(scanner =>
            //{
            //    scanner.FromApplicationDependencies()
            //           .AddClasses(filter =>
            //           {
            //               filter.AssignableTo(typeof(IQueryHandler<,>));
            //           })
            //           .AsImplementedInterfaces()
            //           .WithScopedLifetime();
            //});
            return services;
        }
    }
}
