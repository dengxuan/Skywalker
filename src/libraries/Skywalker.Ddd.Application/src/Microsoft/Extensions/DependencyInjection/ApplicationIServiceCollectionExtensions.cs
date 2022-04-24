// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Application;
using Skywalker.Ddd.Application.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationIServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IApplication, DefaultApplication>();
        services.AddSingleton(typeof(IApplicationHandlerProvider), typeof(DefaultApplicationHandlerProvider));
        return services;
    }
}
