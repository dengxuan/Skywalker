// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions;

/// <summary>
/// 基于内存缓存的权限验证器
/// Server 和 Client 都使用此实现，区别在于谁负责更新缓存
/// </summary>
public class InMemoryPermissionValidator : IPermissionValidator
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<InMemoryPermissionValidator> _logger;

    /// <summary>
    /// 缓存 Key
    /// </summary>
    public const string GrantsCacheKey = "permissions:grants";

    public InMemoryPermissionValidator(IMemoryCache cache, ILogger<InMemoryPermissionValidator> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task<bool> IsGrantedAsync(string name, string providerName, string providerKey)
    {
        if (string.IsNullOrEmpty(providerName) || string.IsNullOrEmpty(providerKey))
        {
            _logger.LogDebug("[InMemoryPermissionValidator] providerName 或 providerKey 为空");
            return Task.FromResult(false);
        }

        var grants = _cache.Get<Dictionary<string, HashSet<(string, string)>>>(GrantsCacheKey);
        _logger.LogDebug("[InMemoryPermissionValidator] 缓存中权限数量 {Count}", grants?.Count ?? 0);

        if (grants != null && grants.TryGetValue(name, out var set))
        {
            var result = set.Contains((providerName, providerKey));
            _logger.LogDebug("[InMemoryPermissionValidator] 检查 ({Permission}, {Provider}, {Key}) = {Result}",
                name, providerName, providerKey, result);
            return Task.FromResult(result);
        }

        _logger.LogDebug("[InMemoryPermissionValidator] 权限 {Permission} 不在缓存中", name);
        return Task.FromResult(false);
    }

    public Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names, string providerName, string providerKey)
    {
        var result = new MultiplePermissionGrantResult(names);
        var grants = _cache.Get<Dictionary<string, HashSet<(string, string)>>>(GrantsCacheKey);

        foreach (var name in names)
        {
            var isGranted = false;
            if (grants != null && grants.TryGetValue(name, out var set))
            {
                isGranted = set.Contains((providerName, providerKey));
            }
            result.Result[name] = isGranted ? PermissionGrantResult.Granted : PermissionGrantResult.Undefined;
        }

        return Task.FromResult(result);
    }
}
