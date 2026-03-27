using System.Security.Claims;
using NSubstitute;
using Skywalker.Security.Claims;
using Skywalker.Security.Clients;
using Xunit;

namespace Skywalker.Security.Tests;

public class SecurityModelTests
{
    // AbpDynamicClaim
    [Fact]
    public void AbpDynamicClaim_StoresTypeAndValue()
    {
        var claim = new AbpDynamicClaim("role", "admin");
        Assert.Equal("role", claim.Type);
        Assert.Equal("admin", claim.Value);
    }

    [Fact]
    public void AbpDynamicClaim_NullValue()
    {
        var claim = new AbpDynamicClaim("role", null);
        Assert.Null(claim.Value);
    }

    // AbpDynamicClaimCacheItem
    [Fact]
    public void AbpDynamicClaimCacheItem_DefaultConstructor()
    {
        var item = new AbpDynamicClaimCacheItem();
        Assert.NotNull(item.Claims);
        Assert.Empty(item.Claims);
    }

    [Fact]
    public void AbpDynamicClaimCacheItem_WithClaims()
    {
        var claims = new List<AbpDynamicClaim>
        {
            new("role", "admin"),
            new("email", "test@test.com")
        };
        var item = new AbpDynamicClaimCacheItem(claims);
        Assert.Equal(2, item.Claims.Count);
    }

    [Fact]
    public void AbpDynamicClaimCacheItem_CalculateCacheKey()
    {
        var key = AbpDynamicClaimCacheItem.CalculateCacheKey("user123");
        Assert.Equal("user123", key);
    }

    // SkywalkerClaimsPrincipalContributorContext
    [Fact]
    public void ClaimsPrincipalContributorContext_StoresValues()
    {
        var principal = new ClaimsPrincipal();
        var sp = Substitute.For<IServiceProvider>();
        var ctx = new SkywalkerClaimsPrincipalContributorContext(principal, sp);
        Assert.Same(principal, ctx.ClaimsPrincipal);
        Assert.Same(sp, ctx.ServiceProvider);
    }

    // SkywalkerClaimsPrincipalFactoryOptions
    [Fact]
    public void ClaimsPrincipalFactoryOptions_HasDefaults()
    {
        var options = new SkywalkerClaimsPrincipalFactoryOptions();
        Assert.NotNull(options.Contributors);
        Assert.NotNull(options.DynamicContributors);
        Assert.NotNull(options.DynamicClaims);
        Assert.NotNull(options.ClaimsMap);
        Assert.True(options.DynamicClaims.Count > 0);
        Assert.True(options.ClaimsMap.Count > 0);
    }

    [Fact]
    public void ClaimsPrincipalFactoryOptions_DynamicClaims_ContainsUserName()
    {
        var options = new SkywalkerClaimsPrincipalFactoryOptions();
        Assert.Contains(SkywalkerClaimTypes.UserName, options.DynamicClaims);
    }

    [Fact]
    public void ClaimsPrincipalFactoryOptions_ClaimsMap_ContainsUserNameMapping()
    {
        var options = new SkywalkerClaimsPrincipalFactoryOptions();
        Assert.True(options.ClaimsMap.ContainsKey(SkywalkerClaimTypes.UserName));
    }

    // CurrentClient
    [Fact]
    public void CurrentClient_NotAuthenticated_WhenNoPrincipal()
    {
        var accessor = Substitute.For<ICurrentPrincipalAccessor>();
        accessor.Principal.Returns((ClaimsPrincipal?)null);
        var client = new CurrentClient(accessor);
        Assert.False(client.IsAuthenticated);
        Assert.Null(client.Id);
    }
}
