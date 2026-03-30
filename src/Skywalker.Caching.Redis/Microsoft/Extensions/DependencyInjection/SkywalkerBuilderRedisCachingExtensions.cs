// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker;
using Skywalker.Caching.Redis;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Skywalker 构建器的 Redis 缓存扩展方法。
/// </summary>
public static class SkywalkerBuilderRedisCachingExtensions
{
    /// <summary>
    /// 添加 Redis 缓存支持，替换默认的内存缓存。
    /// </summary>
    /// <param name="builder">Skywalker 构建器。</param>
    /// <returns>Skywalker 构建器，支持链式调用。</returns>
    /// <remarks>
    /// 配置从 appsettings.json 的 Skywalker:Caching:Redis 节读取。
    /// <code>
    /// services.AddSkywalker()
    ///     .AddRedisCaching();
    /// </code>
    /// </remarks>
    public static ISkywalkerBuilder AddRedisCaching(this ISkywalkerBuilder builder)
    {
        builder.Services.AddRedisCaching();
        return builder;
    }

    /// <summary>
    /// 添加 Redis 缓存支持，替换默认的内存缓存。
    /// </summary>
    /// <param name="builder">Skywalker 构建器。</param>
    /// <param name="configure">Redis 配置选项。</param>
    /// <returns>Skywalker 构建器，支持链式调用。</returns>
    /// <remarks>
    /// <code>
    /// services.AddSkywalker()
    ///     .AddRedisCaching(options =>
    ///     {
    ///         options.ConnectionString = "localhost:6379";
    ///     });
    /// </code>
    /// </remarks>
    public static ISkywalkerBuilder AddRedisCaching(this ISkywalkerBuilder builder, Action<RedisOptions> configure)
    {
        builder.Services.AddRedisCaching(configure);
        return builder;
    }
}
