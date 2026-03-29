// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions.Tests;

public class InMemoryPermissionValidatorTests
{
    private readonly IMemoryCache _cache;
    private readonly InMemoryPermissionValidator _validator;

    public InMemoryPermissionValidatorTests()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _validator = new InMemoryPermissionValidator(_cache, NullLogger<InMemoryPermissionValidator>.Instance);
    }

    private void SetupGrants(params (string permission, string provider, string key)[] grants)
    {
        var dict = new Dictionary<string, HashSet<(string, string)>>();
        foreach (var (permission, provider, key) in grants)
        {
            if (!dict.TryGetValue(permission, out var set))
            {
                set = new HashSet<(string, string)>();
                dict[permission] = set;
            }
            set.Add((provider, key));
        }
        _cache.Set(InMemoryPermissionValidator.GrantsCacheKey, dict);
    }

    // === Single permission ===

    [Fact]
    public async Task IsGrantedAsync_EmptyProviderName_ReturnsFalse()
    {
        SetupGrants(("perm1", "U", "user1"));
        Assert.False(await _validator.IsGrantedAsync("perm1", "", "user1"));
    }

    [Fact]
    public async Task IsGrantedAsync_EmptyProviderKey_ReturnsFalse()
    {
        SetupGrants(("perm1", "U", "user1"));
        Assert.False(await _validator.IsGrantedAsync("perm1", "U", ""));
    }

    [Fact]
    public async Task IsGrantedAsync_NullProviderName_ReturnsFalse()
    {
        Assert.False(await _validator.IsGrantedAsync("perm1", null!, "user1"));
    }

    [Fact]
    public async Task IsGrantedAsync_NoCacheData_ReturnsFalse()
    {
        Assert.False(await _validator.IsGrantedAsync("perm1", "U", "user1"));
    }

    [Fact]
    public async Task IsGrantedAsync_PermissionNotInCache_ReturnsFalse()
    {
        SetupGrants(("other", "U", "user1"));
        Assert.False(await _validator.IsGrantedAsync("perm1", "U", "user1"));
    }

    [Fact]
    public async Task IsGrantedAsync_MatchingGrant_ReturnsTrue()
    {
        SetupGrants(("perm1", "U", "user1"));
        Assert.True(await _validator.IsGrantedAsync("perm1", "U", "user1"));
    }

    [Fact]
    public async Task IsGrantedAsync_WrongProvider_ReturnsFalse()
    {
        SetupGrants(("perm1", "U", "user1"));
        Assert.False(await _validator.IsGrantedAsync("perm1", "R", "user1"));
    }

    [Fact]
    public async Task IsGrantedAsync_WrongKey_ReturnsFalse()
    {
        SetupGrants(("perm1", "U", "user1"));
        Assert.False(await _validator.IsGrantedAsync("perm1", "U", "user2"));
    }

    [Fact]
    public async Task IsGrantedAsync_MultipleGrants_FindsCorrectOne()
    {
        SetupGrants(
            ("perm1", "U", "user1"),
            ("perm1", "R", "admin"),
            ("perm2", "U", "user1"));

        Assert.True(await _validator.IsGrantedAsync("perm1", "U", "user1"));
        Assert.True(await _validator.IsGrantedAsync("perm1", "R", "admin"));
        Assert.True(await _validator.IsGrantedAsync("perm2", "U", "user1"));
        Assert.False(await _validator.IsGrantedAsync("perm2", "R", "admin"));
    }

    // === Multiple permissions ===

    [Fact]
    public async Task IsGrantedAsync_Multiple_NoCacheData_AllUndefined()
    {
        var result = await _validator.IsGrantedAsync(new[] { "p1", "p2" }, "U", "user1");

        Assert.Equal(PermissionGrantResult.Undefined, result.Result["p1"]);
        Assert.Equal(PermissionGrantResult.Undefined, result.Result["p2"]);
    }

    [Fact]
    public async Task IsGrantedAsync_Multiple_PartialMatch_MixedResults()
    {
        SetupGrants(("p1", "U", "user1"));

        var result = await _validator.IsGrantedAsync(new[] { "p1", "p2" }, "U", "user1");

        Assert.Equal(PermissionGrantResult.Granted, result.Result["p1"]);
        Assert.Equal(PermissionGrantResult.Undefined, result.Result["p2"]);
    }

    [Fact]
    public async Task IsGrantedAsync_Multiple_AllMatch_AllGranted()
    {
        SetupGrants(("p1", "U", "user1"), ("p2", "U", "user1"));

        var result = await _validator.IsGrantedAsync(new[] { "p1", "p2" }, "U", "user1");

        Assert.True(result.AllGranted);
    }

    // === Cache update behavior ===

    [Fact]
    public async Task IsGrantedAsync_CacheUpdated_ReflectsNewData()
    {
        Assert.False(await _validator.IsGrantedAsync("perm1", "U", "user1"));

        SetupGrants(("perm1", "U", "user1"));

        Assert.True(await _validator.IsGrantedAsync("perm1", "U", "user1"));
    }

    [Fact]
    public async Task IsGrantedAsync_CacheCleared_ReturnsFalse()
    {
        SetupGrants(("perm1", "U", "user1"));
        Assert.True(await _validator.IsGrantedAsync("perm1", "U", "user1"));

        _cache.Remove(InMemoryPermissionValidator.GrantsCacheKey);
        Assert.False(await _validator.IsGrantedAsync("perm1", "U", "user1"));
    }
}
