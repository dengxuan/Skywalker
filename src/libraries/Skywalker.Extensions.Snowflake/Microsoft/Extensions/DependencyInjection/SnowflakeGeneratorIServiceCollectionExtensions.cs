// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.Snowflake;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class SnowflakeGeneratorIServiceCollectionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSnowflakeGenerator(this IServiceCollection services)
    {
        return services.AddSnowflakeGenerator(options => { });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddSnowflakeGenerator(this IServiceCollection services, Action<SnowflakeGeneratorOptions> options)
    {
        services.Configure(options);
        services.TryAddSingleton<ISnowflakeGenerator, SnowflakeGenerator>();
        services.TryAddSingleton<IWorker, DefaultWorker>();
        return services;
    }
}
