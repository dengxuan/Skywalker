// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.GuidGenerator;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class GuidGeneratorIServiceCollectionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddGuidGenerator(this IServiceCollection services)
    {
        return services.AddGuidGenerator(options => { });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddGuidGenerator(this IServiceCollection services, Action<SequentialGuidGeneratorOptions> options)
    {
        services.Configure(options);
        services.TryAddSingleton<IGuidGenerator, SequentialGuidGenerator>();
        return services;
    }
}
