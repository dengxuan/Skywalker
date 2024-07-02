// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.ExceptionHandler;
using Skywalker.ExceptionHandler.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ExceptionHandleIServiceCollectionExtensions
{

    public static IServiceCollection AddExceptionHandler(this IServiceCollection services)
    {
        services.TryAddSingleton<IExceptionNotifier, NullExceptionNotifier>();
        return services;
    }
}
