using System.Security.Claims;
using Skywalker.Security.Claims;

namespace Skywalker.Security.Tests;

public class CurrentPrincipalAccessorTests
{
    [Fact]
    public void ThreadCurrentPrincipalAccessor_ReturnsPrincipal()
    {
        var accessor = new ThreadCurrentPrincipalAccessor();

        // When Thread.CurrentPrincipal is a ClaimsPrincipal, it should be returned
        var identity = new ClaimsIdentity(new[] { new Claim("test", "value") }, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var old = Thread.CurrentPrincipal;
        try
        {
            Thread.CurrentPrincipal = principal;
            Assert.Same(principal, accessor.Principal);
        }
        finally
        {
            Thread.CurrentPrincipal = old;
        }
    }

    [Fact]
    public void Change_OverridesPrincipal_AndRestoresOnDispose()
    {
        var accessor = new ThreadCurrentPrincipalAccessor();
        var originalPrincipal = accessor.Principal;

        var identity = new ClaimsIdentity(new[] { new Claim("override", "yes") }, "Override");
        var newPrincipal = new ClaimsPrincipal(identity);

        using (accessor.Change(newPrincipal))
        {
            Assert.Same(newPrincipal, accessor.Principal);
        }

        // After dispose, should restore
        Assert.NotSame(newPrincipal, accessor.Principal);
    }

    [Fact]
    public void Change_SupportsNesting()
    {
        var accessor = new ThreadCurrentPrincipalAccessor();

        var p1 = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("level", "1") }, "L1"));
        var p2 = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("level", "2") }, "L2"));

        using (accessor.Change(p1))
        {
            Assert.Same(p1, accessor.Principal);

            using (accessor.Change(p2))
            {
                Assert.Same(p2, accessor.Principal);
            }

            Assert.Same(p1, accessor.Principal);
        }
    }

    [Fact]
    public void Change_WithClaims_CreatesPrincipal()
    {
        var accessor = new ThreadCurrentPrincipalAccessor();
        var claims = new[] { new Claim("custom-key", "custom-value") };

        using (accessor.Change(claims))
        {
            Assert.NotNull(accessor.Principal);
            Assert.Contains(accessor.Principal!.Claims, c => c.Type == "custom-key" && c.Value == "custom-value");
        }
    }
}
