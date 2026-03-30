using System.Security.Claims;
using System.Security.Principal;
using Skywalker.Security.Claims;

namespace Skywalker.Security.Tests;

public class ClaimsIdentityExtensionsTests
{
    [Fact]
    public void FindUserId_ReturnsClaim_WhenExists()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(SkywalkerClaimTypes.UserId, "user-42")
        }, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        Assert.Equal("user-42", principal.FindUserId());
    }

    [Fact]
    public void FindUserId_ReturnsNull_WhenNoClaim()
    {
        var identity = new ClaimsIdentity(Array.Empty<Claim>(), "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        Assert.Null(principal.FindUserId());
    }

    [Fact]
    public void FindClientId_ReturnsClaim_WhenExists()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(SkywalkerClaimTypes.ClientId, "client-abc")
        }, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        Assert.Equal("client-abc", principal.FindClientId());
    }

    [Fact]
    public void FindClientId_ReturnsNull_WhenNoClaim()
    {
        var identity = new ClaimsIdentity(Array.Empty<Claim>(), "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        Assert.Null(principal.FindClientId());
    }

    [Fact]
    public void FindTenantcyId_ReturnsClaim_WhenExists()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(SkywalkerClaimTypes.TenantcyId, "100")
        }, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        Assert.Equal(100L, principal.FindTenantcyId());
    }

    [Fact]
    public void FindTenantcyId_ReturnsNull_WhenNoClaim()
    {
        var identity = new ClaimsIdentity(Array.Empty<Claim>(), "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        Assert.Null(principal.FindTenantcyId());
    }

    [Fact]
    public void AddIfNotContains_AddsClaim_WhenNotExists()
    {
        var identity = new ClaimsIdentity(Array.Empty<Claim>(), "TestAuth");
        var claim = new Claim("type", "value");

        identity.AddIfNotContains(claim);

        Assert.Contains(identity.Claims, c => c.Type == "type" && c.Value == "value");
    }

    [Fact]
    public void AddIfNotContains_DoesNotDuplicate_WhenAlreadyExists()
    {
        var identity = new ClaimsIdentity(new[] { new Claim("type", "value") }, "TestAuth");
        var claim = new Claim("type", "value");

        identity.AddIfNotContains(claim);

        Assert.Single(identity.Claims, c => c.Type == "type" && c.Value == "value");
    }

    [Fact]
    public void AddOrReplace_ReplacesExistingClaim()
    {
        var identity = new ClaimsIdentity(new[] { new Claim("type", "old") }, "TestAuth");

        identity.AddOrReplace(new Claim("type", "new"));

        Assert.Single(identity.Claims, c => c.Type == "type");
        Assert.Equal("new", identity.FindFirst("type")!.Value);
    }

    [Fact]
    public void AddOrReplace_AddsClaim_WhenNotExists()
    {
        var identity = new ClaimsIdentity(Array.Empty<Claim>(), "TestAuth");

        identity.AddOrReplace(new Claim("newtype", "value"));

        Assert.Equal("value", identity.FindFirst("newtype")!.Value);
    }

    [Fact]
    public void AddIdentityIfNotContains_AddsNewIdentity()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>(), "Primary"));
        var extra = new ClaimsIdentity(Array.Empty<Claim>(), "Secondary");

        principal.AddIdentityIfNotContains(extra);

        Assert.Equal(2, principal.Identities.Count());
    }
}
