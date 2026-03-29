using System.Security.Claims;
using NSubstitute;
using Skywalker.Security.Claims;
using Skywalker.Security.Clients;

namespace Skywalker.Security.Tests;

public class CurrentClientTests
{
    private readonly ICurrentPrincipalAccessor _principalAccessor;
    private readonly CurrentClient _currentClient;

    public CurrentClientTests()
    {
        _principalAccessor = Substitute.For<ICurrentPrincipalAccessor>();
        _currentClient = new CurrentClient(_principalAccessor);
    }

    [Fact]
    public void IsAuthenticated_ReturnsFalse_WhenNoPrincipal()
    {
        _principalAccessor.Principal.Returns((ClaimsPrincipal?)null);

        Assert.False(_currentClient.IsAuthenticated);
    }

    [Fact]
    public void IsAuthenticated_ReturnsTrue_WhenAuthenticated()
    {
        var identity = new ClaimsIdentity(new[] { new Claim(SkywalkerClaimTypes.ClientId, "app-1") }, "TestAuth");
        _principalAccessor.Principal.Returns(new ClaimsPrincipal(identity));

        Assert.True(_currentClient.IsAuthenticated);
    }

    [Fact]
    public void Id_ReturnsClientId_WhenAuthenticated()
    {
        var identity = new ClaimsIdentity(new[] { new Claim(SkywalkerClaimTypes.ClientId, "my-client") }, "TestAuth");
        _principalAccessor.Principal.Returns(new ClaimsPrincipal(identity));

        Assert.Equal("my-client", _currentClient.Id);
    }

    [Fact]
    public void Id_ReturnsNull_WhenNoPrincipal()
    {
        _principalAccessor.Principal.Returns((ClaimsPrincipal?)null);

        Assert.Null(_currentClient.Id);
    }
}
