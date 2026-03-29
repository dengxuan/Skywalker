// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Claims;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions.Tests;

public class AlwaysAllowPermissionCheckerTests
{
    private readonly AlwaysAllowPermissionChecker _checker = new();

    [Fact]
    public async Task IsGrantedAsync_SinglePermission_AlwaysTrue()
    {
        Assert.True(await _checker.IsGrantedAsync("any_permission"));
    }

    [Fact]
    public async Task IsGrantedAsync_WithPrincipal_AlwaysTrue()
    {
        Assert.True(await _checker.IsGrantedAsync(new ClaimsPrincipal(), "any_permission"));
    }

    [Fact]
    public async Task IsGrantedAsync_WithNullPrincipal_AlwaysTrue()
    {
        Assert.True(await _checker.IsGrantedAsync(null, "any_permission"));
    }

    [Fact]
    public async Task IsGrantedAsync_Multiple_AllGranted()
    {
        var result = await _checker.IsGrantedAsync(new[] { "p1", "p2", "p3" });

        Assert.True(result.AllGranted);
        Assert.Equal(PermissionGrantResult.Granted, result.Result["p1"]);
        Assert.Equal(PermissionGrantResult.Granted, result.Result["p2"]);
        Assert.Equal(PermissionGrantResult.Granted, result.Result["p3"]);
    }

    [Fact]
    public async Task IsGrantedAsync_Multiple_WithPrincipal_AllGranted()
    {
        var result = await _checker.IsGrantedAsync(new ClaimsPrincipal(), new[] { "read", "write" });

        Assert.True(result.AllGranted);
    }

    [Fact]
    public async Task IsGrantedAsync_Multiple_Empty_ReturnsEmptyResult()
    {
        var result = await _checker.IsGrantedAsync(Array.Empty<string>());

        Assert.Empty(result.Result);
    }
}
