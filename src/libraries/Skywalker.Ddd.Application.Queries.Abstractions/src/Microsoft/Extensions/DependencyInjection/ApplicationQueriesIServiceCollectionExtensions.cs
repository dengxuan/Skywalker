// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Application.Queries;
using Skywalker.Ddd.Application.Queries.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationQueriesIServiceCollectionExtensions
{
    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        services.AddScoped<IQuerier, DefaultQuerier>();
        services.AddScoped(typeof(IQueryHandlerProvider<,>), typeof(DefaultQueryHandlerProvider<,>));
        return services;
    }
}
