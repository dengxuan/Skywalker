// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Claims;
using NSubstitute;
using Skywalker.Permissions.Abstractions;
using Skywalker.Security.Claims;

namespace Skywalker.Permissions.Tests;

public class ClientPermissionValueProviderTests
{
    private readonly IPermissionValidator _validator;
    private readonly ClientPermissionValueProvider _provider;

    public ClientPermissionValueProviderTests()
    {
        _validator = Substitute.For<IPermissionValidator>();
        _provider = new ClientPermissionValueProvider(_validator);
    }

    private ClaimsPrincipal CreatePrincipalWithClientId(string clientId)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(SkywalkerClaimTypes.ClientId, clientId) }, "TestAuth"));
    }

    [Fact]
    public void Name_ReturnsC()
    {
        Assert.Equal("C", _provider.Name);
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
    public async Task CheckAsync_NoClientIdClaim_ReturnsUndefined()
    {
        var perm = new PermissionDefinition("perm1", "Perm 1");
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        var ctx = new PermissionValueCheckContext(perm, principal);

        var result = await _provider.CheckAsync(ctx);

        Assert.Equal(PermissionGrantResult.Undefined, result);
    }

    [Fact]
    public async Task CheckAsync_ValidatorGranted_ReturnsGranted()
    {
        var perm = new PermissionDefinition("perm1", "Perm 1");
        var principal = CreatePrincipalWithClientId("client-app");
        var ctx = new PermissionValueCheckContext(perm, principal);

        _validator.IsGrantedAsync("perm1", "C", "client-app").Returns(Task.FromResult(true));

        var result = await _provider.CheckAsync(ctx);

        Assert.Equal(PermissionGrantResult.Granted, result);
    }

    [Fact]
    public async Task CheckAsync_ValidatorNotGranted_ReturnsUndefined()
    {
        var perm = new PermissionDefinition("perm1", "Perm 1");
        var principal = CreatePrincipalWithClientId("client-app");
        var ctx = new PermissionValueCheckContext(perm, principal);

        _validator.IsGrantedAsync("perm1", "C", "client-app").Returns(Task.FromResult(false));

        var result = await _provider.CheckAsync(ctx);

        Assert.Equal(PermissionGrantResult.Undefined, result);
    }

    // === Multiple permissions ===

    [Fact]
    public async Task CheckAsync_Multiple_NoClientId_ReturnsAllUndefined()
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
    public async Task CheckAsync_Multiple_DelegatesCorrectly()
    {
        var perms = new List<PermissionDefinition>
        {
            new("p1", "P1"),
            new("p2", "P2")
        };
        var principal = CreatePrincipalWithClientId("my-client");
        var ctx = new PermissionValuesCheckContext(perms, principal);

        var expected = new MultiplePermissionGrantResult(new[] { "p1", "p2" }, PermissionGrantResult.Granted);
        _validator.IsGrantedAsync(Arg.Any<string[]>(), "C", "my-client").Returns(Task.FromResult(expected));

        var result = await _provider.CheckAsync(ctx);

        Assert.True(result.AllGranted);
    }
}
