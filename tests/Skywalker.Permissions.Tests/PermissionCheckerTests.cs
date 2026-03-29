// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Claims;
using NSubstitute;
using Skywalker.Extensions.SimpleStateChecking;
using Skywalker.Permissions.Abstractions;
using Skywalker.Security.Claims;

namespace Skywalker.Permissions.Tests;

public class PermissionCheckerTests
{
    private readonly ICurrentPrincipalAccessor _principalAccessor;
    private readonly IPermissionDefinitionManager _definitionManager;
    private readonly IPermissionValueProviderManager _providerManager;
    private readonly ISimpleStateCheckerManager<PermissionDefinition> _stateCheckerManager;
    private readonly PermissionChecker _checker;

    public PermissionCheckerTests()
    {
        _principalAccessor = Substitute.For<ICurrentPrincipalAccessor>();
        _definitionManager = Substitute.For<IPermissionDefinitionManager>();
        _providerManager = Substitute.For<IPermissionValueProviderManager>();
        _stateCheckerManager = Substitute.For<ISimpleStateCheckerManager<PermissionDefinition>>();

        _checker = new PermissionChecker(
            _principalAccessor,
            _definitionManager,
            _providerManager,
            _stateCheckerManager);
    }

    private PermissionDefinition CreatePermission(string name, bool isEnabled = true, string[]? allowedProviders = null)
    {
        return new PermissionDefinition(name, name, isEnabled, allowedProviders: allowedProviders);
    }

    private ClaimsPrincipal CreatePrincipal(string? userId = null, string[]? roles = null, string? clientId = null)
    {
        var claims = new List<Claim>();
        if (userId != null) claims.Add(new Claim(SkywalkerClaimTypes.UserId, userId));
        if (roles != null)
        {
            foreach (var role in roles) claims.Add(new Claim(SkywalkerClaimTypes.Role, role));
        }
        if (clientId != null) claims.Add(new Claim(SkywalkerClaimTypes.ClientId, clientId));
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
    }

    private void SetupPermission(string name, bool isEnabled = true, string[]? allowedProviders = null)
    {
        var perm = CreatePermission(name, isEnabled, allowedProviders);
        _definitionManager.GetAsync(name).Returns(Task.FromResult(perm));
        _stateCheckerManager.IsEnabledAsync(perm).Returns(Task.FromResult(true));
    }

    // === Single permission check ===

    [Fact]
    public async Task IsGrantedAsync_DisabledPermission_ReturnsFalse()
    {
        var perm = CreatePermission("perm1", isEnabled: false);
        _definitionManager.GetAsync("perm1").Returns(Task.FromResult(perm));

        var result = await _checker.IsGrantedAsync(new ClaimsPrincipal(), "perm1");

        Assert.False(result);
    }

    [Fact]
    public async Task IsGrantedAsync_StateCheckerDisabled_ReturnsFalse()
    {
        var perm = CreatePermission("perm1");
        _definitionManager.GetAsync("perm1").Returns(Task.FromResult(perm));
        _stateCheckerManager.IsEnabledAsync(perm).Returns(Task.FromResult(false));

        var result = await _checker.IsGrantedAsync(new ClaimsPrincipal(), "perm1");

        Assert.False(result);
    }

    [Fact]
    public async Task IsGrantedAsync_NoProviders_ReturnsFalse()
    {
        SetupPermission("perm1");
        _providerManager.ValueProviders.Returns(new List<IPermissionValueProvider?>());

        var result = await _checker.IsGrantedAsync(new ClaimsPrincipal(), "perm1");

        Assert.False(result);
    }

    [Fact]
    public async Task IsGrantedAsync_ProviderGranted_ReturnsTrue()
    {
        SetupPermission("perm1");
        var provider = Substitute.For<IPermissionValueProvider>();
        provider.Name.Returns("TestProvider");
        provider.CheckAsync(Arg.Any<PermissionValueCheckContext>())
            .Returns(Task.FromResult(PermissionGrantResult.Granted));
        _providerManager.ValueProviders.Returns(new List<IPermissionValueProvider?> { provider });

        var result = await _checker.IsGrantedAsync(new ClaimsPrincipal(), "perm1");

        Assert.True(result);
    }

    [Fact]
    public async Task IsGrantedAsync_ProviderProhibited_ReturnsFalse()
    {
        SetupPermission("perm1");
        var provider = Substitute.For<IPermissionValueProvider>();
        provider.Name.Returns("TestProvider");
        provider.CheckAsync(Arg.Any<PermissionValueCheckContext>())
            .Returns(Task.FromResult(PermissionGrantResult.Prohibited));
        _providerManager.ValueProviders.Returns(new List<IPermissionValueProvider?> { provider });

        var result = await _checker.IsGrantedAsync(new ClaimsPrincipal(), "perm1");

        Assert.False(result);
    }

    [Fact]
    public async Task IsGrantedAsync_ProviderUndefined_ReturnsFalse()
    {
        SetupPermission("perm1");
        var provider = Substitute.For<IPermissionValueProvider>();
        provider.Name.Returns("TestProvider");
        provider.CheckAsync(Arg.Any<PermissionValueCheckContext>())
            .Returns(Task.FromResult(PermissionGrantResult.Undefined));
        _providerManager.ValueProviders.Returns(new List<IPermissionValueProvider?> { provider });

        var result = await _checker.IsGrantedAsync(new ClaimsPrincipal(), "perm1");

        Assert.False(result);
    }

    // === Provider priority: Prohibited overrides Granted ===

    [Fact]
    public async Task IsGrantedAsync_GrantedThenProhibited_ReturnsFalse()
    {
        SetupPermission("perm1");

        var grantProvider = Substitute.For<IPermissionValueProvider>();
        grantProvider.Name.Returns("GrantProvider");
        grantProvider.CheckAsync(Arg.Any<PermissionValueCheckContext>())
            .Returns(Task.FromResult(PermissionGrantResult.Granted));

        var prohibitProvider = Substitute.For<IPermissionValueProvider>();
        prohibitProvider.Name.Returns("ProhibitProvider");
        prohibitProvider.CheckAsync(Arg.Any<PermissionValueCheckContext>())
            .Returns(Task.FromResult(PermissionGrantResult.Prohibited));

        _providerManager.ValueProviders.Returns(
            new List<IPermissionValueProvider?> { grantProvider, prohibitProvider });

        var result = await _checker.IsGrantedAsync(new ClaimsPrincipal(), "perm1");

        Assert.False(result);
    }

    [Fact]
    public async Task IsGrantedAsync_UndefinedThenGranted_ReturnsTrue()
    {
        SetupPermission("perm1");

        var undefinedProvider = Substitute.For<IPermissionValueProvider>();
        undefinedProvider.Name.Returns("UndefinedProvider");
        undefinedProvider.CheckAsync(Arg.Any<PermissionValueCheckContext>())
            .Returns(Task.FromResult(PermissionGrantResult.Undefined));

        var grantProvider = Substitute.For<IPermissionValueProvider>();
        grantProvider.Name.Returns("GrantProvider");
        grantProvider.CheckAsync(Arg.Any<PermissionValueCheckContext>())
            .Returns(Task.FromResult(PermissionGrantResult.Granted));

        _providerManager.ValueProviders.Returns(
            new List<IPermissionValueProvider?> { undefinedProvider, grantProvider });

        var result = await _checker.IsGrantedAsync(new ClaimsPrincipal(), "perm1");

        Assert.True(result);
    }

    // === AllowedProviders filtering ===

    [Fact]
    public async Task IsGrantedAsync_AllowedProviders_SkipsNonAllowedProvider()
    {
        SetupPermission("perm1", allowedProviders: new[] { "AllowedProvider" });

        var skippedProvider = Substitute.For<IPermissionValueProvider>();
        skippedProvider.Name.Returns("NotAllowed");
        skippedProvider.CheckAsync(Arg.Any<PermissionValueCheckContext>())
            .Returns(Task.FromResult(PermissionGrantResult.Granted));

        _providerManager.ValueProviders.Returns(new List<IPermissionValueProvider?> { skippedProvider });

        var result = await _checker.IsGrantedAsync(new ClaimsPrincipal(), "perm1");

        Assert.False(result);
    }

    [Fact]
    public async Task IsGrantedAsync_AllowedProviders_UsesAllowedProvider()
    {
        SetupPermission("perm1", allowedProviders: new[] { "AllowedProvider" });

        var allowedProvider = Substitute.For<IPermissionValueProvider>();
        allowedProvider.Name.Returns("AllowedProvider");
        allowedProvider.CheckAsync(Arg.Any<PermissionValueCheckContext>())
            .Returns(Task.FromResult(PermissionGrantResult.Granted));

        _providerManager.ValueProviders.Returns(new List<IPermissionValueProvider?> { allowedProvider });

        var result = await _checker.IsGrantedAsync(new ClaimsPrincipal(), "perm1");

        Assert.True(result);
    }

    // === Uses current principal from accessor ===

    [Fact]
    public async Task IsGrantedAsync_WithoutPrincipal_UsesAccessorPrincipal()
    {
        var principal = CreatePrincipal(userId: "user1");
        _principalAccessor.Principal.Returns(principal);
        SetupPermission("perm1");

        ClaimsPrincipal? capturedPrincipal = null;
        var provider = Substitute.For<IPermissionValueProvider>();
        provider.Name.Returns("TestProvider");
        provider.CheckAsync(Arg.Do<PermissionValueCheckContext>(ctx => capturedPrincipal = ctx.Principal))
            .Returns(Task.FromResult(PermissionGrantResult.Granted));

        _providerManager.ValueProviders.Returns(new List<IPermissionValueProvider?> { provider });

        await _checker.IsGrantedAsync("perm1");

        Assert.Same(principal, capturedPrincipal);
    }

    // === Multiple permissions check ===

    [Fact]
    public async Task IsGrantedAsync_Multiple_EmptyNames_ReturnsEmptyResult()
    {
        var result = await _checker.IsGrantedAsync(new ClaimsPrincipal(), Array.Empty<string>());

        Assert.Empty(result.Result);
    }

    [Fact]
    public async Task IsGrantedAsync_Multiple_DisabledPermission_StaysUndefined()
    {
        var disabledPerm = CreatePermission("disabled", isEnabled: false);
        var enabledPerm = CreatePermission("enabled");
        _definitionManager.GetAsync("disabled").Returns(Task.FromResult(disabledPerm));
        _definitionManager.GetAsync("enabled").Returns(Task.FromResult(enabledPerm));
        _stateCheckerManager.IsEnabledAsync(enabledPerm).Returns(Task.FromResult(true));

        var provider = Substitute.For<IPermissionValueProvider>();
        provider.Name.Returns("TestProvider");
        provider.CheckAsync(Arg.Any<PermissionValuesCheckContext>())
            .Returns(callInfo =>
            {
                var ctx = callInfo.Arg<PermissionValuesCheckContext>();
                var multiResult = new MultiplePermissionGrantResult(
                    ctx.Permissions.Select(p => p.Name).ToArray(),
                    PermissionGrantResult.Granted);
                return Task.FromResult(multiResult);
            });
        _providerManager.ValueProviders.Returns(new List<IPermissionValueProvider?> { provider });

        var result = await _checker.IsGrantedAsync(new ClaimsPrincipal(), new[] { "disabled", "enabled" });

        Assert.Equal(PermissionGrantResult.Undefined, result.Result["disabled"]);
        Assert.Equal(PermissionGrantResult.Granted, result.Result["enabled"]);
    }

    [Fact]
    public async Task IsGrantedAsync_Multiple_AllGranted_BreaksEarly()
    {
        SetupPermission("p1");
        SetupPermission("p2");

        var provider1 = Substitute.For<IPermissionValueProvider>();
        provider1.Name.Returns("Provider1");
        provider1.CheckAsync(Arg.Any<PermissionValuesCheckContext>())
            .Returns(callInfo =>
            {
                var ctx = callInfo.Arg<PermissionValuesCheckContext>();
                return Task.FromResult(new MultiplePermissionGrantResult(
                    ctx.Permissions.Select(p => p.Name).ToArray(),
                    PermissionGrantResult.Granted));
            });

        var provider2 = Substitute.For<IPermissionValueProvider>();
        provider2.Name.Returns("Provider2");

        _providerManager.ValueProviders.Returns(
            new List<IPermissionValueProvider?> { provider1, provider2 });

        var result = await _checker.IsGrantedAsync(new ClaimsPrincipal(), new[] { "p1", "p2" });

        Assert.True(result.AllGranted);
        // Provider2 should NOT have been called
        await provider2.DidNotReceive().CheckAsync(Arg.Any<PermissionValuesCheckContext>());
    }

    [Fact]
    public async Task IsGrantedAsync_Multiple_PartialGrant_ContinuesToNextProvider()
    {
        SetupPermission("p1");
        SetupPermission("p2");

        var provider1 = Substitute.For<IPermissionValueProvider>();
        provider1.Name.Returns("Provider1");
        provider1.CheckAsync(Arg.Any<PermissionValuesCheckContext>())
            .Returns(callInfo =>
            {
                var ctx = callInfo.Arg<PermissionValuesCheckContext>();
                var r = new MultiplePermissionGrantResult(ctx.Permissions.Select(p => p.Name).ToArray());
                if (r.Result.ContainsKey("p1")) r.Result["p1"] = PermissionGrantResult.Granted;
                return Task.FromResult(r);
            });

        var provider2 = Substitute.For<IPermissionValueProvider>();
        provider2.Name.Returns("Provider2");
        provider2.CheckAsync(Arg.Any<PermissionValuesCheckContext>())
            .Returns(callInfo =>
            {
                var ctx = callInfo.Arg<PermissionValuesCheckContext>();
                var r = new MultiplePermissionGrantResult(ctx.Permissions.Select(p => p.Name).ToArray());
                if (r.Result.ContainsKey("p2")) r.Result["p2"] = PermissionGrantResult.Granted;
                return Task.FromResult(r);
            });

        _providerManager.ValueProviders.Returns(
            new List<IPermissionValueProvider?> { provider1, provider2 });

        var result = await _checker.IsGrantedAsync(new ClaimsPrincipal(), new[] { "p1", "p2" });

        Assert.Equal(PermissionGrantResult.Granted, result.Result["p1"]);
        Assert.Equal(PermissionGrantResult.Granted, result.Result["p2"]);
    }
}
