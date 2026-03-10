// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

// TODO: Decide whether Redis should be a cache layer or a storage layer
// using Microsoft.Extensions.Caching.StackExchangeRedis;
// using Skywalker.Settings;
// using Skywalker.Settings.Abstractions;
// using Skywalker.Settings.Redis;

// namespace Microsoft.Extensions.DependencyInjection;

// /// <summary>
// /// Extension methods for adding Redis settings store.
// /// </summary>
// public static class SkywalkerSettingOptionsExtensions
// {
//     /// <summary>
//     /// Adds Redis as the settings store.
//     /// </summary>
//     /// <param name="builder">The setting builder.</param>
//     /// <param name="options">Redis cache options.</param>
//     /// <returns>The setting builder.</returns>
//     public static ISettingBuilder AddRedis(this ISettingBuilder builder, Action<RedisCacheOptions> options)
//     {
//         builder.Services.AddSingleton<ISettingStore, RedisSettingStore>();
//         builder.Services.AddStackExchangeRedisCache(options);
//         return builder;
//     }
// }
