// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd;
using Skywalker.Ddd.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class SkywalkerDddIServiceCollectionExtensions
{
    internal static DddBuilder AddCoreServices(this DddBuilder builder)
    {
        builder.Services.TryAddScoped<ICachedServiceProvider, CachedServiceProvider>();
        return builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static DddBuilder AddSkywalkerCore(this IServiceCollection services)
    {
        var ddd = new DddBuilder(services);
        services.AddObjectAccessor(ddd);
        return ddd.AddCoreServices();
    }
}
