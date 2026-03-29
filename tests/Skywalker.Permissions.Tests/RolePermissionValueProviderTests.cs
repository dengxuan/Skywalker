// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Claims;
using NSubstitute;
using Skywalker.Permissions.Abstractions;
using Skywalker.Security.Claims;

namespace Skywalker.Permissions.Tests;

public class RolePermissionValueProviderTests
{
    private readonly IPermissionValidator _validator;
    private readonly RolePermissionValueProvider _provider;

    public RolePermissionValueProviderTests()
    {
        _validator = Substitute.For<IPermissionValidator>();
        _provider = new RolePermissionValueProvider(_validator);
    }

    private ClaimsPrincipal CreatePrincipalWithRoles(params string[] roles)
    {
        var claims = roles.Select(r => new Claim(SkywalkerClaimTypes.Role, r)).ToList();
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
    }

    [Fact]
    public void Name_ReturnsR()
    {
        Assert.Equal("R", _provider.Name);
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
    public async Task CheckAsync_NoRoleClaims_ReturnsUndefined()
    {
        var perm = new PermissionDefinition("perm1", "Perm 1");
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        var ctx = new PermissionValueCheckContext(perm, principal);

        var result = await _provider.CheckAsync(ctx);

        Assert.Equal(PermissionGrantResult.Undefined, result);
    }

    [Fact]
    public async Task CheckAsync_SingleRole_Granted()
    {
        var perm = new PermissionDefinition("perm1", "Perm 1");
        var principal = CreatePrincipalWithRoles("admin");
        var ctx = new PermissionValueCheckContext(perm, principal);

        _validator.IsGrantedAsync("perm1", "R", "admin").Returns(Task.FromResult(true));

        var result = await _provider.CheckAsync(ctx);

        Assert.Equal(PermissionGrantResult.Granted, result);
    }

    [Fact]
    public async Task CheckAsync_MultipleRoles_FirstMatchWins()
    {
        var perm = new PermissionDefinition("perm1", "Perm 1");
        var principal = CreatePrincipalWithRoles("user", "admin");
        var ctx = new PermissionValueCheckContext(perm, principal);

        _validator.IsGrantedAsync("perm1", "R", "user").Returns(Task.FromResult(false));
        _validator.IsGrantedAsync("perm1", "R", "admin").Returns(Task.FromResult(true));

        var result = await _provider.CheckAsync(ctx);

        Assert.Equal(PermissionGrantResult.Granted, result);
    }

    [Fact]
    public async Task CheckAsync_MultipleRoles_NoneGranted_ReturnsUndefined()
    {
        var perm = new PermissionDefinition("perm1", "Perm 1");
        var principal = CreatePrincipalWithRoles("user", "viewer");
        var ctx = new PermissionValueCheckContext(perm, principal);

        _validator.IsGrantedAsync("perm1", "R", Arg.Any<string>()).Returns(Task.FromResult(false));

        var result = await _provider.CheckAsync(ctx);

        Assert.Equal(PermissionGrantResult.Undefined, result);
    }

    [Fact]
    public async Task CheckAsync_DuplicateRoles_DeduplicatesCorrectly()
    {
        var perm = new PermissionDefinition("perm1", "Perm 1");
        var principal = CreatePrincipalWithRoles("admin", "admin", "admin");
        var ctx = new PermissionValueCheckContext(perm, principal);

        _validator.IsGrantedAsync("perm1", "R", "admin").Returns(Task.FromResult(true));

        var result = await _provider.CheckAsync(ctx);

        Assert.Equal(PermissionGrantResult.Granted, result);
        // Should only be called once due to Distinct()
        await _validator.Received(1).IsGrantedAsync("perm1", "R", "admin");
    }

    // === Multiple permissions ===

    [Fact]
    public async Task CheckAsync_Multiple_NoRoles_ReturnsAllUndefined()
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
    public async Task CheckAsync_Multiple_RoleGrantsSome()
    {
        var perms = new List<PermissionDefinition>
        {
            new("p1", "P1"),
            new("p2", "P2")
        };
        var principal = CreatePrincipalWithRoles("admin");
        var ctx = new PermissionValuesCheckContext(perms, principal);

        _validator.IsGrantedAsync(Arg.Any<string[]>(), "R", "admin")
            .Returns(callInfo =>
            {
                var names = callInfo.Arg<string[]>();
                var r = new MultiplePermissionGrantResult(names);
                if (r.Result.ContainsKey("p1")) r.Result["p1"] = PermissionGrantResult.Granted;
                return Task.FromResult(r);
            });

        var result = await _provider.CheckAsync(ctx);

        Assert.Equal(PermissionGrantResult.Granted, result.Result["p1"]);
        Assert.Equal(PermissionGrantResult.Undefined, result.Result["p2"]);
    }
}
