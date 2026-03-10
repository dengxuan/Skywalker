// TODO: Decide whether Redis should be a cache layer or a storage layer
// using Microsoft.Extensions.Caching.Distributed;
// using Skywalker.Settings.Abstractions;

// namespace Skywalker.Settings.Redis;

// internal class RedisSettingStore(IDistributedCache distributedCache) : ISettingStore
// {
//     public async Task<List<SettingValue>> GetAllAsync(string[] names, string? providerName, string? providerKey)
//     {
//         var results = new List<SettingValue>();
//         foreach (var name in names)
//         {
//             var value = await GetOrNullAsync(name, providerName, providerKey);
//             if(value == null)
//             {
//                 continue;
//             }
//             results.Add(new SettingValue(name, value));
//         }
//         return results;
//     }

//     public async Task<string?> GetOrNullAsync(string name, string? providerName, string? providerKey)
//     {
//         var key = $"{providerName}:{providerKey}:{name}";
//         return await distributedCache.GetStringAsync(key);
//     }
// }
