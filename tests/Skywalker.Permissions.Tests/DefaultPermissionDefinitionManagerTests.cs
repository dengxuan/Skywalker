// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions.Tests;

public class DefaultPermissionDefinitionManagerTests
{
    private DefaultPermissionDefinitionManager CreateManager(params Type[] providerTypes)
    {
        var options = new PermissionOptions();
        foreach (var type in providerTypes)
        {
            options.DefinitionProviders.Add(type);
        }

        var services = new ServiceCollection();
        var sp = services.BuildServiceProvider();

        return new DefaultPermissionDefinitionManager(
            sp,
            Options.Create(options));
    }

    [Fact]
    public async Task GetAsync_NoProviders_ThrowsKeyNotFound()
    {
        var manager = CreateManager();

        await Assert.ThrowsAsync<KeyNotFoundException>(() => manager.GetAsync("nonexistent"));
    }

    [Fact]
    public async Task GetAsync_DefinedPermission_ReturnsIt()
    {
        var manager = CreateManager(typeof(TestPermissionProvider));

        var perm = await manager.GetAsync("test.read");

        Assert.NotNull(perm);
        Assert.Equal("test.read", perm.Name);
    }

    [Fact]
    public async Task GetAsync_UndefinedPermission_ThrowsKeyNotFound()
    {
        var manager = CreateManager(typeof(TestPermissionProvider));

        await Assert.ThrowsAsync<KeyNotFoundException>(() => manager.GetAsync("nonexistent"));
    }

    [Fact]
    public async Task GetOrNullAsync_Defined_ReturnsPermission()
    {
        var manager = CreateManager(typeof(TestPermissionProvider));

        var perm = await manager.GetOrNullAsync("test.read");

        Assert.NotNull(perm);
    }

    [Fact]
    public async Task GetOrNullAsync_Undefined_ReturnsNull()
    {
        var manager = CreateManager(typeof(TestPermissionProvider));

        var perm = await manager.GetOrNullAsync("nonexistent");

        Assert.Null(perm);
    }

    [Fact]
    public async Task GetPermissionsAsync_ReturnsAll_IncludingChildren()
    {
        var manager = CreateManager(typeof(TestPermissionProvider));

        var all = await manager.GetPermissionsAsync();

        // TestPermissionProvider defines: test.read, test.write, test.admin, test.admin.full
        Assert.Contains(all, p => p.Name == "test.read");
        Assert.Contains(all, p => p.Name == "test.write");
        Assert.Contains(all, p => p.Name == "test.admin");
        Assert.Contains(all, p => p.Name == "test.admin.full");
        Assert.Equal(4, all.Count);
    }

    [Fact]
    public async Task GetPermissionsAsync_ByNames_FiltersCorrectly()
    {
        var manager = CreateManager(typeof(TestPermissionProvider));

        var filtered = await manager.GetPermissionsAsync("test.read", "test.write", "nonexistent");

        Assert.Equal(2, filtered.Count);
        Assert.Contains(filtered, p => p.Name == "test.read");
        Assert.Contains(filtered, p => p.Name == "test.write");
    }

    [Fact]
    public async Task GetPermissionsAsync_MultipleProviders_MergesAll()
    {
        var manager = CreateManager(typeof(TestPermissionProvider), typeof(SecondTestPermissionProvider));

        var all = await manager.GetPermissionsAsync();

        Assert.Contains(all, p => p.Name == "test.read");
        Assert.Contains(all, p => p.Name == "extra.perm");
    }

    [Fact]
    public async Task GetAsync_ChildPermission_AccessibleDirectly()
    {
        var manager = CreateManager(typeof(TestPermissionProvider));

        var child = await manager.GetAsync("test.admin.full");

        Assert.NotNull(child);
        Assert.Equal("test.admin.full", child.Name);
    }

    [Fact]
    public async Task CreatePermissionsAsync_AddsNewPermission()
    {
        var manager = CreateManager(typeof(TestPermissionProvider));

        var newPerm = new PermissionDefinition("dynamic.perm", "Dynamic Permission");
        await manager.CreatePermissionsAsync(new List<PermissionDefinition> { newPerm });

        var result = await manager.GetAsync("dynamic.perm");
        Assert.NotNull(result);
    }

    // === Test providers ===

    public class TestPermissionProvider : IPermissionDefinitionProvider
    {
        public void Define(PermissionDefinitionContext context)
        {
            context.AddPermission("test.read", "Test Read");
            context.AddPermission("test.write", "Test Write");
            var admin = context.AddPermission("test.admin", "Test Admin");
            admin.AddChild("test.admin.full", "Full Admin");
        }
    }

    public class SecondTestPermissionProvider : IPermissionDefinitionProvider
    {
        public void Define(PermissionDefinitionContext context)
        {
            context.AddPermission("extra.perm", "Extra Permission");
        }
    }
}
