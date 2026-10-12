using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Skywalker.Permissions.Abstractions;
using Skywalker.Security.Claims;

namespace Skywalker.Permissions.Tests;

/// <summary>
/// 覆盖 <c>AddPermissions()</c> 开箱即用的装配（#310）。
/// </summary>
public class PermissionsServiceCollectionExtensionsTests
{
    private const string PermissionName = "Orders.Create";

    private sealed class TestPermissionDefinitionProvider : IPermissionDefinitionProvider
    {
        public void Define(PermissionDefinitionContext context)
        {
            context.AddPermission(PermissionName, PermissionName);
        }
    }

    /// <summary>未在 DI 注册、仅登记在 options 中的自定义值提供者。</summary>
    private sealed class AlwaysGrantPermissionValueProvider : IPermissionValueProvider
    {
        public string Name => "Always";

        public Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
            => Task.FromResult(PermissionGrantResult.Granted);

        public Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
            => Task.FromResult(new MultiplePermissionGrantResult(context.Permissions.Select(p => p.Name).ToArray(), PermissionGrantResult.Granted));
    }

    private static ClaimsPrincipal CreateUser(string userId)
        => new(new ClaimsIdentity(new[] { new Claim(SkywalkerClaimTypes.UserId, userId) }, "TestAuth"));

    [Fact]
    public void AddPermissions_RegistersBuiltInValueProvidersInPriorityOrder()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddPermissions();
        using var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<PermissionOptions>>().Value;
        Assert.Equal(
            new[] { typeof(UserPermissionValueProvider), typeof(RolePermissionValueProvider), typeof(ClientPermissionValueProvider) },
            options.ValueProviders.ToArray());

        var manager = provider.GetRequiredService<IPermissionValueProviderManager>();
        Assert.Equal(3, manager.ValueProviders.Count);
    }

    [Fact]
    public async Task IsGrantedAsync_GrantedForUser_WithoutManualProviderRegistration()
    {
        var validator = Substitute.For<IPermissionValidator>();
        validator.IsGrantedAsync(PermissionName, UserPermissionValueProvider.ProviderName, "user-1").Returns(true);

        var services = new ServiceCollection();
        services.AddSingleton(validator);
        services.AddLogging();
        services.AddPermissions();
        services.Configure<PermissionOptions>(o => o.DefinitionProviders.Add<TestPermissionDefinitionProvider>());
        using var provider = services.BuildServiceProvider();

        var checker = provider.GetRequiredService<IPermissionChecker>();

        Assert.True(await checker.IsGrantedAsync(CreateUser("user-1"), PermissionName));
        Assert.False(await checker.IsGrantedAsync(CreateUser("user-2"), PermissionName));
    }

    [Fact]
    public void ValueProviderManager_ResolvesProviderNotRegisteredInDi()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddPermissions();
        services.Configure<PermissionOptions>(o => o.ValueProviders.Add<AlwaysGrantPermissionValueProvider>());
        using var provider = services.BuildServiceProvider();

        var manager = provider.GetRequiredService<IPermissionValueProviderManager>();

        Assert.IsType<AlwaysGrantPermissionValueProvider>(manager.ValueProviders[^1]);
    }

    [Fact]
    public void AddPermissions_CalledTwice_DoesNotDuplicateValueProviders()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddPermissions();
        services.AddPermissions();
        using var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<PermissionOptions>>().Value;
        Assert.Equal(3, options.ValueProviders.Count);
    }
}
