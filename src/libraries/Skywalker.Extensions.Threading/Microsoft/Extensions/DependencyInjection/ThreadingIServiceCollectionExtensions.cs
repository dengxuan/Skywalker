// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.Threading;

namespace Microsoft.Extensions.DependencyInjection;

public static class ThreadingIServiceCollectionExtensions
{

    public static IServiceCollection AddThreading(this IServiceCollection services)
    {
        services.TryAddTransient<SkywalkerAsyncTimer>();
        return services;
    }
}
