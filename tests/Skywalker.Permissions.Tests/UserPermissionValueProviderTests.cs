// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Claims;
using NSubstitute;
using Skywalker.Permissions.Abstractions;
using Skywalker.Security.Claims;

namespace Skywalker.Permissions.Tests;

public class UserPermissionValueProviderTests
{
    private readonly IPermissionValidator _validator;
    private readonly UserPermissionValueProvider _provider;

    public UserPermissionValueProviderTests()
    {
        _validator = Substitute.For<IPermissionValidator>();
        _provider = new UserPermissionValueProvider(_validator);
    }

    private ClaimsPrincipal CreatePrincipalWithUserId(string userId)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(SkywalkerClaimTypes.UserId, userId) }, "TestAuth"));
    }

    [Fact]
    public void Name_ReturnsU()
    {
        Assert.Equal("U", _provider.Name);
    }

    [Fact]
    public async Task CheckAsync_NoPrincipal_ReturnsUndefined()
    {
        var perm = new PermissionDefinition("perm1", "Perm 1");
        var ctx = new PermissionValueCheckContext(perm, null);

        var result = await _provider.CheckAsync(ctx);

        Assert.Equal(PermissionGrantResult.Undefined, result);
    }

    [Fact]
    public async Task CheckAsync_NoUserIdClaim_ReturnsUndefined()
    {
        var perm = new PermissionDefinition("perm1", "Perm 1");
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        var ctx = new PermissionValueCheckContext(perm, principal);

        var result = await _provider.CheckAsync(ctx);

        Assert.Equal(PermissionGrantResult.Undefined, result);
    }

    [Fact]
    public async Task CheckAsync_ValidatorReturnsTrue_ReturnsGranted()
    {
        var perm = new PermissionDefinition("perm1", "Perm 1");
        var principal = CreatePrincipalWithUserId("user1");
        var ctx = new PermissionValueCheckContext(perm, principal);

        _validator.IsGrantedAsync("perm1", "U", "user1").Returns(Task.FromResult(true));

        var result = await _provider.CheckAsync(ctx);

        Assert.Equal(PermissionGrantResult.Granted, result);
    }

    [Fact]
    public async Task CheckAsync_ValidatorReturnsFalse_ReturnsUndefined()
    {
        var perm = new PermissionDefinition("perm1", "Perm 1");
        var principal = CreatePrincipalWithUserId("user1");
        var ctx = new PermissionValueCheckContext(perm, principal);

        _validator.IsGrantedAsync("perm1", "U", "user1").Returns(Task.FromResult(false));

        var result = await _provider.CheckAsync(ctx);

        Assert.Equal(PermissionGrantResult.Undefined, result);
    }

    [Fact]
    public async Task CheckAsync_PassesCorrectProviderNameAndKey()
    {
        var perm = new PermissionDefinition("read:users", "Read Users");
        var principal = CreatePrincipalWithUserId("abc-123");
        var ctx = new PermissionValueCheckContext(perm, principal);

        _validator.IsGrantedAsync("read:users", "U", "abc-123").Returns(Task.FromResult(true));

        await _provider.CheckAsync(ctx);

        await _validator.Received(1).IsGrantedAsync("read:users", "U", "abc-123");
    }

    // === Multiple permissions ===

    [Fact]
    public async Task CheckAsync_Multiple_NoUserId_ReturnsAllUndefined()
    {
        var perms = new List<PermissionDefinition>
        {
            new("p1", "P1"),
            new("p2", "P2")
        };
        var ctx = new PermissionValuesCheckContext(perms, null);

        var result = await _provider.CheckAsync(ctx);

        Assert.Equal(PermissionGrantResult.Undefined, result.Result["p1"]);
        Assert.Equal(PermissionGrantResult.Undefined, result.Result["p2"]);
    }

    [Fact]
    public async Task CheckAsync_Multiple_DelegatesCorrectlyToValidator()
    {
        var perms = new List<PermissionDefinition>
        {
            new("p1", "P1"),
            new("p2", "P2")
        };
        var principal = CreatePrincipalWithUserId("user1");
        var ctx = new PermissionValuesCheckContext(perms, principal);

        var expected = new MultiplePermissionGrantResult(new[] { "p1", "p2" });
        expected.Result["p1"] = PermissionGrantResult.Granted;

        _validator.IsGrantedAsync(Arg.Any<string[]>(), "U", "user1").Returns(Task.FromResult(expected));

        var result = await _provider.CheckAsync(ctx);

        Assert.Equal(PermissionGrantResult.Granted, result.Result["p1"]);
        Assert.Equal(PermissionGrantResult.Undefined, result.Result["p2"]);
    }
}
