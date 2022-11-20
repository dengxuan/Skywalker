// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.Caching.StackExchangeRedis;
using Skywalker.Settings;
using Skywalker.Settings.Abstractions;
using Skywalker.Settings.Redis;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class SkywalkerSettingOptionsExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static ISettingBuilder AddRedisStore(this ISettingBuilder builder, Action<RedisCacheOptions> options)
    {
        builder.Services.AddSingleton<ISettingStore, RedisSettingStore>();
        builder.Services.AddStackExchangeRedisCache(options);
        return builder;
    }
}
