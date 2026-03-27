using Skywalker.Permissions;
using System.Security.Claims;

namespace Skywalker.Permissions.Tests;

public class PermissionModelTests
{
    // PermissionGrantInfo
    [Fact]
    public void PermissionGrantInfo_Granted()
    {
        var info = new PermissionGrantInfo("read", true, "Role", "admin");
        Assert.Equal("read", info.Name);
        Assert.True(info.IsGranted);
        Assert.Equal("Role", info.ProviderName);
        Assert.Equal("admin", info.ProviderKey);
    }

    [Fact]
    public void PermissionGrantInfo_NotGranted()
    {
        var info = new PermissionGrantInfo("write", false);
        Assert.False(info.IsGranted);
        Assert.Null(info.ProviderName);
    }

    // MultiplePermissionGrantResult
    [Fact]
    public void MultiplePermissionGrantResult_DefaultConstructor()
    {
        var result = new MultiplePermissionGrantResult();
        Assert.Empty(result.Result);
    }

    [Fact]
    public void MultiplePermissionGrantResult_AllGranted()
    {
        var result = new MultiplePermissionGrantResult(new[] { "perm1", "perm2" }, PermissionGrantResult.Granted);
        Assert.True(result.AllGranted);
        Assert.False(result.AllProhibited);
    }

    [Fact]
    public void MultiplePermissionGrantResult_AllProhibited()
    {
        var result = new MultiplePermissionGrantResult(new[] { "perm1" }, PermissionGrantResult.Prohibited);
        Assert.True(result.AllProhibited);
        Assert.False(result.AllGranted);
    }

    [Fact]
    public void MultiplePermissionGrantResult_Mixed()
    {
        var result = new MultiplePermissionGrantResult(new[] { "perm1", "perm2" }, PermissionGrantResult.Undefined);
        result.Result["perm1"] = PermissionGrantResult.Granted;
        Assert.False(result.AllGranted);
        Assert.False(result.AllProhibited);
    }

    // PermissionValueCheckContext
    [Fact]
    public void PermissionValueCheckContext_StoresValues()
    {
        var perm = new PermissionDefinition("test", "Test");
        var principal = new ClaimsPrincipal();
        var ctx = new PermissionValueCheckContext(perm, principal);
        Assert.Same(perm, ctx.Permission);
        Assert.Same(principal, ctx.Principal);
    }

    [Fact]
    public void PermissionValueCheckContext_NullPrincipal()
    {
        var perm = new PermissionDefinition("test", "Test");
        var ctx = new PermissionValueCheckContext(perm, null);
        Assert.Null(ctx.Principal);
    }

    // PermissionOptions
    [Fact]
    public void PermissionOptions_Defaults()
    {
        var options = new PermissionOptions();
        Assert.NotNull(options.DefinitionProviders);
        Assert.NotNull(options.ValueProviders);
        Assert.Empty(options.DefinitionProviders);
        Assert.Empty(options.ValueProviders);
    }
}
