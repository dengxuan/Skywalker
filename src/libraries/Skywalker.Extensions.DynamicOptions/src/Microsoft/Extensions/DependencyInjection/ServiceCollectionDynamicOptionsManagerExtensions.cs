// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Skywalker.Extensions.DynamicOptions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class ServiceCollectionDynamicOptionsManagerExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <typeparam name="TManager"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSkywalkerDynamicOptions<TOptions, TManager>(this IServiceCollection services)
        where TOptions : class
        where TManager : SkywalkerDynamicOptionsManager<TOptions>
    {
        services.Replace(ServiceDescriptor.Scoped(typeof(IOptions<TOptions>), typeof(TManager)));
        services.Replace(ServiceDescriptor.Scoped(typeof(IOptionsSnapshot<TOptions>), typeof(TManager)));

        return services;
    }
}
