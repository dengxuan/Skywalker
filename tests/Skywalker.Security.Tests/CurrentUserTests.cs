using System.Security.Claims;
using NSubstitute;
using Skywalker.Security.Claims;
using Skywalker.Security.Users;

namespace Skywalker.Security.Tests;

public class CurrentUserTests
{
    private readonly ICurrentPrincipalAccessor _principalAccessor;
    private readonly CurrentUser _currentUser;

    public CurrentUserTests()
    {
        _principalAccessor = Substitute.For<ICurrentPrincipalAccessor>();
        _currentUser = new CurrentUser(_principalAccessor);
    }

    [Fact]
    public void IsAuthenticated_ReturnsFalse_WhenNoPrincipal()
    {
        _principalAccessor.Principal.Returns((ClaimsPrincipal?)null);

        Assert.False(_currentUser.IsAuthenticated);
    }

    [Fact]
    public void IsAuthenticated_ReturnsTrue_WhenAuthenticated()
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "123") }, "TestAuth");
        _principalAccessor.Principal.Returns(new ClaimsPrincipal(identity));

        Assert.True(_currentUser.IsAuthenticated);
    }

    [Fact]
    public void Id_ReturnsNull_WhenNoPrincipal()
    {
        _principalAccessor.Principal.Returns((ClaimsPrincipal?)null);

        Assert.Null(_currentUser.Id);
    }

    [Fact]
    public void Id_ReturnsUserId_WhenAuthenticated()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(SkywalkerClaimTypes.UserId, "user-123")
        }, "TestAuth");
        _principalAccessor.Principal.Returns(new ClaimsPrincipal(identity));

        Assert.Equal("user-123", _currentUser.Id);
    }

    [Fact]
    public void Username_ReturnsClaimValue()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(SkywalkerClaimTypes.UserName, "johndoe")
        }, "TestAuth");
        _principalAccessor.Principal.Returns(new ClaimsPrincipal(identity));

        Assert.Equal("johndoe", _currentUser.Username);
    }

    [Fact]
    public void Email_ReturnsClaimValue()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(SkywalkerClaimTypes.Email, "john@example.com")
        }, "TestAuth");
        _principalAccessor.Principal.Returns(new ClaimsPrincipal(identity));

        Assert.Equal("john@example.com", _currentUser.Email);
    }

    [Fact]
    public void PhoneNumber_ReturnsClaimValue()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("phone_number", "+1234567890")
        }, "TestAuth");
        _principalAccessor.Principal.Returns(new ClaimsPrincipal(identity));

        Assert.Equal("+1234567890", _currentUser.PhoneNumber);
    }

    [Fact]
    public void Roles_ReturnsAllRoleClaims()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(SkywalkerClaimTypes.Role, "admin"),
            new Claim(SkywalkerClaimTypes.Role, "user"),
        }, "TestAuth");
        _principalAccessor.Principal.Returns(new ClaimsPrincipal(identity));

        Assert.Equal(new[] { "admin", "user" }, _currentUser.Roles);
    }

    [Fact]
    public void IsInRole_ReturnsTrue_WhenUserHasRole()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(SkywalkerClaimTypes.Role, "admin")
        }, "TestAuth");
        _principalAccessor.Principal.Returns(new ClaimsPrincipal(identity));

        Assert.True(_currentUser.IsInRole("admin"));
    }

    [Fact]
    public void IsInRole_ReturnsFalse_WhenUserDoesNotHaveRole()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(SkywalkerClaimTypes.Role, "user")
        }, "TestAuth");
        _principalAccessor.Principal.Returns(new ClaimsPrincipal(identity));

        Assert.False(_currentUser.IsInRole("admin"));
    }

    [Fact]
    public void FindClaim_ReturnsClaim_WhenExists()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("custom", "value")
        }, "TestAuth");
        _principalAccessor.Principal.Returns(new ClaimsPrincipal(identity));

        var claim = _currentUser.FindClaim("custom");

        Assert.NotNull(claim);
        Assert.Equal("value", claim!.Value);
    }

    [Fact]
    public void FindClaim_ReturnsNull_WhenNotExists()
    {
        var identity = new ClaimsIdentity(Array.Empty<Claim>(), "TestAuth");
        _principalAccessor.Principal.Returns(new ClaimsPrincipal(identity));

        Assert.Null(_currentUser.FindClaim("nonexistent"));
    }

    [Fact]
    public void FindClaims_ReturnsMultipleClaims()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("multi", "val1"),
            new Claim("multi", "val2"),
        }, "TestAuth");
        _principalAccessor.Principal.Returns(new ClaimsPrincipal(identity));

        var claims = _currentUser.FindClaims("multi");

        Assert.Equal(2, claims.Length);
    }

    [Fact]
    public void GetAllClaims_ReturnsAllClaims()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("a", "1"),
            new Claim("b", "2"),
        }, "TestAuth");
        _principalAccessor.Principal.Returns(new ClaimsPrincipal(identity));

        var claims = _currentUser.GetAllClaims();

        Assert.True(claims.Length >= 2);
    }
}
